﻿using Microsoft.Extensions.Logging;
using MiniBank.Api.Domain;
using MiniBank.Api.Exceptions;
using MiniBank.Api.Features.Transactions.Commands.RevertTransaction;
using MiniBank.Api.Infrastructure.Repositories.Interfaces;
using Moq;

namespace MiniBank.UnitTests.Application.Transactions.Commands;

public class RevertTransactionTests
{
    private readonly Mock<ITransactionRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<RevertTransactionHandler>> _loggerMock;
    private readonly RevertTransactionHandler _handler;

    public RevertTransactionTests()
    {
        _repositoryMock = new Mock<ITransactionRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<RevertTransactionHandler>>();
        _handler = new RevertTransactionHandler(_repositoryMock.Object, _unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ShouldRevertTransaction_WhenTransactionExistsAndIsCompleted()
    {
        // Arrange
        var sender = new User("Sender Name", "12345678901", "sender@example.com", "hashedPassword", UserType.Common);
        sender.Credit(100);

        var receiver = new User("Receiver Name", "98765432100", "receiver@example.com", "hashedPassword", UserType.Common);

        var transaction = new Transaction(sender, receiver, 100);
        transaction.Execute();

        _repositoryMock.Setup(x => x.GetByIdAsync(transaction.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction);

        var command = new RevertTransactionCommand(transaction.Id);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(TransactionStatus.Reverted, transaction.Status);
    }

    [Fact]
    public async Task ShouldThrowException_WhenTransactionDoesNotExist()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        _repositoryMock.Setup(x => x.GetByIdAsync(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Transaction)null);

        var command = new RevertTransactionCommand(transactionId);

        // Act & Assert
        AppException exception = await Assert.ThrowsAsync<AppException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal($"Transaction with ID {transactionId} not found.", exception.Message);
    }

    [Fact]
    public async Task ShouldThrowException_WhenTransactionIsNotCompleted()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var sender = new User("Sender Name", "12345678901", "sender@example.com", "hashedPassword", UserType.Common);
        var receiver = new User("Receiver Name", "98765432100", "receiver@example.com", "hashedPassword", UserType.Common);

        var transaction = new Transaction(sender, receiver, 100);
        _repositoryMock.Setup(x => x.GetByIdAsync(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction);

        var command = new RevertTransactionCommand(transactionId);

        // Act & Assert
        AppException exception = await Assert.ThrowsAsync<AppException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal("Only completed transactions can be reverted.", exception.Message);
    }
}
