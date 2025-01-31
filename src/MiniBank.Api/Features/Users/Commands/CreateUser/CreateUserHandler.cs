using MiniBank.Api.Domain;
using MiniBank.Api.Infrastructure.Repositories.Interfaces;

namespace MiniBank.Api.Features.Users.Commands.CreateUser;

internal sealed class CreateUserHandler(
    IUserRepository repository,
    IUnitOfWork unitOfWork,
    ILogger<CreateUserHandler> logger
) : IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        User existingUserByCpfCnpj = await repository.GetByCpfCnpjAsync(request.CpfCnpj, cancellationToken);

        if (existingUserByCpfCnpj is not null)
        {
            throw new AppException($"A user with CPF/CNPJ \"{request.CpfCnpj}\" already exists");
        }

        User existingUserByEmail = await repository.GetByEmailAsync(request.Email, cancellationToken);

        if (existingUserByEmail is not null)
        {
            throw new AppException($"A user with email \"{request.Email}\" already exists");
        }

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User(
            request.FullName,
            request.CpfCnpj,
            request.Email,
            hashedPassword,
            request.Type
        );

        repository.Create(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Successfully created {@User}", user);

        return new CreateUserResponse(
            user.Id,
            user.FullName,
            user.CpfCnpj,
            user.Email,
            user.Type
        );
    }
}
