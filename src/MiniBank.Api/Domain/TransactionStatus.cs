namespace MiniBank.Api.Domain;

internal enum TransactionStatus
{
    Pending = 0,
    Completed = 1,
    Failed = 2,
    Reverted = 3
}
