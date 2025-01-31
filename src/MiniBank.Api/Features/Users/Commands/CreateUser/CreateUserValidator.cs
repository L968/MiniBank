namespace MiniBank.Api.Features.Users.Commands.CreateUser;

internal sealed class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(u => u.FullName)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(u => u.CpfCnpj)
            .NotEmpty()
            .Matches("^[0-9]{11,14}$").WithMessage("CPF/CNPJ must be a valid format.");

        RuleFor(u => u.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(u => u.Password)
            .NotEmpty()
            .MinimumLength(6);

        RuleFor(u => u.Type)
            .IsInEnum();
    }
}
