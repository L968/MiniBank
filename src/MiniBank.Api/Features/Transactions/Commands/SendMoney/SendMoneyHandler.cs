using MiniBank.Api.Domain;
using MiniBank.Api.Infrastructure.Repositories.Interfaces;
using MiniBank.Api.Infrastructure.Services;

namespace MiniBank.Api.Features.Transactions.Commands.SendMoney;

internal sealed class SendMoneyHandler(
    IUserRepository userRepository,
    ITransactionRepository transactionRepository,
    IAuthorizationService authorizationService,
    INotificationService notificationService,
    IUnitOfWork unitOfWork,
    ILogger<SendMoneyHandler> logger
) : IRequestHandler<SendMoneyCommand, SendMoneyResponse>
{
    public async Task<SendMoneyResponse> Handle(SendMoneyCommand request, CancellationToken cancellationToken)
    {
        User? sender = await userRepository.GetByIdAsync(request.SenderId, cancellationToken);
        if (sender is null)
        {
            throw new AppException($"Sender with ID {request.SenderId} not found.");
        }

        User? receiver = await userRepository.GetByIdAsync(request.ReceiverId, cancellationToken);
        if (receiver is null)
        {
            throw new AppException($"Receiver with ID {request.ReceiverId} not found.");
        }

        var transaction = new Transaction(sender, receiver, request.Amount);

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
            logger.LogError(ex, "Notification failed for transaction {TransactionId}. SenderId: {SenderId}, ReceiverId: {ReceiverId}",
                transaction.Id, sender.Id, receiver.Id);
        }

        return new SendMoneyResponse(
            sender.Id,
            receiver.Id,
            request.Amount,
            sender.Balance,
            receiver.Balance
        );
    }
}
