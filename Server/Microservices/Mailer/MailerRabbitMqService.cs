using System.Text;
using System.Text.Json;
using CommonServer.Asp.HostedServices;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using CommonServer.Data.Messages;
using CommonServer.Utils.Extensions;

namespace Mailer;

public class MailerRabbitMqService: RabbitMqService
{
    private readonly ILogger<MailerRabbitMqService> _logger;
    private readonly IConfiguration _configuration;

    public MailerRabbitMqService(
        IConnectionFactory factory,
        ILogger<MailerRabbitMqService> logger,
        RabbitMqProvider provider,
        IConfiguration configuration
    ) : base(factory, logger, provider)
    {
        _logger = logger;
        _configuration = configuration;
    }

    async Task SendMessage(MailMessage mail)
    {
        var mime = new MimeMessage
        {
            Sender = new MailboxAddress(mail.SenderName ?? _configuration["MAIL_DEFAULT_SENDER_NAME"]!, _configuration["MAIL_USERNAME"]!)
        };
        mime.To.AddRange(mail.Receivers.Select(m => new MailboxAddress("", m)));
        if (mail.Title != null)
        {
            mime.Subject = mail.Title;
        }
        mime.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = mail.Body
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(
            _configuration["MAIL_HOST"]!,
            Convert.ToInt32(_configuration["MAIL_PORT"]),
            Convert.ToBoolean(_configuration["MAIL_USE_SSL"])
        );
        await client.AuthenticateAsync(_configuration["MAIL_USERNAME"]!, _configuration["MAIL_PASSWORD"]!);
        await client.SendAsync(mime);
        await client.DisconnectAsync(true);
    }

    protected override void OnModeling(IModel model)
    {
        model.MailerRequestQueueDeclare(_configuration["MAILER_REQUEST_QUEUE"]!);

        var consumer = new AsyncEventingBasicConsumer(model);

        consumer.Received += async (_, e) =>
        {
            _logger.LogInformation("Received");

            var props = e.BasicProperties;
            var message = Encoding.UTF8.GetString(e.Body.ToArray());
            MailMessage? mail = JsonSerializer.Deserialize<MailMessage>(message);

            ResultMessage? result = null;

            if (mail == null)
            {
                model.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
                result = new ResultMessage(false, "Received incorrect message (MailMessage only)");
                _logger.LogInformation("Received incorrect message");
            }
            else
            {
                try
                {
                    await SendMessage(mail);
                    model.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
                    result = new ResultMessage(true, "Email sent");
                    _logger.LogInformation("Email sent");
                }
                catch (Exception error)
                {
                    model.BasicNack(deliveryTag: e.DeliveryTag, multiple: false, requeue: true);
                    _logger.LogError(error.ToString());
                }
            }

            if (props.ReplyTo != String.Empty && result != null)
            {
                var response = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(result));

                var replyProps = model.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                model.BasicPublish(
                    exchange: string.Empty,
                    routingKey: props.ReplyTo,
                    basicProperties: replyProps,
                    body: response
                );
            }
        };

        model.BasicConsume(_configuration["MAILER_REQUEST_QUEUE"]!, autoAck: false, consumer);
    }
}
