namespace MiniBank.Api.Features.Transactions.Commands.Transfer;

internal sealed record TransferResponse(
    int PayerId,
    int PayeeId,
    decimal Amount,
    decimal PayerBalanceAfter,
    decimal PayeeBalanceAfter
);
