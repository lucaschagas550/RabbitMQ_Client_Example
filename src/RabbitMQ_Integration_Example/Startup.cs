using MessageBus.Extensions;
using MessageBus.MessageBus;
using RabbitMQ_Integration_Example.Service;

namespace RabbitMQ_Integration_Example
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) =>
            Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            var rabbitMQConfiguration = new RabbitMQConfiguration();
            Configuration.GetSection("RabbitMQ").Bind(rabbitMQConfiguration);

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddMessageBus(rabbitMQConfiguration)
                .AddHostedService<IntegrationHandler>();
        }

        public static void Configure(WebApplication app, IWebHostEnvironment env)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}