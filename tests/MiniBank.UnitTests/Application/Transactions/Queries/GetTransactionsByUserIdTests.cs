using Microsoft.Extensions.Logging;
using MiniBank.Api.Domain;
using MiniBank.Api.Features.Transactions.Queries.GetTransactionsByUserId;
using MiniBank.Api.Infrastructure.Repositories.Interfaces;
using Moq;

namespace MiniBank.UnitTests.Application.Transactions.Queries;

public class GetTransactionsByUserIdTests
{
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly Mock<ILogger<GetTransactionsByUserIdHandler>> _loggerMock;
    private readonly GetTransactionsByUserIdHandler _handler;

    public GetTransactionsByUserIdTests()
    {
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _loggerMock = new Mock<ILogger<GetTransactionsByUserIdHandler>>();
        _handler = new GetTransactionsByUserIdHandler(_transactionRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ShouldReturnTransactions_WhenTransactionsExist()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var sender1 = new User("Sender1", "11111111111", "sender1@example.com", "hashedPassword", UserType.Common);
        var receiver1 = new User("Receiver1", "22222222222", "receiver1@example.com", "hashedPassword", UserType.Common);
        var transaction1 = new Transaction(sender1, receiver1, 100);

        var sender2 = new User("Sender2", "33333333333", "sender2@example.com", "hashedPassword", UserType.Common);
        var receiver2 = new User("Receiver2", "44444444444", "receiver2@example.com", "hashedPassword", UserType.Common);
        var transaction2 = new Transaction(sender2, receiver2, 200);

        var transactions = new List<Transaction> { transaction1, transaction2 };

        _transactionRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactions);

        var query = new GetTransactionsByUserIdQuery(userId);

        // Act
        IEnumerable<GetTransactionsByUserIdResponse> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var resultList = result.ToList();
        Assert.Equal(transactions.Count, resultList.Count);

        for (int i = 0; i < transactions.Count; i++)
        {
            Assert.Equal(transactions[i].Id, resultList[i].Id);
            Assert.Equal(transactions[i].SenderId, resultList[i].SenderId);
            Assert.Equal(transactions[i].ReceiverId, resultList[i].ReceiverId);
            Assert.Equal(transactions[i].Amount, resultList[i].Amount);
            Assert.Equal(transactions[i].Timestamp, resultList[i].Timestamp);
            Assert.Equal(transactions[i].Status, resultList[i].Status);
        }
    }

    [Fact]
    public async Task ShouldReturnEmpty_WhenNoTransactionsFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var emptyTransactions = new List<Transaction>();

        _transactionRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyTransactions);

        var query = new GetTransactionsByUserIdQuery(userId);

        // Act
        IEnumerable<GetTransactionsByUserIdResponse> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
