namespace MiniBank.Api.Features.Transactions.Commands.SendMoney;

internal sealed record SendMoneyCommand(
    Guid SenderId,
    Guid ReceiverId,
    decimal Amount
) : IRequest<SendMoneyResponse>;
