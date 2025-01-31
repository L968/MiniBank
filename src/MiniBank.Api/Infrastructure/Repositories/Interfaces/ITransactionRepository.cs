using MiniBank.Api.Domain;

namespace MiniBank.Api.Infrastructure.Repositories.Interfaces;

internal interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(Guid transactionId, CancellationToken cancellationToken);
    Task<IEnumerable<Transaction>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Transaction Create(Transaction transaction);
}
