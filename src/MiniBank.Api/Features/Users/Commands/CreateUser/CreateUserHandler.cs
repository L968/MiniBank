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
        User existingUserByCpfCnpj = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.CpfCnpj == request.CpfCnpj, cancellationToken);

        if (existingUserByCpfCnpj is not null)
        {
            throw new AppException($"A user with CPF/CNPJ \"{request.CpfCnpj}\" already exists");
        }

        User existingUserByEmail = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

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
