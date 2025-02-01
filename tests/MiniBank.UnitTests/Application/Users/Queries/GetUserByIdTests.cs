using Microsoft.Extensions.Logging;
using Moq;
using MiniBank.Api.Domain;
using MiniBank.Api.Features.Users.Queries.GetUserById;
using MiniBank.Api.Infrastructure.Repositories.Interfaces;
using MiniBank.Api.Exceptions;

namespace MiniBank.UnitTests.Application.Users.Queries;

public class GetUserByIdTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly Mock<ILogger<GetUserByIdHandler>> _loggerMock;
    private readonly GetUserByIdHandler _handler;

    public GetUserByIdTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<GetUserByIdHandler>>();

        _handler = new GetUserByIdHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        int userId = 1;
        var user = new User("John Doe", "12345678900", "john@example.com", "hashedPassword", UserType.Common);

        _repositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var query = new GetUserByIdQuery(userId);

        // Act
        GetUserByIdResponse result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.FullName, result.FullName);
        _repositoryMock.Verify(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShouldThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        int userId = 1;
        _repositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);

        var query = new GetUserByIdQuery(userId);

        // Act & Assert
        AppException exception = await Assert.ThrowsAsync<AppException>(() => _handler.Handle(query, CancellationToken.None));
        Assert.Equal($"User with ID {userId} not found", exception.Message);
    }
}
