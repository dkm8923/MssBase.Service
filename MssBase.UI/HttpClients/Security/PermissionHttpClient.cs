using Contract.Security.Permission;
using Microsoft.Extensions.Options;
using MssBase.UI.Configuration;
using Shared.Models;
using Shared.Models.Dtos;
using Dto.Security.Permission;
using Dto.Security.Permission.Service;
using System.Net;
using MssBase.UI.HttpClients.Shared;

namespace MssBase.UI.HttpClients.Security
{
    public class PermissionHttpClient : IPermissionService
    {
        private readonly HttpClient _httpClient;

        public PermissionHttpClient(HttpClient httpClient, IOptions<SecurityApiOptions> options)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(options.Value.BaseAddress, UriKind.Absolute);
        }

        public Task<ErrorValidationResult<IEnumerable<PermissionDto>>> GetAll(
            BaseServiceGet req,
            CancellationToken cancellationToken = default)
        {
            return HttpClientUtils.GetResultAsync<IEnumerable<PermissionDto>>(
                _httpClient,
                $"Permission{HttpClientUtils.BuildQuery(req)}",
                cancellationToken);
        }

        public Task<ErrorValidationResult<PermissionDto>> GetById(
            int permissionId,
            BaseServiceGet req,
            CancellationToken cancellationToken = default)
        {
            return HttpClientUtils.GetResultAsync<PermissionDto>(
                _httpClient,
                $"Permission/{permissionId}{HttpClientUtils.BuildQuery(req)}",
                cancellationToken);
        }

        public Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByPermissionId(
            int permissionId,
            BaseServiceGet req,
            CancellationToken cancellationToken = default)
        {
            return HttpClientUtils.GetResultAsync<IEnumerable<AuditLogDto>>(
                _httpClient,
                $"Permission/{permissionId}/AuditLogs{HttpClientUtils.BuildQuery(req)}",
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

            return await HttpClientUtils.ReadResultAsync<IEnumerable<PermissionDto>>(response, cancellationToken);
        }

        public async Task<ErrorValidationResult<PermissionDto>> Insert(InsertUpdatePermissionRequest req)
        {
            using var response = await _httpClient.PostAsJsonAsync("Permission", req);
            return await HttpClientUtils.ReadResultAsync<PermissionDto>(response);
        }

        public async Task<ErrorValidationResult<PermissionDto>> Update(
            int permissionId,
            InsertUpdatePermissionRequest req)
        {
            using var response = await _httpClient.PutAsJsonAsync($"Permission/{permissionId}", req);
            return await HttpClientUtils.ReadResultAsync<PermissionDto>(response);
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
    }
}