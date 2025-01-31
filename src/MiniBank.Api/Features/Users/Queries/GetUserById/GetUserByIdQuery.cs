namespace MiniBank.Api.Features.Users.Queries.GetUserById;

internal sealed record GetUserByIdQuery(
    Guid Id
) : IRequest<GetUserByIdResponse>;
