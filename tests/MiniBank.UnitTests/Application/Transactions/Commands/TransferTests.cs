using Microsoft.Extensions.Logging;
using MiniBank.Api.Domain;
using MiniBank.Api.Exceptions;
using MiniBank.Api.Features.Transactions.Commands.Transfer;
using MiniBank.Api.Infrastructure.Repositories.Interfaces;
using MiniBank.Api.Infrastructure.Services;
using Moq;

namespace MiniBank.UnitTests.Application.Transactions.Commands;

public class TransferTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly Mock<IAuthorizationService> _authorizationServiceMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<TransferHandler>> _loggerMock;
    private readonly TransferHandler _handler;

    public TransferTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _notificationServiceMock = new Mock<INotificationService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<TransferHandler>>();

        _handler = new TransferHandler(
            _userRepositoryMock.Object,
            _transactionRepositoryMock.Object,
            _authorizationServiceMock.Object,
            _notificationServiceMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task ShouldTransfer_WhenTransactionIsAuthorized()
    {
        // Arrange
        var payer = new User("Payer Name", "12345678901", "payer@example.com", "hashedPassword", UserType.Common);
        payer.Credit(200);
        var payee = new User("Payee Name", "98765432100", "payee@example.com", "hashedPassword", UserType.Common);
        payee.Credit(100);

        var command = new TransferCommand(payer.Id, payee.Id, 70);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(payer.Id, It.IsAny<CancellationToken>())).ReturnsAsync(payer);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(payee.Id, It.IsAny<CancellationToken>())).ReturnsAsync(payee);
        _authorizationServiceMock.Setup(x => x.IsTransactionAuthorizedAsync()).ReturnsAsync(true);

        var transaction = new Transaction(payer, payee, command.Value);
        _transactionRepositoryMock.Setup(x => x.Create(It.IsAny<Transaction>())).Returns(transaction);

        // Act
        TransferResponse result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(payer.Id, result.PayerId);
        Assert.Equal(payee.Id, result.PayeeId);
        Assert.Equal(70m, result.Amount);
        Assert.Equal(130m, payer.Balance);
        Assert.Equal(170m, payee.Balance);

        _transactionRepositoryMock.Verify(x => x.Create(It.IsAny<Transaction>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _notificationServiceMock.Verify(x => x.Notify(), Times.Once);
    }

    [Fact]
    public async Task ShouldFailTransaction_WhenTransactionIsNotAuthorized()
    {
        // Arrange
        var payer = new User("Payer Name", "12345678901", "payer@example.com", "hashedPassword", UserType.Common);
        payer.Credit(200);
        var payee = new User("Payee Name", "98765432100", "payee@example.com", "hashedPassword", UserType.Common);
        payee.Credit(100);

        var command = new TransferCommand(payer.Id, payee.Id, 50);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(payer.Id, It.IsAny<CancellationToken>())).ReturnsAsync(payer);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(payee.Id, It.IsAny<CancellationToken>())).ReturnsAsync(payee);
        _authorizationServiceMock.Setup(x => x.IsTransactionAuthorizedAsync()).ReturnsAsync(false);

        var transaction = new Transaction(payer, payee, command.Value);
        _transactionRepositoryMock.Setup(x => x.Create(It.IsAny<Transaction>())).Returns(transaction);

        // Act
        TransferResponse result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(payer.Id, result.PayerId);
        Assert.Equal(payee.Id, result.PayeeId);
        Assert.Equal(50m, result.Amount);
    }

    [Fact]
    public async Task ShouldThrowException_WhenPayerNotFound()
    {
        // Arrange
        var payee = new User("Payee Name", "98765432100", "payee@example.com", "hashedPassword", UserType.Common);
        var command = new TransferCommand(0, payee.Id, 50);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(0, It.IsAny<CancellationToken>())).ReturnsAsync((User)null);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(payee.Id, It.IsAny<CancellationToken>())).ReturnsAsync(payee);

        // Act & Assert
        AppException exception = await Assert.ThrowsAsync<AppException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal($"Payer with ID {command.Payer} not found.", exception.Message);
    }

    [Fact]
    public async Task ShouldThrowException_WhenPayeeNotFound()
    {
        // Arrange
        var payer = new User("Payer Name", "12345678901", "payer@example.com", "hashedPassword", UserType.Common);
        var command = new TransferCommand(payer.Id, 0, 50);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(payer.Id, It.IsAny<CancellationToken>())).ReturnsAsync(payer);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(0, It.IsAny<CancellationToken>())).ReturnsAsync((User)null);

        // Act & Assert
        AppException exception = await Assert.ThrowsAsync<AppException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal($"Payee with ID {command.Payee} not found.", exception.Message);
    }

}
