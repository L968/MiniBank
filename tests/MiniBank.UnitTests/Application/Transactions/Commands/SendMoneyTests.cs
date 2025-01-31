using Microsoft.Extensions.Logging;
using MiniBank.Api.Domain;
using MiniBank.Api.Exceptions;
using MiniBank.Api.Features.Transactions.Commands.SendMoney;
using MiniBank.Api.Infrastructure.Repositories.Interfaces;
using MiniBank.Api.Infrastructure.Services;
using Moq;

namespace MiniBank.UnitTests.Application.Transactions.Commands;

public class SendMoneyTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly Mock<IAuthorizationService> _authorizationServiceMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<SendMoneyHandler>> _loggerMock;
    private readonly SendMoneyHandler _handler;

    public SendMoneyTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _notificationServiceMock = new Mock<INotificationService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<SendMoneyHandler>>();

        _handler = new SendMoneyHandler(
            _userRepositoryMock.Object,
            _transactionRepositoryMock.Object,
            _authorizationServiceMock.Object,
            _notificationServiceMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task ShouldSendMoney_WhenTransactionIsAuthorized()
    {
        // Arrange
        var sender = new User("Sender Name", "12345678901", "sender@example.com", "hashedPassword", UserType.Common);
        sender.Credit(200);
        var receiver = new User("Receiver Name", "98765432100", "receiver@example.com", "hashedPassword", UserType.Common);
        receiver.Credit(100);

        var command = new SendMoneyCommand(sender.Id, receiver.Id, 70);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(sender.Id, It.IsAny<CancellationToken>())).ReturnsAsync(sender);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(receiver.Id, It.IsAny<CancellationToken>())).ReturnsAsync(receiver);
        _authorizationServiceMock.Setup(x => x.IsTransactionAuthorizedAsync()).ReturnsAsync(true);

        var transaction = new Transaction(sender, receiver, command.Amount);
        _transactionRepositoryMock.Setup(x => x.Create(It.IsAny<Transaction>())).Returns(transaction);

        // Act
        SendMoneyResponse result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(sender.Id, result.SenderId);
        Assert.Equal(receiver.Id, result.ReceiverId);
        Assert.Equal(70m, result.Amount);
        Assert.Equal(130m, sender.Balance);
        Assert.Equal(170m, receiver.Balance);

        _transactionRepositoryMock.Verify(x => x.Create(It.IsAny<Transaction>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _notificationServiceMock.Verify(x => x.Notify(), Times.Once);
    }

    [Fact]
    public async Task ShouldFailTransaction_WhenTransactionIsNotAuthorized()
    {
        // Arrange
        var sender = new User("Sender Name", "12345678901", "sender@example.com", "hashedPassword", UserType.Common);
        sender.Credit(200);
        var receiver = new User("Receiver Name", "98765432100", "receiver@example.com", "hashedPassword", UserType.Common);
        receiver.Credit(100);

        var command = new SendMoneyCommand(sender.Id, receiver.Id, 50);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(sender.Id, It.IsAny<CancellationToken>())).ReturnsAsync(sender);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(receiver.Id, It.IsAny<CancellationToken>())).ReturnsAsync(receiver);
        _authorizationServiceMock.Setup(x => x.IsTransactionAuthorizedAsync()).ReturnsAsync(false);

        var transaction = new Transaction(sender, receiver, command.Amount);
        _transactionRepositoryMock.Setup(x => x.Create(It.IsAny<Transaction>())).Returns(transaction);

        // Act
        SendMoneyResponse result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(sender.Id, result.SenderId);
        Assert.Equal(receiver.Id, result.ReceiverId);
        Assert.Equal(50m, result.Amount);
    }

    [Fact]
    public async Task ShouldThrowException_WhenSenderNotFound()
    {
        // Arrange
        var receiver = new User("Receiver Name", "98765432100", "receiver@example.com", "hashedPassword", UserType.Common);
        var command = new SendMoneyCommand(Guid.Empty, receiver.Id, 50);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(Guid.Empty, It.IsAny<CancellationToken>())).ReturnsAsync((User)null);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(receiver.Id, It.IsAny<CancellationToken>())).ReturnsAsync(receiver);

        // Act & Assert
        AppException exception = await Assert.ThrowsAsync<AppException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal($"Sender with ID {command.SenderId} not found.", exception.Message);
    }

    [Fact]
    public async Task ShouldThrowException_WhenReceiverNotFound()
    {
        // Arrange
        var sender = new User("Sender Name", "12345678901", "sender@example.com", "hashedPassword", UserType.Common);
        var command = new SendMoneyCommand(sender.Id, Guid.Empty, 50);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(sender.Id, It.IsAny<CancellationToken>())).ReturnsAsync(sender);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(Guid.Empty, It.IsAny<CancellationToken>())).ReturnsAsync((User)null);

        // Act & Assert
        AppException exception = await Assert.ThrowsAsync<AppException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal($"Receiver with ID {command.ReceiverId} not found.", exception.Message);
    }

}
