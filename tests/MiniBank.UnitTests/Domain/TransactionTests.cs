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
        Assert.Equal(DomainErrors.Transaction.InvalidValue, exception.Message);
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
    public void ProcessTransaction_ShouldProcessTransaction_WhenValid()
    {
        // Arrange
        var payer = new User("Payer", "12345678901", "payer@example.com", "passwordHash", UserType.Common);
        payer.Credit(200);
        var payee = new User("Payee", "98765432100", "payee@example.com", "passwordHash", UserType.Common);
        payee.Credit(50);

        var transaction = new Transaction(payer, payee, 50);

        // Act
        transaction.Process();

        // Assert
        Assert.Equal(150, payer.Balance);
        Assert.Equal(100, payee.Balance);
        Assert.Equal(TransactionStatus.Completed, transaction.Status);
    }

    [Fact]
    public void ProcessTransaction_ShouldThrowException_WhenTransactionIsNotPending()
    {
        // Arrange
        var payer = new User("Payer", "12345678901", "payer@example.com", "passwordHash", UserType.Common);
        payer.Credit(200);
        var payee = new User("Payee", "98765432100", "payee@example.com", "passwordHash", UserType.Common);
        var transaction = new Transaction(payer, payee, 50);
        transaction.Process();

        // Act & Assert
        AppException exception = Assert.Throws<AppException>(transaction.Process);
        Assert.Equal(DomainErrors.Transaction.OnlyPendingTransactionsCanBeProcessed, exception.Message);
    }

    [Fact]
    public void ProcessTransaction_ShouldFail_WhenPayerHasInsufficientBalance()
    {
        // Arrange
        var payer = new User("Payer", "12345678901", "payer@example.com", "passwordHash", UserType.Common);
        payer.Credit(50);
        var payee = new User("Payee", "98765432100", "payee@example.com", "passwordHash", UserType.Common);
        payee.Credit(50);

        var transaction = new Transaction(payer, payee, 100);

        // Act
        transaction.Process();

        // Assert
        Assert.Equal(50, payer.Balance);
        Assert.Equal(50, payee.Balance);
        Assert.Equal(TransactionStatus.Failed, transaction.Status);
        Assert.Equal(DomainErrors.User.InsufficientBalance, transaction.Message);
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
        transaction.Fail("Failed transaction");

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
        transaction.Process();

        // Act & Assert
        AppException exception = Assert.Throws<AppException>(() => transaction.Fail("Failed transaction"));
        Assert.Equal(DomainErrors.Transaction.OnlyPendingTransactionsCanFail, exception.Message);
    }

    [Fact]
    public void RevertTransaction_ShouldRevertTransaction_WhenTransactionIsCompleted()
    {
        // Arrange
        var payer = new User("Payer", "12345678901", "payer@example.com", "passwordHash", UserType.Common);
        payer.Credit(200);

        var payee = new User("Payee", "98765432100", "payee@example.com", "passwordHash", UserType.Common);

        var transaction = new Transaction(payer, payee, 50);
        transaction.Process();

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
        Assert.Equal(DomainErrors.Transaction.OnlyCompletedTransactionsCanBeReverted, exception.Message);
    }
}
