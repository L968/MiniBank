using MiniBank.Api.Domain;

namespace MiniBank.Api.Features.Users.Commands.CreateUser;

internal sealed record CreateUserResponse(
    Guid Id,
    string FullName,
    string CpfCnpj,
    string Email,
    UserType Type
);
