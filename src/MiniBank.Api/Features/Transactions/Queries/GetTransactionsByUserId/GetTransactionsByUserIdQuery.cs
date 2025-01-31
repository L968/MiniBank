namespace MiniBank.Api.Features.Transactions.Queries.GetTransactionsByUserId;

internal sealed record GetTransactionsByUserIdQuery(Guid UserId) : IRequest<IEnumerable<GetTransactionsByUserIdResponse>>;
