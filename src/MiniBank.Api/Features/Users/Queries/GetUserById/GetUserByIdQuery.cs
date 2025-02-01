namespace MiniBank.Api.Features.Users.Queries.GetUserById;

internal sealed record GetUserByIdQuery(
    int Id
) : IRequest<GetUserByIdResponse>;
