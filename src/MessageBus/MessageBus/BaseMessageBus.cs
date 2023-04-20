using MessageBus.Messages.Base;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace MessageBus.MessageBus
{
    public abstract class BaseMessageBus
    {
        protected static byte[] GetMessageAsByteArray<T>(T message) where T : IntegrationEvent
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            var content = JsonSerializer.Serialize(message, options);
            return Encoding.UTF8.GetBytes(content);
        }

        protected static T? DeserializeMessage<T>(BasicDeliverEventArgs eventArgs) where T : IntegrationEvent
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            return JsonSerializer.Deserialize<T>(message, options);
        }
    }
}