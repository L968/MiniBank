namespace MiniBank.Api.Domain;

internal sealed class User
{
    public int Id { get; private set; }
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
            throw new AppException("Only Common users are allowed to send money.");
        }

        if (Balance < value)
        {
            throw new AppException($"Insufficient balance. Current balance: {Balance:C}.");
        }
    }

    public void Debit(decimal value)
    {
        if (value <= 0)
        {
            throw new AppException("Invalid value.");
        }

        if (Balance < value)
        {
            throw new AppException("Insufficient balance.");
        }

        Balance -= value;
    }

    public void Credit(decimal value)
    {
        if (value <= 0)
        {
            throw new AppException("Invalid value.");
        }

        Balance += value;
    }
}
