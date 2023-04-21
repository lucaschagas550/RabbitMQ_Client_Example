using MessageBus.Extensions;
using MessageBus.Messages.Base;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MessageBus.MessageBus
{
    public class MessageBus : BaseMessageBus, IMessageBus, IDisposable
    {
        private IModel? _channel;
        private IConnection? _connection;
        private readonly RabbitMQConfiguration _rabbitMqConfiguration;

        public MessageBus(RabbitMQConfiguration rabbitMqConfiguration)
        {
            _rabbitMqConfiguration = rabbitMqConfiguration;
            EnsureConnection();
        }

        public IConnection? Connection => _connection;

        public void Publish<T>(T message, string exchangeName, ExchangeType exchangeType, string queue,
            string? routingKey = "") where T : IntegrationEvent
        {
            if (ConnectionExists())
            {
                using var channel = _connection?.CreateModel();

                QueueSetup(exchangeName, exchangeType, queue, routingKey, channel);

                var body = GetMessageAsByteArray(message);

                channel.BasicPublish(exchangeName, routingKey, null, body);
            }
        }

        public EventingBasicConsumer CreateBasicConsumer(IModel channel) =>
            new EventingBasicConsumer(channel);

        public IModel CreateChannel(string queue, string exchangeName, ExchangeType exchangeType, bool isAutoDelete = false, string? routingKey = "")
        {
            var channel = _connection?.CreateModel();
            QueueSetup(exchangeName, exchangeType, queue, routingKey, channel, isAutoDelete);

            _channel = channel;
            return channel;
        }

        public T? GetMessageContent<T>(BasicDeliverEventArgs eventArgs) where T : IntegrationEvent =>
            DeserializeMessage<T>(eventArgs);

        public IModel? GetChannel() =>
            _channel;

        public void BasicConsume(string queue, bool autoAck, EventingBasicConsumer consumer) =>
            _channel.BasicConsume(queue, autoAck, consumer);

        private static void QueueSetup(string exchangeName, ExchangeType exchangeType, string queue, string? routingKey, IModel? channel, bool isAutoDelete = false)
        {
            channel?.ExchangeDeclare(exchangeName, exchangeType.ToString(), true);
            channel?.QueueDeclare(queue, true, false, isAutoDelete, null);
            channel?.QueueBind(queue, exchangeName, routingKey);
        }

        private void EnsureConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _rabbitMqConfiguration.HostName,
                    UserName = _rabbitMqConfiguration.UserName,
                    Password = _rabbitMqConfiguration.Password
                };
                _connection = factory.CreateConnection();
            }
            catch (Exception)
            {
                //Log exception
                throw;
            }
        }

        private bool ConnectionExists()
        {
            if (_connection == null)
                EnsureConnection();

            return _connection != null;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}