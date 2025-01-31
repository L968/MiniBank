namespace MiniBank.Api.Features.Transactions.Commands.SendMoney;

internal sealed class SendMoneyValidator : AbstractValidator<SendMoneyCommand>
{
    public SendMoneyValidator()
    {
        RuleFor(cmd => cmd.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");

        RuleFor(cmd => cmd.SenderId)
            .NotEmpty().WithMessage("Sender ID must be provided.");

        RuleFor(cmd => cmd.ReceiverId)
            .NotEmpty().WithMessage("Receiver ID must be provided.");
    }
}
