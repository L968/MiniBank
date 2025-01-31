namespace MiniBank.Api.Infrastructure.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        var serverVersion = ServerVersion.AutoDetect(Config.DatabaseConnectionString);

        services.AddDbContext<AppDbContext>(options =>
            options
                .UseMySql(
                    Config.DatabaseConnectionString,
                    serverVersion,
                    mysqlOptions => mysqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
                )
        );

        return services;
    }
}
