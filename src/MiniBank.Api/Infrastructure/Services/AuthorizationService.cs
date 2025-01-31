namespace MiniBank.Api.Infrastructure.Services;

internal class AuthorizationService(HttpClient httpClient) : IAuthorizationService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<bool> IsTransactionAuthorizedAsync()
    {
        var requestUri = new Uri("authorize", UriKind.Relative);

        HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();

        string? content = await response.Content.ReadAsStringAsync();

        return content == null;
    }
}
