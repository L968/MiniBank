using MassTransit;
using MiniBank.Api.Domain;
using MiniBank.Api.Domain.Events;
using MiniBank.Api.Infrastructure.Repositories.Interfaces;
using MiniBank.Api.Infrastructure.Services;

namespace MiniBank.Api.Infrastructure.EventBus;

internal sealed class TransactionEventConsumer(
    ITransactionRepository transactionRepository,
    IAuthorizationService authorizationService,
    INotificationService notificationService,
    IUnitOfWork unitOfWork,
    ILogger<TransactionEventConsumer> logger
    ) : IConsumer<TransactionEvent>
{
    public async Task Consume(ConsumeContext<TransactionEvent> context)
    {
        TransactionEvent transactionEvent = context.Message;

        logger.LogInformation("Processing transaction: Id: {Id}", transactionEvent.TransactionId);

        Transaction? transaction = await transactionRepository.GetByIdAsync(transactionEvent.TransactionId, context.CancellationToken);
        if (transaction is null)
        {
            throw new AppException($"Transaction with ID {transactionEvent.TransactionId} not found.");
        }

        bool isTransactionAuthorized = await authorizationService.IsTransactionAuthorizedAsync();

        if (isTransactionAuthorized)
        {
            transaction.Process();
            logger.LogInformation("Transaction processed successfully: TransactionId: {TransactionId}", transaction.Id);
        }
        else
        {
            transaction.Fail("Transaction not authorized.");
            logger.LogWarning("Transaction authorization failed for TransactionId: {TransactionId}", transaction.Id);
        }

        await unitOfWork.SaveChangesAsync(context.CancellationToken);

        try
        {
            await notificationService.Notify();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Notification failed for transaction {TransactionId}. PayerId: {PayerId}, PayeeId: {PayeeId}",
                transaction.Id, transaction.PayerId, transaction.PayeeId);
        }
    }
}
