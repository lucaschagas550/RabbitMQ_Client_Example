using MessageBus.MessageBus;
using MessageBus.Messages.Integration;
using System.Diagnostics;

namespace RabbitMQ_Integration_Example.Service
{
    public class IntegrationHandler : BackgroundService
    {
        private int TotalMessagesReceived = 0;
        private readonly IMessageBus _bus;

        public IntegrationHandler(IMessageBus bus) =>
            _bus = bus;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            SetSubscribers();

            return Task.CompletedTask;
        }

        private void SetSubscribers()
        {
            var personConsumer = _bus.CreateBasicConsumer(
                _bus.CreateChannel($"{nameof(PersonIntegration)}Queue", $"{nameof(PersonIntegration)}Exchange", ExchangeType.topic, false, $"{nameof(PersonIntegration)}Key"));
            
            var carConsumer = _bus.CreateBasicConsumer(
                _bus.CreateChannel($"{nameof(CarIntegration)}Queue", $"{nameof(PersonIntegration)}Exchange", ExchangeType.topic, false,$"{nameof(CarIntegration)}Key"));

            personConsumer.Received += (channel, eventArgs) =>
            {
                var content = _bus.GetMessageContent<PersonIntegration>(eventArgs);
                _bus?.GetChannel()?.BasicAck(eventArgs.DeliveryTag, false);
                TotalMessagesReceived++;
                
                Debug.WriteLine($"Person => {DateTime.Now}: {content.Name} {content.Age} {content.Date} {content.Timestamp}");
                Console.WriteLine($"Person => {DateTime.Now}: {content.Name} {content.Age} {content.Timestamp}");

                Console.WriteLine($"Total messages Received: {TotalMessagesReceived} \n Total time: {DateTime.Now - content.Date}");
                Debug.WriteLine($"Total messages Received: {TotalMessagesReceived} \n Total time: {DateTime.Now - content.Date}");

                //Enviar outra messagem ou disparar algum evento abaixo se desejar apos obter a messagem serializada
            };

            carConsumer.Received += (channel, eventArgs) =>
            {
                var content = _bus.GetMessageContent<CarIntegration>(eventArgs);
                _bus?.GetChannel()?.BasicAck(eventArgs.DeliveryTag, false);
                Debug.WriteLine($"Car =>{DateTime.Now}: {content.Name} {content.Age}");
                Console.WriteLine($"Car =>{DateTime.Now}: {content.Name} {content.Age}");

                //Enviar outra messagem ou disparar algum evento abaixo se desejar apos obter a messagem serializada
            };

            _bus.BasicConsume($"{nameof(PersonIntegration)}Queue", false, personConsumer);
            _bus.BasicConsume($"{nameof(CarIntegration)}Queue", false, carConsumer);
        }
    }
}