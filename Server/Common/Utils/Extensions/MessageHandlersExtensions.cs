using System.Text;
using CommonServer.Utils.RabbitMq;
using RabbitMQ.Client.Events;

namespace CommonServer.Utils.Extensions;

public static class RabbitMqServiceExtensions
{
    public static async Task HandleMessage(
        this MessageHandlers handlers,
        BasicDeliverEventArgs e
    )
    {
        var props = e.BasicProperties;
        var message = Encoding.UTF8.GetString(e.Body.ToArray());

        var handler = handlers[props.CorrelationId];

        if (handler != null)
        {
            handlers.RemoveHandler(props.CorrelationId);
            await handler.Handle(e, message);
        }
    }
}
