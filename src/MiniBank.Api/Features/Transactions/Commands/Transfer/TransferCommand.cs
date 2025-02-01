namespace MiniBank.Api.Features.Transactions.Commands.Transfer;

internal sealed record TransferCommand(
    int Payer,
    int Payee,
    decimal Value
) : IRequest<TransferResponse>;
