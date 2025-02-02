namespace MiniBank.Api.Infrastructure.EventBus;

internal interface IRabbitMQService
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken) where T : class;
}
