using System.Collections.Concurrent;
using RabbitMQ.Client.Events;

namespace CommonServer.Utils.RabbitMq;

public interface IMessageHandler
{
    Task Handle(BasicDeliverEventArgs e, string? message);
}

public class MessageHandlers
{
    private readonly ConcurrentDictionary<string, IMessageHandler> Handlers = new();

    public IMessageHandler? this[string correlationId]
    {
        get
        {
            Handlers.TryGetValue(correlationId, out IMessageHandler? handler);
            return handler;
        }
    }

    public void AddHandler(string correlationId, IMessageHandler handler)
    {
        if (Handlers.ContainsKey(correlationId))
        {
            throw new Exception("already exists");
        }
        Handlers[correlationId] = handler;
    }

    public void RemoveHandler(string correlationId)
    {
        Handlers.Remove(correlationId, out _);
    }
}
