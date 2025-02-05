using MiniBank.Api.Domain;
using MiniBank.Api.Infrastructure;

namespace MiniBank.Api.Features.Users.Commands.CreateUser;

internal sealed class CreateUserHandler(
    AppDbContext dbContext,
    ILogger<CreateUserHandler> logger
) : IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        bool isCpfCnpjAlreadyRegistered = await dbContext.Users
            .AnyAsync(u => u.CpfCnpj == request.CpfCnpj, cancellationToken);

        if (isCpfCnpjAlreadyRegistered)
        {
            throw new AppException($"A user with CPF/CNPJ \"{request.CpfCnpj}\" already exists");
        }

        bool isEmailAlreadyRegistered = await dbContext.Users
            .AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (isEmailAlreadyRegistered)
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

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

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
