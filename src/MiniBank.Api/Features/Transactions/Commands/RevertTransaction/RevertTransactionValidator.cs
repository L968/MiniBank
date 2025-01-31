namespace MiniBank.Api.Features.Transactions.Commands.RevertTransaction;

internal sealed class RevertTransactionValidator : AbstractValidator<RevertTransactionCommand>
{
    public RevertTransactionValidator()
    {
        RuleFor(cmd => cmd.TransactionId)
            .NotEmpty().WithMessage("Transaction ID must be provided.");
    }
}
