namespace MiniBank.Api.Features.Transactions.Queries.GetTransactionsByUserId;

internal sealed record GetTransactionsByUserIdQuery(int UserId) : IRequest<IEnumerable<GetTransactionsByUserIdResponse>>;
