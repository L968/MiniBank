using Microsoft.Extensions.Logging;
using MiniBank.Api.Domain;
using MiniBank.Api.Domain.Events;
using MiniBank.Api.Exceptions;
using MiniBank.Api.Features.Transactions.Commands.Transfer;
using MiniBank.Api.Infrastructure;
using MiniBank.Api.Infrastructure.EventBus;
using MiniBank.UnitTests.Application.Fixtures;
using Moq;

namespace MiniBank.UnitTests.Application.Transactions.Commands;

public class TransferTests : IClassFixture<AppDbContextFixture>
{
    private readonly AppDbContext _dbContext;
    private readonly Mock<IRabbitMQService> _rabbitMqServiceMock;
    private readonly Mock<ILogger<TransferHandler>> _loggerMock;
    private readonly TransferHandler _handler;

    public TransferTests(AppDbContextFixture fixture)
    {
        _dbContext = fixture.DbContext;
        _rabbitMqServiceMock = new Mock<IRabbitMQService>();
        _loggerMock = new Mock<ILogger<TransferHandler>>();

        _handler = new TransferHandler(
            _dbContext,
            _rabbitMqServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task ShouldTransferSuccessfully_WhenUsersExist()
    {
        // Arrange
        var payer = new User("Payer Name", "12345678901", "payer@example.com", "hashedPassword", UserType.Common);
        payer.Credit(200);

        var payee = new User("Payee Name", "98765432100", "payee@example.com", "hashedPassword", UserType.Common);
        payee.Credit(100);

        _dbContext.Users.AddRange(payer, payee);
        await _dbContext.SaveChangesAsync();

        var command = new TransferCommand(payer.Id, payee.Id, 70);

        _rabbitMqServiceMock.Setup(x => x.PublishAsync(It.IsAny<TransactionEvent>(), It.IsAny<CancellationToken>()))
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

        _rabbitMqServiceMock.Verify(x => x.PublishAsync(It.IsAny<TransactionEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShouldThrowException_WhenPayerNotFound()
    {
        // Arrange
        var payee = new User("Payee Name", "98765432100", "payee@example.com", "hashedPassword", UserType.Common);
        var command = new TransferCommand(-1, payee.Id, 50);

        _dbContext.Users.Add(payee);
        await _dbContext.SaveChangesAsync();

        // Act & Assert
        AppException exception = await Assert.ThrowsAsync<AppException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal($"Payer with ID {command.Payer} not found.", exception.Message);
    }

    [Fact]
    public async Task ShouldThrowException_WhenPayeeNotFound()
    {
        // Arrange
        var payer = new User("Payer Name", "12345678901", "payer@example.com", "hashedPassword", UserType.Common);

        _dbContext.Users.Add(payer);
        await _dbContext.SaveChangesAsync();

        var command = new TransferCommand(payer.Id, -1, 50);

        // Act & Assert
        AppException exception = await Assert.ThrowsAsync<AppException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal($"Payee with ID {command.Payee} not found.", exception.Message);
    }
}
