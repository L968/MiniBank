using MiniBank.Api.Endpoints;

namespace MiniBank.Api.Features.Transactions.Commands.SendMoney;

internal sealed class SendMoneyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("transfer", async (SendMoneyCommand command, ISender sender) =>
        {
            SendMoneyResponse response = await sender.Send(command);
            return Results.Ok(response);
        })
        .WithTags(Tags.Transactions);
    }
}
