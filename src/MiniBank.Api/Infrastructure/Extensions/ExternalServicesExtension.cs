using MiniBank.Api.Infrastructure.Services;

namespace MiniBank.Api.Infrastructure.Extensions;

internal static class ExternalServicesExtension
{
    public static void AddExternalServices(this IServiceCollection services)
    {
        services.AddHttpClient<IAuthorizationService, AuthorizationService>((serviceProvider, client) =>
        {
            client.BaseAddress = new Uri(Config.AuthorizerUrl);
        });

        services.AddHttpClient<INotificationService, NotificationService>((serviceProvider, client) =>
        {
            client.BaseAddress = new Uri(Config.NotificationUrl);
        });
    }
}
