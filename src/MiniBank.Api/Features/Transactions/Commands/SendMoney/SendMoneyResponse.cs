namespace MiniBank.Api.Features.Transactions.Commands.SendMoney;

internal sealed record SendMoneyResponse(
    Guid SenderId,
    Guid ReceiverId,
    decimal Amount,
    decimal SenderBalanceAfter,
    decimal ReceiverBalanceAfter
);
