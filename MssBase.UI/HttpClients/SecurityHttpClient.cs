using System.Net;
using Contract.Security.Permission;
using Dto.Security.Permission;
using Dto.Security.Permission.Service;
using MssBase.UI.Configuration;
using Microsoft.Extensions.Options;
using Shared.Models;
using Shared.Models.Dtos;

namespace MssBase.UI.HttpClients
{
    public class SecurityHttpClient : IPermissionService
    {
        private readonly HttpClient _httpClient;

        public SecurityHttpClient(HttpClient httpClient, IOptions<SecurityApiOptions> options)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(options.Value.BaseAddress, UriKind.Absolute);
        }

        #region Permission

        public Task<ErrorValidationResult<IEnumerable<PermissionDto>>> GetAll(
            BaseServiceGet req,
            CancellationToken cancellationToken = default)
        {
            return GetResultAsync<IEnumerable<PermissionDto>>(
                $"Permission{BuildQuery(req)}",
                cancellationToken);
        }

        public Task<ErrorValidationResult<PermissionDto>> GetById(
            int permissionId,
            BaseServiceGet req,
            CancellationToken cancellationToken = default)
        {
            return GetResultAsync<PermissionDto>(
                $"Permission/{permissionId}{BuildQuery(req)}",
                cancellationToken);
        }

        public Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByPermissionId(
            int permissionId,
            BaseServiceGet req,
            CancellationToken cancellationToken = default)
        {
            return GetResultAsync<IEnumerable<AuditLogDto>>(
                $"Permission/{permissionId}/AuditLogs?deleteCache={req.DeleteCache}",
                cancellationToken);
        }

        public async Task<ErrorValidationResult<IEnumerable<PermissionDto>>> Filter(
            FilterPermissionServiceRequest req,
            CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.PostAsJsonAsync(
                "Permission/Filter",
                req,
                cancellationToken);

            return await ReadResultAsync<IEnumerable<PermissionDto>>(response, cancellationToken);
        }

        public async Task<ErrorValidationResult<PermissionDto>> Insert(InsertUpdatePermissionRequest req)
        {
            using var response = await _httpClient.PostAsJsonAsync("Permission", req);
            return await ReadResultAsync<PermissionDto>(response);
        }

        public async Task<ErrorValidationResult<PermissionDto>> Update(
            int permissionId,
            InsertUpdatePermissionRequest req)
        {
            using var response = await _httpClient.PutAsJsonAsync($"Permission/{permissionId}", req);
            return await ReadResultAsync<PermissionDto>(response);
        }

        public async Task<ErrorValidationResult> Delete(int permissionId, string currentUser)
        {
            var query = Uri.EscapeDataString(currentUser);
            using var response = await _httpClient.DeleteAsync(
                $"Permission/{permissionId}?currentUser={query}");

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return await response.Content.ReadFromJsonAsync<ErrorValidationResult>()
                    ?? new ErrorValidationResult();
            }

            response.EnsureSuccessStatusCode();
            return new ErrorValidationResult();
        }

        #endregion

        #region Utils

        private async Task<ErrorValidationResult<T>> GetResultAsync<T>(
            string requestUri,
            CancellationToken cancellationToken)
        {
            using var response = await _httpClient.GetAsync(requestUri, cancellationToken);
            return await ReadResultAsync<T>(response, cancellationToken);
        }

        private static async Task<ErrorValidationResult<T>> ReadResultAsync<T>(
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

        private static string BuildQuery(BaseServiceGet req)
        {
            return $"?deleteCache={req.DeleteCache}&includeInactive={req.IncludeInactive}&includeReadOnly={req.IncludeReadOnly}";
        }

        #endregion
    
    }
}