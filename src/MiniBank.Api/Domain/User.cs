namespace MiniBank.Api.Domain;

internal sealed class User
{
    public int Id { get; init; }
    public string FullName { get; private set; }
    public string CpfCnpj { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public UserType Type { get; private set; }
    public decimal Balance { get; private set; }

    private User() { }

    public User(
        string fullName,
        string cpfCnpj,
        string email,
        string passwordHash,
        UserType type
    )
    {
        FullName = fullName;
        CpfCnpj = cpfCnpj;
        Email = email;
        PasswordHash = passwordHash;
        Type = type;
        Balance = 0;
    }

    public void ValidateCanTransfer(decimal value)
    {
        if (Type != UserType.Common)
        {
            throw new AppException(DomainErrors.User.OnlyCommonUsersCanSendMoney);
        }

        if (Balance < value)
        {
            throw new AppException(DomainErrors.User.InsufficientBalance);
        }
    }

    public void Debit(decimal value)
    {
        if (value <= 0)
        {
            throw new AppException(DomainErrors.User.InvalidValue);
        }

        if (Balance < value)
        {
            throw new AppException(DomainErrors.User.InsufficientBalance);
        }

        Balance -= value;
    }

    public void Credit(decimal value)
    {
        if (value <= 0)
        {
            throw new AppException(DomainErrors.User.InvalidValue);
        }

        Balance += value;
    }
}
