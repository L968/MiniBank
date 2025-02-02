using Microsoft.Extensions.Logging;
using Moq;
using MiniBank.Api.Domain;
using MiniBank.Api.Features.Users.Commands.CreateUser;
using MiniBank.Api.Infrastructure;
using MiniBank.Api.Exceptions;
using MiniBank.UnitTests.Application.Fixtures;

namespace MiniBank.UnitTests.Application.Users.Commands;

public class CreateUserTests : IClassFixture<AppDbContextFixture>
{
    private readonly AppDbContext _dbContext;
    private readonly Mock<ILogger<CreateUserHandler>> _loggerMock;
    private readonly CreateUserHandler _handler;

    public CreateUserTests(AppDbContextFixture fixture)
    {
        _dbContext = fixture.DbContext;

        _loggerMock = new Mock<ILogger<CreateUserHandler>>();
        _handler = new CreateUserHandler(_dbContext, _loggerMock.Object);
    }

    [Fact]
    public async Task ShouldCreateNewUser_WhenUserDoesNotExist()
    {
        // Arrange
        var command = new CreateUserCommand(
            FullName: "John Doe",
            CpfCnpj: "12345678900",
            Email: "john@example.com",
            Password: "password123",
            Type: UserType.Common
        );

        // Act
        CreateUserResponse result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.FullName, result.FullName);
        Assert.Equal(command.CpfCnpj, result.CpfCnpj);
        Assert.Equal(command.Email, result.Email);

        User? createdUser = await _dbContext.Users.FindAsync(result.Id);
        Assert.NotNull(createdUser);
        Assert.Equal(command.FullName, createdUser.FullName);
        Assert.Equal(command.CpfCnpj, createdUser.CpfCnpj);
        Assert.Equal(command.Email, createdUser.Email);
        Assert.Equal(command.Type, createdUser.Type);
    }

    [Fact]
    public async Task ShouldThrowException_WhenUserWithCpfCnpjAlreadyExists()
    {
        // Arrange
        var command = new CreateUserCommand(
            FullName: "John Doe",
            CpfCnpj: "12345678900",
            Email: "john@example.com",
            Password: "password123",
            Type: UserType.Common
        );

        var existingUser = new User("Jane Doe", command.CpfCnpj, "jane@example.com", "hashedPassword", UserType.Common);
        _dbContext.Users.Add(existingUser);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        // Act & Assert
        AppException exception = await Assert.ThrowsAsync<AppException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal($"A user with CPF/CNPJ \"{command.CpfCnpj}\" already exists", exception.Message);
    }

    [Fact]
    public async Task ShouldThrowException_WhenUserWithEmailAlreadyExists()
    {
        // Arrange
        var command = new CreateUserCommand(
            FullName: "John Doe",
            CpfCnpj: "12345678900",
            Email: "john@example.com",
            Password: "password123",
            Type: UserType.Common
        );

        var existingUser = new User("Jane Doe", "09876543210", command.Email, "hashedPassword", UserType.Common);
        _dbContext.Users.Add(existingUser);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        // Act & Assert
        AppException exception = await Assert.ThrowsAsync<AppException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal($"A user with email \"{command.Email}\" already exists", exception.Message);
    }
}
