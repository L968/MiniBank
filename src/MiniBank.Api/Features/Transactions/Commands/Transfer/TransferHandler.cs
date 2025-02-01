using MiniBank.Api.Domain;
using MiniBank.Api.Infrastructure.Repositories.Interfaces;
using MiniBank.Api.Infrastructure.Services;

namespace MiniBank.Api.Features.Transactions.Commands.Transfer;

internal sealed class TransferHandler(
    IUserRepository userRepository,
    ITransactionRepository transactionRepository,
    IAuthorizationService authorizationService,
    INotificationService notificationService,
    IUnitOfWork unitOfWork,
    ILogger<TransferHandler> logger
) : IRequestHandler<TransferCommand, TransferResponse>
{
    public async Task<TransferResponse> Handle(TransferCommand request, CancellationToken cancellationToken)
    {
        User? payer = await userRepository.GetByIdAsync(request.Payer, cancellationToken);
        if (payer is null)
        {
            throw new AppException($"Payer with ID {request.Payer} not found.");
        }

        User? payee = await userRepository.GetByIdAsync(request.Payee, cancellationToken);
        if (payee is null)
        {
            throw new AppException($"Payee with ID {request.Payee} not found.");
        }

        var transaction = new Transaction(payer, payee, request.Value);

        bool isTransactionAuthorized = await authorizationService.IsTransactionAuthorizedAsync();

        if (isTransactionAuthorized)
        {
            transaction.Execute();
            logger.LogInformation("Transaction completed successfully: TransactionId: {TransactionId}", transaction.Id);
        }
        else
        {
            transaction.Fail();
            logger.LogError("Transaction authorization failed for TransactionId: {TransactionId}", transaction.Id);
        }

        transactionRepository.Create(transaction);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        try
        {
            await notificationService.Notify();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Notification failed for transaction {TransactionId}. PayerId: {PayerId}, PayeeId: {PayeeId}",
                transaction.Id, payer.Id, payee.Id);
        }

        return new TransferResponse(
            payer.Id,
            payee.Id,
            request.Value,
            payer.Balance,
            payee.Balance
        );
    }
}
