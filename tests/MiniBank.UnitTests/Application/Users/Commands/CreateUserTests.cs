using Microsoft.Extensions.Logging;
using Moq;
using MiniBank.Api.Domain;
using MiniBank.Api.Features.Users.Commands.CreateUser;
using MiniBank.Api.Infrastructure.Repositories.Interfaces;
using MiniBank.Api.Exceptions;

namespace MiniBank.UnitTests.Application.Users.Commands;

public class CreateUserTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<CreateUserHandler>> _loggerMock;
    private readonly CreateUserHandler _handler;

    public CreateUserTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CreateUserHandler>>();

        _handler = new CreateUserHandler(_repositoryMock.Object, _unitOfWorkMock.Object, _loggerMock.Object);
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

        _repositoryMock
            .Setup(x => x.GetByCpfCnpjAsync(command.CpfCnpj, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);

        _repositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);

        // Act
        CreateUserResponse result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.FullName, result.FullName);
        Assert.Equal(command.CpfCnpj, result.CpfCnpj);
        Assert.Equal(command.Email, result.Email);

        _repositoryMock.Verify(x => x.Create(It.IsAny<User>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
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

        _repositoryMock
            .Setup(x => x.GetByCpfCnpjAsync(command.CpfCnpj, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

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

        _repositoryMock
            .Setup(x => x.GetByCpfCnpjAsync(command.CpfCnpj, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);

        _repositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act & Assert
        AppException exception = await Assert.ThrowsAsync<AppException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal($"A user with email \"{command.Email}\" already exists", exception.Message);
    }
}
