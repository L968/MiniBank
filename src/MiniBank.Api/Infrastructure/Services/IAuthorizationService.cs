namespace MiniBank.Api.Infrastructure.Services;

internal interface IAuthorizationService
{
    Task<bool> IsTransactionAuthorizedAsync();
}
