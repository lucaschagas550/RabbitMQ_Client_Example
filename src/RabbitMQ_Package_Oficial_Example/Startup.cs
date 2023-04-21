using MessageBus.Extensions;
using MessageBus.MessageBus;

namespace RabbitMQ_Package_Oficial_Example
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
            services.AddMessageBus(rabbitMQConfiguration);
            services.Configure<RabbitMQConfiguration>(Configuration.GetSection("RabbitMQ"));
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