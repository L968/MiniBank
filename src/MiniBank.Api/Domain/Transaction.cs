namespace MiniBank.Api.Domain;

internal sealed class Transaction
{
    public Guid Id { get; init; }
    public int PayerId { get; init; }
    public int PayeeId { get; init; }
    public decimal Value { get; init; }
    public DateTime Timestamp { get; init; }
    public TransactionStatus Status { get; private set; }
    public string Message { get; private set; }

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
        Message = "";

        Payer = payer;
        Payee = payee;
    }

    public void Process()
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
            Message = "Transaction competed successfully.";
        }
        catch (AppException ex)
        {
            Fail(ex.Message);
        }
    }

    public void Fail(string message)
    {
        if (Status != TransactionStatus.Pending)
        {
            throw new AppException("Only pending transactions can fail.");
        }

        Status = TransactionStatus.Failed;
        Message = message;
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
        Message = "Transaction reverted.";
    }
}
