﻿namespace MiniBank.Api.Infrastructure.Services;

internal class NotificationService(HttpClient httpClient) : INotificationService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task Notify()
    {
        var requestUri = new Uri("api/v1/notify", UriKind.Relative);

        HttpResponseMessage response = await _httpClient.PostAsync(requestUri, null);
        response.EnsureSuccessStatusCode();
    }
}
