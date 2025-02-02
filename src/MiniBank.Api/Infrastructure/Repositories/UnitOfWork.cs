using MiniBank.Api.Infrastructure.Repositories.Interfaces;

namespace MiniBank.Api.Infrastructure.Repositories;

internal sealed class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
