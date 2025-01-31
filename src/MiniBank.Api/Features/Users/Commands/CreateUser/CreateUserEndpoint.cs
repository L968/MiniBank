using MiniBank.Api.Endpoints;

namespace MiniBank.Api.Features.Users.Commands.CreateUser;

internal sealed class CreateUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("user", async (CreateUserCommand command, ISender sender) =>
        {
            CreateUserResponse response = await sender.Send(command);

            return Results.Created($"/users/{response.Id}", response);
        })
        .WithTags(Tags.Users);
    }
}
