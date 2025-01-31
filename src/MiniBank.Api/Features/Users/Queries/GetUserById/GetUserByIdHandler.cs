using MiniBank.Api.Domain;
using MiniBank.Api.Infrastructure.Repositories.Interfaces;

namespace MiniBank.Api.Features.Users.Queries.GetUserById;

internal sealed class GetUserByIdHandler(
    IUserRepository repository,
    ILogger<GetUserByIdHandler> logger
) : IRequestHandler<GetUserByIdQuery, GetUserByIdResponse>
{
    public async Task<GetUserByIdResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        User user = await repository.GetByIdAsync(request.Id, cancellationToken);

        if (user is null)
        {
            throw new AppException($"User with ID {request.Id} not found");
        }

        logger.LogInformation("Successfully retrieved {@User}", user);

        return new GetUserByIdResponse(
            user.Id,
            user.FullName,
            user.CpfCnpj,
            user.Email,
            user.Type
        );
    }
}
