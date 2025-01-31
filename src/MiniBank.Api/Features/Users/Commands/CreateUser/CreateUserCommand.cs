using MiniBank.Api.Domain;

namespace MiniBank.Api.Features.Users.Commands.CreateUser;

internal sealed record CreateUserCommand(
    string FullName,
    string CpfCnpj,
    string Email,
    string Password,
    UserType Type
) : IRequest<CreateUserResponse>;
