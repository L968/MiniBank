namespace MiniBank.Api.Domain;

internal sealed class Transaction
{
    public Guid Id { get; private set; }
    public Guid SenderId { get; private set; }
    public Guid ReceiverId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime Timestamp { get; private set; }
    public TransactionStatus Status { get; private set; }

    public User Sender { get; private set; }
    public User Receiver { get; private set; }

    private Transaction() { }

    public Transaction(User sender, User receiver, decimal amount)
    {
        if (amount <= 0)
        {
            throw new AppException("Invalid amount.");
        }

        Id = Guid.NewGuid();
        SenderId = sender.Id;
        ReceiverId = receiver.Id;
        Amount = amount;
        Timestamp = DateTime.UtcNow;
        Status = TransactionStatus.Pending;

        Sender = sender;
        Receiver = receiver;
    }

    public void Execute()
    {
        if (Status != TransactionStatus.Pending)
        {
            throw new AppException("Only pending transactions can be executed.");
        }

        try
        {
            Sender.ValidateCanSendMoney(Amount);
            Sender.Debit(Amount);
            Receiver.Credit(Amount);
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

        Sender.Credit(Amount);
        Receiver.Debit(Amount);

        Status = TransactionStatus.Reverted;
    }
}
