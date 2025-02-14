﻿namespace MiniBank.Api.Features.Transactions.Commands.Transfer;

internal sealed class TransferValidator : AbstractValidator<TransferCommand>
{
    public TransferValidator()
    {
        RuleFor(cmd => cmd.Value)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");

        RuleFor(cmd => cmd.Payer)
            .NotEmpty().WithMessage("Payer ID must be provided.");

        RuleFor(cmd => cmd.Payee)
            .NotEmpty().WithMessage("Payee ID must be provided.");

        RuleFor(cmd => new { cmd.Payer, cmd.Payee })
            .Must(x => x.Payer != x.Payee)
            .WithMessage("The payer and the payee cannot be the same person.");
    }
}
