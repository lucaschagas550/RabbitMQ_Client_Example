using MessageBus.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace MessageBus.MessageBus
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddMessageBus(this IServiceCollection services, RabbitMQConfiguration rabbitMqConfiguration)
        {
            if (rabbitMqConfiguration is null) throw new ArgumentNullException();

            services.AddSingleton<IMessageBus>(new MessageBus(rabbitMqConfiguration));

            return services;
        }
    }
}
