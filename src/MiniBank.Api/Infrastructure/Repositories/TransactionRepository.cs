using MiniBank.Api.Domain;
using MiniBank.Api.Infrastructure.Repositories.Interfaces;

namespace MiniBank.Api.Infrastructure.Repositories;

internal class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _context;

    public TransactionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Transaction?> GetByIdAsync(Guid transactionId, CancellationToken cancellationToken)
    {
        return await _context.Transactions
            .Include(t => t.Sender)
            .Include(t => t.Receiver)
            .FirstOrDefaultAsync(t => t.Id == transactionId, cancellationToken);
    }

    // Add Pagination!
    public async Task<IEnumerable<Transaction>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Transactions
            .Where(t => t.SenderId == userId || t.ReceiverId == userId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Transaction Create(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        return transaction;
    }
}
