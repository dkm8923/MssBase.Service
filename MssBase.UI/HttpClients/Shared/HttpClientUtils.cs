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
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                // The 400 body's "response" is a string, so only the errors are read.
                var errors = await response.Content.ReadFromJsonAsync<ErrorValidationResult>(
                    cancellationToken: cancellationToken);

                return new ErrorValidationResult<T> { Errors = errors?.Errors ?? new() };
            }

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ErrorValidationResult<T>>(
                cancellationToken: cancellationToken);

            return result ?? new ErrorValidationResult<T>();
        }

        public static string BuildQuery(BaseServiceGet req)
        {
            return $"?deleteCache={req.DeleteCache}&includeInactive={req.IncludeInactive}&includeReadOnly={req.IncludeReadOnly}";
        }
    }
}