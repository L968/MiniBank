using MiniBank.Api.Domain;

namespace MiniBank.Api.Features.Transactions.Queries.GetTransactionsByUserId;

internal sealed record GetTransactionsByUserIdResponse(
    Guid Id,
    int PayerId,
    int PayeeId,
    decimal Value,
    DateTime Timestamp,
    TransactionStatus Status,
    string Message
);
