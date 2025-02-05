using MiniBank.Api.Domain;
using MiniBank.Api.Domain.Events;
using MiniBank.Api.Infrastructure;
using MiniBank.Api.Infrastructure.EventBus;

namespace MiniBank.Api.Features.Transactions.Commands.Transfer;

internal sealed class TransferHandler(
    AppDbContext dbContext,
    IRabbitMQService rabbitMqService,
    ILogger<TransferHandler> logger
) : IRequestHandler<TransferCommand, TransferResponse>
{
    public async Task<TransferResponse> Handle(TransferCommand request, CancellationToken cancellationToken)
    {
        User? payer = await dbContext.Users.FindAsync([request.Payer], cancellationToken);
        if (payer is null)
        {
            throw new AppException($"Payer with ID {request.Payer} not found.");
        }

        User? payee = await dbContext.Users.FindAsync([request.Payee], cancellationToken);
        if (payee is null)
        {
            throw new AppException($"Payee with ID {request.Payee} not found.");
        }

        var transaction = new Transaction(payer, payee, request.Value);
        dbContext.Transactions.Add(transaction);
        await dbContext.SaveChangesAsync(cancellationToken);

        var transactionEvent = new TransactionEvent(transaction.Id);
        await rabbitMqService.PublishAsync(transactionEvent, CancellationToken.None);

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
