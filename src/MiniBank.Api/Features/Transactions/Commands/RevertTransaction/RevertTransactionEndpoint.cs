using MiniBank.Api.Endpoints;

namespace MiniBank.Api.Features.Transactions.Commands.RevertTransaction;

internal sealed class RevertTransactionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("{transactionId}/revert", async (Guid transactionId, ISender sender) =>
        {
            var command = new RevertTransactionCommand(transactionId);
            await sender.Send(command);
            return Results.NoContent();
        })
        .WithTags(Tags.Transactions);
    }
}
