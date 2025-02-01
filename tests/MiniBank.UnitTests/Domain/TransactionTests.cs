using MiniBank.Api.Domain;
using MiniBank.Api.Exceptions;

namespace MiniBank.UnitTests.Domain;

public class TransactionTests
{
    [Fact]
    public void ValidateTransaction_ShouldThrowException_WhenAmountIsInvalid()
    {
        // Arrange
        var payer = new User("Payer", "12345678901", "payer@example.com", "passwordHash", UserType.Common);
        var payee = new User("Payee", "98765432100", "payee@example.com", "passwordHash", UserType.Common);

        // Act & Assert
        AppException exception = Assert.Throws<AppException>(() => new Transaction(payer, payee, -100));
        Assert.Equal("Invalid amount.", exception.Message);
    }

    [Fact]
    public void CreateTransaction_ShouldCreateTransaction_WhenAmountIsValid()
    {
        // Arrange
        var payer = new User("Payer", "12345678901", "payer@example.com", "passwordHash", UserType.Common);
        payer.Credit(200);

        var payee = new User("Payee", "98765432100", "payee@example.com", "passwordHash", UserType.Common);

        // Act
        var transaction = new Transaction(payer, payee, 50);

        // Assert
        Assert.NotNull(transaction);
        Assert.Equal(payer.Id, transaction.PayerId);
        Assert.Equal(payee.Id, transaction.PayeeId);
        Assert.Equal(50, transaction.Value);
        Assert.Equal(TransactionStatus.Pending, transaction.Status);
    }

    [Fact]
    public void ExecuteTransaction_ShouldExecuteTransaction_WhenValid()
    {
        // Arrange
        var payer = new User("Payer", "12345678901", "payer@example.com", "passwordHash", UserType.Common);
        payer.Credit(200);
        var payee = new User("Payee", "98765432100", "payee@example.com", "passwordHash", UserType.Common);
        payee.Credit(50);

        var transaction = new Transaction(payer, payee, 50);

        // Act
        transaction.Execute();

        // Assert
        Assert.Equal(150, payer.Balance);
        Assert.Equal(100, payee.Balance);
        Assert.Equal(TransactionStatus.Completed, transaction.Status);
    }

    [Fact]
    public void ExecuteTransaction_ShouldThrowException_WhenTransactionIsNotPending()
    {
        // Arrange
        var payer = new User("Payer", "12345678901", "payer@example.com", "passwordHash", UserType.Common);
        payer.Credit(200);
        var payee = new User("Payee", "98765432100", "payee@example.com", "passwordHash", UserType.Common);
        var transaction = new Transaction(payer, payee, 50);
        transaction.Execute();

        // Act & Assert
        AppException exception = Assert.Throws<AppException>(transaction.Execute);
        Assert.Equal("Only pending transactions can be executed.", exception.Message);
    }

    [Fact]
    public void FailTransaction_ShouldFailTransaction_WhenTransactionIsPending()
    {
        // Arrange
        var payer = new User("Payer", "12345678901", "payer@example.com", "passwordHash", UserType.Common);
        payer.Credit(200);
        var payee = new User("Payee", "98765432100", "payee@example.com", "passwordHash", UserType.Common);
        var transaction = new Transaction(payer, payee, 50);

        // Act
        transaction.Fail();

        // Assert
        Assert.Equal(TransactionStatus.Failed, transaction.Status);
    }

    [Fact]
    public void FailTransaction_ShouldThrowException_WhenTransactionIsNotPending()
    {
        // Arrange
        var payer = new User("Payer", "12345678901", "payer@example.com", "passwordHash", UserType.Common);
        payer.Credit(200);
        var payee = new User("Payee", "98765432100", "payee@example.com", "passwordHash", UserType.Common);
        var transaction = new Transaction(payer, payee, 50);
        transaction.Execute();

        // Act & Assert
        AppException exception = Assert.Throws<AppException>(transaction.Fail);
        Assert.Equal("Only pending transactions can fail.", exception.Message);
    }

    [Fact]
    public void RevertTransaction_ShouldRevertTransaction_WhenTransactionIsCompleted()
    {
        // Arrange
        var payer = new User("Payer", "12345678901", "payer@example.com", "passwordHash", UserType.Common);
        payer.Credit(200);

        var payee = new User("Payee", "98765432100", "payee@example.com", "passwordHash", UserType.Common);

        var transaction = new Transaction(payer, payee, 50);
        transaction.Execute();

        // Act
        transaction.Revert();

        // Assert
        Assert.Equal(200, payer.Balance);
        Assert.Equal(0, payee.Balance);
        Assert.Equal(TransactionStatus.Reverted, transaction.Status);
    }

    [Fact]
    public void RevertTransaction_ShouldThrowException_WhenTransactionIsNotCompleted()
    {
        // Arrange
        var payer = new User("Payer", "12345678901", "payer@example.com", "passwordHash", UserType.Common);
        payer.Credit(200);
        var payee = new User("Payee", "98765432100", "payee@example.com", "passwordHash", UserType.Common);
        var transaction = new Transaction(payer, payee, 50);

        // Act & Assert
        AppException exception = Assert.Throws<AppException>(transaction.Revert);
        Assert.Equal("Only completed transactions can be reverted.", exception.Message);
    }
}
