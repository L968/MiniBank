using MiniBank.Api.Domain;
using MiniBank.Api.Exceptions;

namespace MiniBank.UnitTests.Domain;

public class UserTests
{
    [Fact]
    public void ValidateCanTransfer_ShouldThrowException_WhenUserIsNotCommon()
    {
        // Arrange
        var user = new User("Merchant User", "12345678901", "merchant@example.com", "hashedPassword", UserType.Merchant);

        // Act & Assert
        AppException exception = Assert.Throws<AppException>(() => user.ValidateCanTransfer(100));
        Assert.Equal("Only Common users are allowed to send money.", exception.Message);
    }

    [Fact]
    public void ValidateCanTransfer_ShouldThrowException_WhenBalanceIsInsufficient()
    {
        // Arrange
        var user = new User("Common User", "98765432100", "user@example.com", "hashedPassword", UserType.Common);
        user.Credit(50);

        // Act & Assert
        AppException exception = Assert.Throws<AppException>(() => user.ValidateCanTransfer(100));
        Assert.Equal("Insufficient balance. Current balance: R$ 50,00.", exception.Message);
    }

    [Fact]
    public void ValidateCanTransfer_ShouldAllowToTransfer_WhenUserIsCommonAndHasEnoughBalance()
    {
        // Arrange
        var user = new User("Common User", "98765432100", "user@example.com", "hashedPassword", UserType.Common);
        user.Credit(100);

        // Act
        user.ValidateCanTransfer(50);
    }

    [Fact]
    public void Debit_ShouldThrowException_WhenAmountIsInvalidForDebit()
    {
        // Arrange
        var user = new User("Common User", "98765432100", "user@example.com", "hashedPassword", UserType.Common);
        user.Credit(100);

        // Act & Assert
        AppException exception = Assert.Throws<AppException>(() => user.Debit(-50));
        Assert.Equal("Invalid amount.", exception.Message);
    }

    [Fact]
    public void Debit_ShouldThrowException_WhenBalanceIsInsufficientForDebit()
    {
        // Arrange
        var user = new User("Common User", "98765432100", "user@example.com", "hashedPassword", UserType.Common);
        user.Credit(50);

        // Act & Assert
        AppException exception = Assert.Throws<AppException>(() => user.Debit(100));
        Assert.Equal("Insufficient balance.", exception.Message);
    }

    [Fact]
    public void Debit_ShouldDebitAmount_WhenValidAmount()
    {
        // Arrange
        var user = new User("Common User", "98765432100", "user@example.com", "hashedPassword", UserType.Common);
        user.Credit(100);

        // Act
        user.Debit(50);

        // Assert
        Assert.Equal(50, user.Balance);
    }

    [Fact]
    public void Credit_ShouldThrowException_WhenAmountIsInvalidForCredit()
    {
        // Arrange
        var user = new User("Common User", "98765432100", "user@example.com", "hashedPassword", UserType.Common);

        // Act & Assert
        AppException exception = Assert.Throws<AppException>(() => user.Credit(-50));
        Assert.Equal("Invalid amount.", exception.Message);
    }

    [Fact]
    public void Credit_ShouldCreditAmount_WhenValidAmount()
    {
        // Arrange
        var user = new User("Common User", "98765432100", "user@example.com", "hashedPassword", UserType.Common);

        // Act
        user.Credit(50);

        // Assert
        Assert.Equal(50, user.Balance);
    }
}
