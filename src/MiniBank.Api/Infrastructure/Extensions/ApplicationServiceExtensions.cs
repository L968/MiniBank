using System.Globalization;
using System.Reflection;
using Evently.Common.Application.Behaviors;
using MassTransit;
using MiniBank.Api.Behaviours;
using MiniBank.Api.Infrastructure.EventBus;
using MiniBank.Api.Infrastructure.Repositories;
using MiniBank.Api.Infrastructure.Repositories.Interfaces;

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

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IRabbitMQService, RabbitMQService>();

        return services;
    }

    private static void AddRabbitMQService(this IServiceCollection services)
    {
        services.AddMassTransit(config =>
        {
            config.AddConsumer<TransactionEventConsumer>();

            config.AddConfigureEndpointsCallback((context, name, cfg) =>
            {
                cfg.UseMessageRetry(r => r.Interval(5, 5000));
            });

            config.UsingRabbitMq((context, configurator) =>
            {
                configurator.Host(new Uri(Config.RabbitMQConnectionString), host =>
                {
                    host.Username("guest");
                    host.Password("guest");
                });

                configurator.ConfigureEndpoints(context);
            });
        });
    }
}
