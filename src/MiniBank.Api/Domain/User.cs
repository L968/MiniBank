namespace MiniBank.Api.Domain;

internal sealed class User
{
    public Guid Id { get; private set; }
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
        Id = Guid.CreateVersion7();
        FullName = fullName;
        CpfCnpj = cpfCnpj;
        Email = email;
        PasswordHash = passwordHash;
        Type = type;
        Balance = 0;
    }

    public void ValidateCanSendMoney(decimal amount)
    {
        if (Type != UserType.Common)
        {
            throw new AppException("Only Common users are allowed to send money.");
        }

        if (Balance < amount)
        {
            throw new AppException($"Insufficient balance. Current balance: {Balance:C}.");
        }
    }

    public void Debit(decimal amount)
    {
        if (amount <= 0)
        {
            throw new AppException("Invalid amount.");
        }

        if (Balance < amount)
        {
            throw new AppException("Insufficient balance.");
        }

        Balance -= amount;
    }

    public void Credit(decimal amount)
    {
        if (amount <= 0)
        {
            throw new AppException("Invalid amount.");
        }

        Balance += amount;
    }
}
