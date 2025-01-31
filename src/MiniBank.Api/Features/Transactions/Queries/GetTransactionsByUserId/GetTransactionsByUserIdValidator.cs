namespace MiniBank.Api.Features.Transactions.Queries.GetTransactionsByUserId;

internal sealed class GetTransactionsByUserIdValidator : AbstractValidator<GetTransactionsByUserIdQuery>
{
    public GetTransactionsByUserIdValidator()
    {
        RuleFor(q => q.UserId)
            .NotEmpty().WithMessage("User ID must be provided.");
    }
}
