namespace MiniBank.Api;

internal static class Config
{
    public static string DatabaseConnectionString { get; private set; } = "";
    public static string RabbitMQConnectionString { get; private set; } = "";
    public static string[] AllowedOrigins { get; private set; } = [];
    public static string AuthorizerUrl { get; private set; } = "";
    public static string NotificationUrl { get; private set; } = "";

    public static void Init(IConfiguration configuration)
    {
        DatabaseConnectionString = configuration.GetConnectionString("Database") ?? throw new MissingConfigurationException("ConnectionStrings:Database");
        RabbitMQConnectionString = configuration.GetConnectionString("RabbitMQ") ?? throw new MissingConfigurationException("ConnectionStrings:RabbitMQ");
        AllowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>() ?? throw new MissingConfigurationException("AllowedOrigins");
        AuthorizerUrl = configuration.GetValue<string>("AuthorizerUrl") ?? throw new MissingConfigurationException("AuthorizerUrl");
        NotificationUrl = configuration.GetValue<string>("NotificationUrl") ?? throw new MissingConfigurationException("NotificationUrl");
    }
}
