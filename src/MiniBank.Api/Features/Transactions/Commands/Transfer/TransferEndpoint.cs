using MiniBank.Api.Endpoints;

namespace MiniBank.Api.Features.Transactions.Commands.Transfer;

internal sealed class TransferEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("transfer", async (TransferCommand command, ISender sender) =>
        {
            TransferResponse response = await sender.Send(command);
            return Results.Ok(response);
        })
        .WithTags(Tags.Transactions);
    }
}
