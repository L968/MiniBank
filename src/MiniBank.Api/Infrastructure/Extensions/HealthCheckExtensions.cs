using RabbitMQ.Client;

namespace MiniBank.Api.Infrastructure.Extensions;

internal static class HealthCheckExtensions
{
    public static IServiceCollection AddHealthChecksConfiguration(this IServiceCollection services)
    {
        services
            .AddSingleton(sp =>
            {
                var factory = new ConnectionFactory
                {
                    Uri = new Uri(Config.RabbitMQConnectionString),
                };
                return factory.CreateConnectionAsync().GetAwaiter().GetResult();
            })
            .AddHealthChecks()
            .AddDbContextCheck<AppDbContext>()
            .AddRabbitMQ();

        return services;
    }
}
