namespace MiniBank.Api.Domain;

internal sealed class Transaction
{
    public Guid Id { get; private set; }
    public int PayerId { get; private set; }
    public int PayeeId { get; private set; }
    public decimal Value { get; private set; }
    public DateTime Timestamp { get; private set; }
    public TransactionStatus Status { get; private set; }

    public User Payer { get; private set; }
    public User Payee { get; private set; }

    private Transaction() { }

    public Transaction(User payer, User payee, decimal value)
    {
        if (value <= 0)
        {
            throw new AppException("Invalid value.");
        }

        Id = Guid.CreateVersion7();
        PayerId = payer.Id;
        PayeeId = payee.Id;
        Value = value;
        Timestamp = DateTime.UtcNow;
        Status = TransactionStatus.Pending;

        Payer = payer;
        Payee = payee;
    }

    public void Execute()
    {
        if (Status != TransactionStatus.Pending)
        {
            throw new AppException("Only pending transactions can be executed.");
        }

        try
        {
            Payer.ValidateCanTransfer(Value);
            Payer.Debit(Value);
            Payee.Credit(Value);
            Status = TransactionStatus.Completed;
        }
        catch (AppException)
        {
            Status = TransactionStatus.Failed;
            throw;
        }
    }

    public void Fail()
    {
        if (Status != TransactionStatus.Pending)
        {
            throw new AppException("Only pending transactions can fail.");
        }

        Status = TransactionStatus.Failed;
    }

    public void Revert()
    {
        if (Status != TransactionStatus.Completed)
        {
            throw new AppException("Only completed transactions can be reverted.");
        }

        Payer.Credit(Value);
        Payee.Debit(Value);

        Status = TransactionStatus.Reverted;
    }
}
