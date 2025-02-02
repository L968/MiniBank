using Microsoft.Extensions.Logging;
using MiniBank.Api.Domain;
using MiniBank.Api.Exceptions;
using MiniBank.Api.Features.Users.Queries.GetUserById;
using MiniBank.Api.Infrastructure;
using MiniBank.UnitTests.Application.Fixtures;
using Moq;

namespace MiniBank.UnitTests.Application.Users.Queries;

public class GetUserByIdTests : IClassFixture<AppDbContextFixture>
{
    private readonly AppDbContext _dbContext;
    private readonly Mock<ILogger<GetUserByIdHandler>> _loggerMock;
    private readonly GetUserByIdHandler _handler;

    public GetUserByIdTests(AppDbContextFixture fixture)
    {
        _dbContext = fixture.DbContext;
        _loggerMock = new Mock<ILogger<GetUserByIdHandler>>();
        _handler = new GetUserByIdHandler(_dbContext, _loggerMock.Object);
    }

    [Fact]
    public async Task ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        int userId = 1;
        var user = new User("John Doe", "12345678900", "john@example.com", "hashedPassword", UserType.Common);
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var query = new GetUserByIdQuery(userId);

        // Act
        GetUserByIdResponse result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.FullName, result.FullName);
    }

    [Fact]
    public async Task ShouldThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        int userId = 1;

        var query = new GetUserByIdQuery(userId);

        // Act & Assert
        AppException exception = await Assert.ThrowsAsync<AppException>(() => _handler.Handle(query, CancellationToken.None));
        Assert.Equal($"User with ID {userId} not found", exception.Message);
    }
}
