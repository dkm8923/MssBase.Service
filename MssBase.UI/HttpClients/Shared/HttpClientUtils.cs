using System.Net;
using Shared.Models;

namespace MssBase.UI.HttpClients.Shared
{
    public static class HttpClientUtils
    {
        public static async Task<ErrorValidationResult<T>> GetResultAsync<T>(
            HttpClient httpClient,
            string requestUri,
            CancellationToken cancellationToken)
        {
            using var response = await httpClient.GetAsync(requestUri, cancellationToken);
            return await ReadResultAsync<T>(response, cancellationToken);
        }

        public static async Task<ErrorValidationResult<T>> ReadResultAsync<T>(
            HttpResponseMessage response,
            CancellationToken cancellationToken = default)
        {
            var result = await response.Content.ReadFromJsonAsync<ErrorValidationResult<T>>(
                cancellationToken: cancellationToken);

            if (response.StatusCode == HttpStatusCode.BadRequest && result is not null)
            {
                return result;
            }

            response.EnsureSuccessStatusCode();
            return result ?? new ErrorValidationResult<T>();
        }

        public static string BuildQuery(BaseServiceGet req)
        {
            return $"?deleteCache={req.DeleteCache}&includeInactive={req.IncludeInactive}&includeReadOnly={req.IncludeReadOnly}";
        }
    }
}