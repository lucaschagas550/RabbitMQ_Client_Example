using MessageBus.Extensions;
using MessageBus.MessageBus;
using MessageBus.Messages.Integration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace RabbitMQ_Package_Oficial_Example.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IMessageBus _bus;
        private readonly RabbitMQConfiguration _rabbitMqConfiguration;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,
                                         IMessageBus bus,
                                         IOptions<RabbitMQConfiguration> rabbitMqConfiguration)
        {
            _logger = logger;
            _bus = bus;
            _rabbitMqConfiguration = rabbitMqConfiguration.Value;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var count = 0;

            while (count < 3000)
            {
                count++;
                _bus.Publish(new PersonIntegration("Lucas", 25, DateTime.Now), $"{nameof(PersonIntegration)}Exchange", ExchangeType.topic, $"{nameof(PersonIntegration)}Queue", $"{nameof(PersonIntegration)}Key");
            }

            count = 0;
            while (count < 3000)
            {
                count++;
                _bus.Publish(new CarIntegration("Lucas", 25), $"{nameof(PersonIntegration)}Exchange", ExchangeType.topic, $"{nameof(CarIntegration)}Queue", $"{nameof(CarIntegration)}Key");
            }

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("GetTotalMessages")]
        public async Task<IEnumerable<WeatherForecast>> GetTotalMessages()
        {
            var count = 0;

            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();

            while (sw.Elapsed < TimeSpan.FromSeconds(10))
            {
                count++;
                _bus.Publish(new PersonIntegration("Lucas", 25+count, DateTime.Now),
                        $"{nameof(PersonIntegration)}Exchange", ExchangeType.topic, $"{nameof(PersonIntegration)}Queue", $"{nameof(PersonIntegration)}Key");
            }

            sw.Stop();
            Console.WriteLine($"Total messages sent: {count}");
            Debug.WriteLine($"Total messages sent: {count}");

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            }).ToArray();
        }

        [HttpGet("GetNewConsumer")]
        public async Task<IEnumerable<WeatherForecast>> GetNewConsumer()
        {
            var count = 0;

            using var bus = new MessageBus.MessageBus.MessageBus(_rabbitMqConfiguration);

            var personConsumer = bus.CreateBasicConsumer(
                bus.CreateChannel($"{nameof(PersonIntegration)}Queue2", $"{nameof(PersonIntegration)}Exchange", ExchangeType.topic, true, $"{nameof(PersonIntegration)}Key"));

            personConsumer.Received += (model, ea) =>
            {
                var content = bus.GetMessageContent<PersonIntegration>(ea);

                if (content?.Age < 10000)
                {

                    Debug.WriteLine($"Person NEW QUEUE  => {DateTime.Now}: {content.Name} {content.Age} {content.Date} {content.Timestamp}");
                    Console.WriteLine($"Person NEW QUEUE  => {DateTime.Now}: {content.Name} {content.Age} {content.Timestamp}");

                    bus?.GetChannel()?.BasicAck(ea.DeliveryTag, false);
                }
            };

            bus?.BasicConsume($"{nameof(PersonIntegration)}Queue2", false, personConsumer);

            while (count < 3000)
            {
                count++;
                _bus.Publish(new PersonIntegration("Lucas", 25+count, DateTime.Now),
                        $"{nameof(PersonIntegration)}Exchange", ExchangeType.topic, $"{nameof(PersonIntegration)}Queue", $"{nameof(PersonIntegration)}Key");
            }

            Console.WriteLine($"Total messages sent: {count}");
            Debug.WriteLine($"Total messages sent: {count}");

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            }).ToArray();
        }
    }
}