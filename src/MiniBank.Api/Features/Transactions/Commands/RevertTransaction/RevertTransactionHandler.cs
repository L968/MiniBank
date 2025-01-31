using MiniBank.Api.Domain;
using MiniBank.Api.Infrastructure.Repositories.Interfaces;

namespace MiniBank.Api.Features.Transactions.Commands.RevertTransaction;

internal sealed class RevertTransactionHandler(
    ITransactionRepository transactionRepository,
    IUnitOfWork unitOfWork,
    ILogger<RevertTransactionHandler> logger
) : IRequestHandler<RevertTransactionCommand>
{
    public async Task Handle(RevertTransactionCommand request, CancellationToken cancellationToken)
    {
        Transaction? transaction = await transactionRepository.GetByIdAsync(request.TransactionId, cancellationToken);
        if (transaction is null)
        {
            throw new AppException($"Transaction with ID {request.TransactionId} not found.");
        }

        transaction.Revert();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Transaction reverted successfully: TransactionId: {TransactionId}", transaction.Id);
    }
}
