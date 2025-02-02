using MiniBank.Api.Domain;
using MiniBank.Api.Infrastructure;

namespace MiniBank.Api.Features.Transactions.Commands.RevertTransaction;

internal sealed class RevertTransactionHandler(
    AppDbContext dbContext,
    ILogger<RevertTransactionHandler> logger
) : IRequestHandler<RevertTransactionCommand>
{
    public async Task Handle(RevertTransactionCommand request, CancellationToken cancellationToken)
    {
        Transaction? transaction = await dbContext.Transactions
            .Include(t => t.Payer)
            .Include(t => t.Payee)
            .FirstOrDefaultAsync(t => t.Id == request.TransactionId, cancellationToken);

        if (transaction is null)
        {
            throw new AppException($"Transaction with ID {request.TransactionId} not found.");
        }

        transaction.Revert();

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Transaction reverted successfully: TransactionId: {TransactionId}", transaction.Id);
    }
}
