namespace MiniBank.Api.Features.Transactions.Commands.RevertTransaction;

internal sealed record RevertTransactionCommand(Guid TransactionId) : IRequest;
