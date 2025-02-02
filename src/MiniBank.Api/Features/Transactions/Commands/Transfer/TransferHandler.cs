using MiniBank.Api.Domain;
using MiniBank.Api.Domain.Events;
using MiniBank.Api.Infrastructure.EventBus;
using MiniBank.Api.Infrastructure.Repositories.Interfaces;

namespace MiniBank.Api.Features.Transactions.Commands.Transfer;

internal sealed class TransferHandler(
    IUserRepository userRepository,
    ITransactionRepository transactionRepository,
    IRabbitMQService rabbitMqService,
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
        transactionRepository.Create(transaction);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var transactionEvent = new TransactionEvent(transaction.Id);
        await rabbitMqService.PublishAsync(transactionEvent, cancellationToken);

        logger.LogInformation("Transaction published successfully: TransactionId: {TransactionId}", transaction.Id);

        return new TransferResponse(
            payer.Id,
            payee.Id,
            request.Value,
            payer.Balance,
            payee.Balance
        );
    }
}
