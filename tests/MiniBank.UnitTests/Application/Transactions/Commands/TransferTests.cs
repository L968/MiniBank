using Microsoft.Extensions.Logging;
using MiniBank.Api.Domain;
using MiniBank.Api.Domain.Events;
using MiniBank.Api.Exceptions;
using MiniBank.Api.Features.Transactions.Commands.Transfer;
using MiniBank.Api.Infrastructure.EventBus;
using MiniBank.Api.Infrastructure.Repositories.Interfaces;
using MiniBank.Api.Infrastructure.Services;
using Moq;

namespace MiniBank.UnitTests.Application.Transactions.Commands;

public class TransferTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly Mock<IRabbitMQService> _rabbitMqServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<TransferHandler>> _loggerMock;
    private readonly TransferHandler _handler;

    public TransferTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _rabbitMqServiceMock = new Mock<IRabbitMQService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<TransferHandler>>();

        _handler = new TransferHandler(
            _userRepositoryMock.Object,
            _transactionRepositoryMock.Object,
            _rabbitMqServiceMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task ShouldTransferSuccessfully_WhenUsersExist()
    {
        // Arrange
        var payer = new User("Payer Name", "12345678901", "payer@example.com", "hashedPassword", UserType.Common);
        typeof(User).GetProperty("Id")!.SetValue(payer, 1);
        payer.Credit(200);

        var payee = new User("Payee Name", "98765432100", "payee@example.com", "hashedPassword", UserType.Common);
        typeof(User).GetProperty("Id")!.SetValue(payee, 2);
        payee.Credit(100);

        var command = new TransferCommand(payer.Id, payee.Id, 70);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(payer.Id, It.IsAny<CancellationToken>())).ReturnsAsync(payer);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(payee.Id, It.IsAny<CancellationToken>())).ReturnsAsync(payee);

        Transaction? createdTransaction = null;
        _transactionRepositoryMock
            .Setup(x => x.Create(It.IsAny<Transaction>()))
            .Callback<Transaction>(t => createdTransaction = t)
            .Returns(() => createdTransaction);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _rabbitMqServiceMock
            .Setup(x => x.PublishAsync(It.IsAny<TransactionEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        TransferResponse transferResponse = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(transferResponse);
        Assert.Equal(payer.Id, transferResponse.PayerId);
        Assert.Equal(payee.Id, transferResponse.PayeeId);
        Assert.Equal(70m, transferResponse.Amount);

        Assert.Equal(200m, transferResponse.PayerBalanceAfter);
        Assert.Equal(100m, transferResponse.PayeeBalanceAfter);

        _transactionRepositoryMock.Verify(x => x.Create(It.IsAny<Transaction>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _rabbitMqServiceMock.Verify(x => x.PublishAsync(It.IsAny<TransactionEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShouldThrowException_WhenPayerNotFound()
    {
        // Arrange
        var payee = new User("Payee Name", "98765432100", "payee@example.com", "hashedPassword", UserType.Common);
        var command = new TransferCommand(-1, payee.Id, 50);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(-1, It.IsAny<CancellationToken>())).ReturnsAsync((User)null);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(payee.Id, It.IsAny<CancellationToken>())) .ReturnsAsync(payee);

        // Act & Assert
        AppException exception = await Assert.ThrowsAsync<AppException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal($"Payer with ID {command.Payer} not found.", exception.Message);
    }

    [Fact]
    public async Task ShouldThrowException_WhenPayeeNotFound()
    {
        // Arrange
        var payer = new User("Payer Name", "12345678901", "payer@example.com", "hashedPassword", UserType.Common);
        var command = new TransferCommand(payer.Id, -1, 50);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(payer.Id, It.IsAny<CancellationToken>())).ReturnsAsync(payer);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(-1, It.IsAny<CancellationToken>())).ReturnsAsync((User)null);

        // Act & Assert
        AppException exception = await Assert.ThrowsAsync<AppException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal($"Payee with ID {command.Payee} not found.", exception.Message);
    }
}
