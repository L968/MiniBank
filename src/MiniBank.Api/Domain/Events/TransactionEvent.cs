namespace MiniBank.Api.Domain.Events;

internal sealed record TransactionEvent
{
    public Guid TransactionId { get; init; }
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;

    public TransactionEvent(Guid transactionId)
    {
        TransactionId = transactionId;
    }
}
