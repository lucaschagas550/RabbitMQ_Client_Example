using MessageBus.Messages.Base;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MessageBus.MessageBus
{
    public interface IMessageBus
    {
        IConnection? Connection { get; }
        IModel? GetChannel();
        EventingBasicConsumer CreateBasicConsumer(IModel channel);
        IModel CreateChannel(string queue, string exchangeName, ExchangeType exchangeType, bool isExclusive = false, string? routingKey = "");
        void Publish<T>(T message, string exchangeName, ExchangeType exchangeType, string queue, string? routingKey = "") where T : IntegrationEvent;
        void BasicConsume(string queue, bool autoAck, EventingBasicConsumer consumer);
        T? GetMessageContent<T>(BasicDeliverEventArgs eventArgs) where T : IntegrationEvent;
    }
}
