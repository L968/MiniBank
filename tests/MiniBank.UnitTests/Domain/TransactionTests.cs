using MiniBank.Api.Domain;
using MiniBank.Api.Exceptions;

namespace MiniBank.UnitTests.Domain;

public class TransactionTests
{
    [Fact]
    public void ValidateTransaction_ShouldThrowException_WhenAmountIsInvalid()
    {
        // Arrange
        var sender = new User("Sender", "12345678901", "sender@example.com", "passwordHash", UserType.Common);
        var receiver = new User("Receiver", "98765432100", "receiver@example.com", "passwordHash", UserType.Common);

        // Act & Assert
        AppException exception = Assert.Throws<AppException>(() => new Transaction(sender, receiver, -100));
        Assert.Equal("Invalid amount.", exception.Message);
    }

    [Fact]
    public void CreateTransaction_ShouldCreateTransaction_WhenAmountIsValid()
    {
        // Arrange
        var sender = new User("Sender", "12345678901", "sender@example.com", "passwordHash", UserType.Common);
        sender.Credit(200);

        var receiver = new User("Receiver", "98765432100", "receiver@example.com", "passwordHash", UserType.Common);

        // Act
        var transaction = new Transaction(sender, receiver, 50);

        // Assert
        Assert.NotNull(transaction);
        Assert.Equal(sender.Id, transaction.SenderId);
        Assert.Equal(receiver.Id, transaction.ReceiverId);
        Assert.Equal(50, transaction.Amount);
        Assert.Equal(TransactionStatus.Pending, transaction.Status);
    }

    [Fact]
    public void ExecuteTransaction_ShouldExecuteTransaction_WhenValid()
    {
        // Arrange
        var sender = new User("Sender", "12345678901", "sender@example.com", "passwordHash", UserType.Common);
        sender.Credit(200);
        var receiver = new User("Receiver", "98765432100", "receiver@example.com", "passwordHash", UserType.Common);
        receiver.Credit(50);

        var transaction = new Transaction(sender, receiver, 50);

        // Act
        transaction.Execute();

        // Assert
        Assert.Equal(150, sender.Balance);
        Assert.Equal(100, receiver.Balance);
        Assert.Equal(TransactionStatus.Completed, transaction.Status);
    }

    [Fact]
    public void ExecuteTransaction_ShouldThrowException_WhenTransactionIsNotPending()
    {
        // Arrange
        var sender = new User("Sender", "12345678901", "sender@example.com", "passwordHash", UserType.Common);
        sender.Credit(200);
        var receiver = new User("Receiver", "98765432100", "receiver@example.com", "passwordHash", UserType.Common);
        var transaction = new Transaction(sender, receiver, 50);
        transaction.Execute();

        // Act & Assert
        AppException exception = Assert.Throws<AppException>(transaction.Execute);
        Assert.Equal("Only pending transactions can be executed.", exception.Message);
    }

    [Fact]
    public void FailTransaction_ShouldFailTransaction_WhenTransactionIsPending()
    {
        // Arrange
        var sender = new User("Sender", "12345678901", "sender@example.com", "passwordHash", UserType.Common);
        sender.Credit(200);
        var receiver = new User("Receiver", "98765432100", "receiver@example.com", "passwordHash", UserType.Common);
        var transaction = new Transaction(sender, receiver, 50);

        // Act
        transaction.Fail();

        // Assert
        Assert.Equal(TransactionStatus.Failed, transaction.Status);
    }

    [Fact]
    public void FailTransaction_ShouldThrowException_WhenTransactionIsNotPending()
    {
        // Arrange
        var sender = new User("Sender", "12345678901", "sender@example.com", "passwordHash", UserType.Common);
        sender.Credit(200);
        var receiver = new User("Receiver", "98765432100", "receiver@example.com", "passwordHash", UserType.Common);
        var transaction = new Transaction(sender, receiver, 50);
        transaction.Execute();

        // Act & Assert
        AppException exception = Assert.Throws<AppException>(transaction.Fail);
        Assert.Equal("Only pending transactions can fail.", exception.Message);
    }

    [Fact]
    public void RevertTransaction_ShouldRevertTransaction_WhenTransactionIsCompleted()
    {
        // Arrange
        var sender = new User("Sender", "12345678901", "sender@example.com", "passwordHash", UserType.Common);
        sender.Credit(200);

        var receiver = new User("Receiver", "98765432100", "receiver@example.com", "passwordHash", UserType.Common);

        var transaction = new Transaction(sender, receiver, 50);
        transaction.Execute();

        // Act
        transaction.Revert();

        // Assert
        Assert.Equal(200, sender.Balance);
        Assert.Equal(0, receiver.Balance);
        Assert.Equal(TransactionStatus.Reverted, transaction.Status);
    }

    [Fact]
    public void RevertTransaction_ShouldThrowException_WhenTransactionIsNotCompleted()
    {
        // Arrange
        var sender = new User("Sender", "12345678901", "sender@example.com", "passwordHash", UserType.Common);
        sender.Credit(200);
        var receiver = new User("Receiver", "98765432100", "receiver@example.com", "passwordHash", UserType.Common);
        var transaction = new Transaction(sender, receiver, 50);

        // Act & Assert
        AppException exception = Assert.Throws<AppException>(transaction.Revert);
        Assert.Equal("Only completed transactions can be reverted.", exception.Message);
    }
}
