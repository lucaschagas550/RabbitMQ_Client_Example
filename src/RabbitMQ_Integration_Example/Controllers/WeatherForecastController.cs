using MessageBus.MessageBus;
using MessageBus.Messages.Integration;
using Microsoft.AspNetCore.Mvc;

namespace RabbitMQ_Integration_Example.Controllers
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

        public WeatherForecastController(ILogger<WeatherForecastController> logger,
                                         IMessageBus bus)
        {
            _logger = logger;
            _bus = bus;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {

            //var count = 0;

            //while(count < 3)
            //{
            //    _bus.SendMessage(new PersonIntegration("Lucas", 25), $"{nameof(PersonIntegration)}Exchange", ExchangeType.topic, $"{nameof(PersonIntegration)}Queue", $"{nameof(PersonIntegration)}Key");
            //}

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}