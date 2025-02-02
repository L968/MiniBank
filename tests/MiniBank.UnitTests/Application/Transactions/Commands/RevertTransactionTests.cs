using Microsoft.Extensions.Logging;
using MiniBank.Api.Domain;
using MiniBank.Api.Exceptions;
using MiniBank.Api.Features.Transactions.Commands.RevertTransaction;
using MiniBank.Api.Infrastructure;
using MiniBank.UnitTests.Application.Fixtures;
using Moq;

namespace MiniBank.UnitTests.Application.Transactions.Commands;

public class RevertTransactionTests : IClassFixture<AppDbContextFixture>
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<RevertTransactionHandler> _logger;
    private readonly RevertTransactionHandler _handler;

    public RevertTransactionTests(AppDbContextFixture fixture)
    {
        _dbContext = fixture.DbContext;
        _logger = new Mock<ILogger<RevertTransactionHandler>>().Object;
        _handler = new RevertTransactionHandler(_dbContext, _logger);
    }

    [Fact]
    public async Task ShouldRevertTransaction_WhenTransactionExistsAndIsCompleted()
    {
        // Arrange
        var payer = new User("Payer Name", "12345678901", "payer@example.com", "hashedPassword", UserType.Common);
        var payee = new User("Payee Name", "98765432100", "payee@example.com", "hashedPassword", UserType.Common);

        payer.Credit(100);

        var transaction = new Transaction(payer, payee, 100);
        transaction.Process();

        _dbContext.Users.AddRange(payer, payee);
        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync();

        var command = new RevertTransactionCommand(transaction.Id);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Transaction? revertedTransaction = await _dbContext.Transactions.FindAsync(transaction.Id);
        Assert.Equal(TransactionStatus.Reverted, revertedTransaction!.Status);
    }

    [Fact]
    public async Task ShouldThrowException_WhenTransactionDoesNotExist()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var command = new RevertTransactionCommand(transactionId);

        // Act & Assert
        AppException exception = await Assert.ThrowsAsync<AppException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal($"Transaction with ID {transactionId} not found.", exception.Message);
    }

    [Fact]
    public async Task ShouldThrowException_WhenTransactionIsNotCompleted()
    {
        // Arrange
        var payer = new User("Payer Name", "12345678901", "payer@example.com", "hashedPassword", UserType.Common);
        var payee = new User("Payee Name", "98765432100", "payee@example.com", "hashedPassword", UserType.Common);

        var transaction = new Transaction(payer, payee, 100);

        _dbContext.Users.AddRange(payer, payee);
        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync();

        var command = new RevertTransactionCommand(transaction.Id);

        // Act & Assert
        AppException exception = await Assert.ThrowsAsync<AppException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal("Only completed transactions can be reverted.", exception.Message);
    }
}
