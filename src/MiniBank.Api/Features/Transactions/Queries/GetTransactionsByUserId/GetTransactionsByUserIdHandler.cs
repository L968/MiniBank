using MiniBank.Api.Domain;
using MiniBank.Api.Infrastructure;

namespace MiniBank.Api.Features.Transactions.Queries.GetTransactionsByUserId;

internal sealed class GetTransactionsByUserIdHandler(
    AppDbContext dbContext,
    ILogger<GetTransactionsByUserIdHandler> logger
) : IRequestHandler<GetTransactionsByUserIdQuery, IEnumerable<GetTransactionsByUserIdResponse>>
{
    public async Task<IEnumerable<GetTransactionsByUserIdResponse>> Handle(GetTransactionsByUserIdQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Transaction> transactions = await dbContext.Transactions
            .AsNoTracking()
            .Where(t => t.PayerId == request.UserId || t.PayeeId == request.UserId)
            .ToListAsync(cancellationToken);

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
