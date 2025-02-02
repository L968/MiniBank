using MassTransit;

namespace MiniBank.Api.Infrastructure.EventBus;

internal class RabbitMQService(IBus bus) : IRabbitMQService
{
    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken) where T : class
    {
        await bus.Publish(message, cancellationToken);
    }
}
