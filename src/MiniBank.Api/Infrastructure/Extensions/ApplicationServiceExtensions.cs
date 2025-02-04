using System.Globalization;
using System.Reflection;
using Evently.Common.Application.Behaviors;
using MassTransit;
using MiniBank.Api.Behaviours;
using MiniBank.Api.Infrastructure.EventBus;

namespace MiniBank.Api.Infrastructure.Extensions;

internal static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, Assembly assembly)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            config.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
        });

        services.AddRabbitMQService();

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
        ValidatorOptions.Global.LanguageManager.Culture = CultureInfo.InvariantCulture;

        return services;
    }

    private static void AddRabbitMQService(this IServiceCollection services)
    {
        services.AddScoped<IRabbitMQService, RabbitMQService>();

        services.AddMassTransit(config =>
        {
            config.AddConsumer<TransactionEventConsumer>();

            config.AddConfigureEndpointsCallback((context, name, cfg) =>
            {
                cfg.UseMessageRetry(r => r.Interval(5, 5000));
            });

            config.UsingRabbitMq((context, configurator) =>
            {
                configurator.Host(Config.RabbitMQConnectionString);

                configurator.ConfigureEndpoints(context);
            });
        });
    }
}
