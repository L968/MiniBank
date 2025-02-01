using MiniBank.Api.Endpoints;

namespace MiniBank.Api.Features.Transactions.Queries.GetTransactionsByUserId;

internal sealed class GetTransactionsByUserIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("user/{id}/transactions", async (int id, ISender sender) =>
        {
            var query = new GetTransactionsByUserIdQuery(id);
            IEnumerable<GetTransactionsByUserIdResponse> response = await sender.Send(query);

            return Results.Ok(response);
        })
        .WithTags(Tags.Transactions);
    }
}
