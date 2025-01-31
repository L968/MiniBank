using MiniBank.Api.Domain;

namespace MiniBank.Api.Features.Users.Queries.GetUserById;

internal sealed record GetUserByIdResponse(
    Guid Id,
    string FullName,
    string CpfCnpj,
    string Email,
    UserType Type
);
