namespace MiniBank.Api.Features.Users.Queries.GetUserById;

internal sealed class GetUserByIdValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdValidator()
    {
        RuleFor(q => q.Id)
            .NotEmpty().WithMessage("User ID must be provided.");
    }
}
