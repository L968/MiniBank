using MiniBank.Api.Domain;

namespace MiniBank.Api.Infrastructure.Repositories.Interfaces;

internal interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<User?> GetByCpfCnpjAsync(string cpfCnpj, CancellationToken cancellationToken);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    User Create(User user);
}
