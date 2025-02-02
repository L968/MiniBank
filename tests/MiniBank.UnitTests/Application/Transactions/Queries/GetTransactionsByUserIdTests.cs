using Microsoft.Extensions.Logging;
using MiniBank.Api.Domain;
using MiniBank.Api.Features.Transactions.Queries.GetTransactionsByUserId;
using MiniBank.Api.Infrastructure;
using MiniBank.UnitTests.Application.Fixtures;
using Moq;

namespace MiniBank.UnitTests.Application.Transactions.Queries;

public class GetTransactionsByUserIdTests : IClassFixture<AppDbContextFixture>
{
    private readonly AppDbContext _dbContext;
    private readonly Mock<ILogger<GetTransactionsByUserIdHandler>> _loggerMock;
    private readonly GetTransactionsByUserIdHandler _handler;

    public GetTransactionsByUserIdTests(AppDbContextFixture fixture)
    {
        _dbContext = fixture.DbContext;
        _loggerMock = new Mock<ILogger<GetTransactionsByUserIdHandler>>();
        _handler = new GetTransactionsByUserIdHandler(_dbContext, _loggerMock.Object);
    }

    [Fact]
    public async Task ShouldReturnTransactions_WhenTransactionsExist()
    {
        // Arrange
        var payer1 = new User("Payer1", "11111111111", "payer1@example.com", "hashedPassword", UserType.Common);
        var payee1 = new User("Payee1", "22222222222", "payee1@example.com", "hashedPassword", UserType.Common);
        var transaction1 = new Transaction(payer1, payee1, 100);

        _dbContext.Users.AddRange(payer1, payee1);
        _dbContext.Transactions.Add(transaction1);
        await _dbContext.SaveChangesAsync();

        var query = new GetTransactionsByUserIdQuery(1);

        // Act
        IEnumerable<GetTransactionsByUserIdResponse> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var resultList = result.ToList();
        Assert.Single(resultList);

        resultList = resultList.OrderBy(r => r.Id).ToList();

        Assert.Equal(transaction1.Id, resultList[0].Id);
        Assert.Equal(transaction1.PayerId, resultList[0].PayerId);
        Assert.Equal(transaction1.PayeeId, resultList[0].PayeeId);
        Assert.Equal(transaction1.Value, resultList[0].Value);
        Assert.Equal(transaction1.Timestamp, resultList[0].Timestamp);
        Assert.Equal(transaction1.Status, resultList[0].Status);
    }

    [Fact]
    public async Task ShouldReturnEmpty_WhenNoTransactionsFound()
    {
        // Arrange
        int userId = 1;

        var query = new GetTransactionsByUserIdQuery(userId);

        // Act
        IEnumerable<GetTransactionsByUserIdResponse> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
