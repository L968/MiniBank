namespace MiniBank.Api.Infrastructure.Services;

internal class AuthorizationService(HttpClient httpClient) : IAuthorizationService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<bool> IsTransactionAuthorizedAsync()
    {
        var requestUri = new Uri("api/v2/authorize", UriKind.Relative);

        HttpResponseMessage response = await _httpClient.GetAsync(requestUri);

        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();
        return true;
    }
}
