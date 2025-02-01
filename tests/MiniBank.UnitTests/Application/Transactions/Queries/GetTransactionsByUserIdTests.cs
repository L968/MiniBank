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
        int userId = 1;

        var payer1 = new User("Payer1", "11111111111", "payer1@example.com", "hashedPassword", UserType.Common);
        var payee1 = new User("Payee1", "22222222222", "payee1@example.com", "hashedPassword", UserType.Common);
        var transaction1 = new Transaction(payer1, payee1, 100);

        var payer2 = new User("Payer2", "33333333333", "payer2@example.com", "hashedPassword", UserType.Common);
        var payee2 = new User("Payee2", "44444444444", "payee2@example.com", "hashedPassword", UserType.Common);
        var transaction2 = new Transaction(payer2, payee2, 200);

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
            Assert.Equal(transactions[i].PayerId, resultList[i].PayerId);
            Assert.Equal(transactions[i].PayeeId, resultList[i].PayeeId);
            Assert.Equal(transactions[i].Value, resultList[i].Value);
            Assert.Equal(transactions[i].Timestamp, resultList[i].Timestamp);
            Assert.Equal(transactions[i].Status, resultList[i].Status);
        }
    }

    [Fact]
    public async Task ShouldReturnEmpty_WhenNoTransactionsFound()
    {
        // Arrange
        int userId = 1;
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
