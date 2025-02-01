using MiniBank.Api.Domain;
using MiniBank.Api.Infrastructure.Repositories.Interfaces;

namespace MiniBank.Api.Features.Transactions.Queries.GetTransactionsByUserId;

internal sealed class GetTransactionsByUserIdHandler(
    ITransactionRepository repository,
    ILogger<GetTransactionsByUserIdHandler> logger
) : IRequestHandler<GetTransactionsByUserIdQuery, IEnumerable<GetTransactionsByUserIdResponse>>
{
    public async Task<IEnumerable<GetTransactionsByUserIdResponse>> Handle(GetTransactionsByUserIdQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Transaction> transactions = await repository.GetByUserIdAsync(request.UserId, cancellationToken);

        logger.LogInformation("Successfully retrieved transactions for user {UserId}", request.UserId);

        return transactions.Select(t => new GetTransactionsByUserIdResponse(
            t.Id,
            t.PayerId,
            t.PayeeId,
            t.Value,
            t.Timestamp,
            t.Status
        ));
    }
}
