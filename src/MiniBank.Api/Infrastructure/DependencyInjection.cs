using System.Reflection;
using MiniBank.Api.Endpoints;
using MiniBank.Api.Infrastructure.Extensions;

namespace MiniBank.Api.Infrastructure;

internal static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, Assembly assembly)
    {
        Config.Init(configuration);

        services.AddDatabase();
        services.AddApplicationServices(assembly);
        services.AddExternalServices();
        services.AddEndpoints(assembly);
        services.AddCorsConfiguration();
        services.AddHealthChecksConfiguration();
        services.AddDocumentation();

        return services;
    }
}
