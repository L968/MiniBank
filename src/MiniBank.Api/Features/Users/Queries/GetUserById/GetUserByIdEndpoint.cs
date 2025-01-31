using MiniBank.Api.Endpoints;

namespace MiniBank.Api.Features.Users.Queries.GetUserById;

internal sealed class GetUserByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("user/{id}", async (Guid id, ISender sender) =>
        {
            var query = new GetUserByIdQuery(id);
            GetUserByIdResponse response = await sender.Send(query);

            return response is not null
                ? Results.Ok(response)
                : Results.NotFound();
        })
        .WithTags(Tags.Users);
    }
}
