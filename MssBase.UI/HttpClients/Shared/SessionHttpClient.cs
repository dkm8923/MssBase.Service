using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MssBase.UI.HttpClients.Shared;

public sealed class SessionHttpClient(HttpClient httpClient, CurrentSessionTokenProvider tokenProvider)
{
    /// <summary>Sends a GET request with the current session's bearer token.</summary>
    public Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken = default) =>
        SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), cancellationToken);

    /// <summary>Sends a JSON POST request with the current session's bearer token.</summary>
    public Task<HttpResponseMessage> PostAsJsonAsync<T>(
        string requestUri,
        T value,
        CancellationToken cancellationToken = default) =>
        SendAsync(new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = JsonContent.Create(value)
        }, cancellationToken);

    /// <summary>Sends a JSON PUT request with the current session's bearer token.</summary>
    public Task<HttpResponseMessage> PutAsJsonAsync<T>(
        string requestUri,
        T value,
        CancellationToken cancellationToken = default) =>
        SendAsync(new HttpRequestMessage(HttpMethod.Put, requestUri)
        {
            Content = JsonContent.Create(value)
        }, cancellationToken);

    /// <summary>Sends a DELETE request with the current session's bearer token.</summary>
    public Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken cancellationToken = default) =>
        SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri), cancellationToken);

    /// <summary>Attaches the current bearer token, sends the request, and expires the session on unauthorized responses.</summary>
    private async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        using (request)
        {
            var token = await tokenProvider.GetAccessTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
            {
                await tokenProvider.ExpireSessionAsync();
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await httpClient.SendAsync(request, cancellationToken);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                response.Dispose();
                await tokenProvider.ExpireSessionAsync();
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            return response;
        }
    }
}