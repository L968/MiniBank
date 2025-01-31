using MiniBank.Api.Domain;

namespace MiniBank.Api.Features.Transactions.Queries.GetTransactionsByUserId;

internal sealed record GetTransactionsByUserIdResponse(
    Guid Id,
    Guid SenderId,
    Guid ReceiverId,
    decimal Amount,
    DateTime Timestamp,
    TransactionStatus Status
);
