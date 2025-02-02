using MiniBank.Api.Domain;

namespace MiniBank.Api.Features.Users.Queries.GetUserById;

internal sealed record GetUserByIdResponse(
    int Id,
    string FullName,
    string CpfCnpj,
    string Email,
    decimal Balance,
    UserType Type
);
