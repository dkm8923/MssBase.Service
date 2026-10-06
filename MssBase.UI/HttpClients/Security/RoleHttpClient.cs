using Contract.Security.Role;
using Microsoft.Extensions.Options;
using MssBase.UI.Configuration;
using Shared.Models;
using Shared.Models.Dtos;
using Dto.Security.Role;
using Dto.Security.Role.Service;
using System.Net;
using MssBase.UI.HttpClients.Shared;

namespace MssBase.UI.HttpClients.Security
{
    public class RoleHttpClient : IRoleService
    {
        private readonly HttpClient _httpClient;

        public RoleHttpClient(HttpClient httpClient, IOptions<SecurityApiOptions> options)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(options.Value.BaseAddress, UriKind.Absolute);
        }

        public Task<ErrorValidationResult<IEnumerable<RoleDto>>> GetAll(
            BaseServiceGet req,
            CancellationToken cancellationToken = default)
        {
            return HttpClientUtils.GetResultAsync<IEnumerable<RoleDto>>(
                _httpClient,
                $"Role{HttpClientUtils.BuildQuery(req)}",
                cancellationToken);
        }

        public Task<ErrorValidationResult<RoleDto>> GetById(
            int RoleId,
            BaseServiceGet req,
            CancellationToken cancellationToken = default)
        {
            return HttpClientUtils.GetResultAsync<RoleDto>(
                _httpClient,
                $"Role/{RoleId}{HttpClientUtils.BuildQuery(req)}",
                cancellationToken);
        }

        public Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByRoleId(
            int RoleId,
            BaseServiceGet req,
            CancellationToken cancellationToken = default)
        {
            return HttpClientUtils.GetResultAsync<IEnumerable<AuditLogDto>>(
                _httpClient,
                $"Role/{RoleId}/AuditLogs{HttpClientUtils.BuildQuery(req)}",
                cancellationToken);
        }

        public async Task<ErrorValidationResult<IEnumerable<RoleDto>>> Filter(
            FilterRoleServiceRequest req,
            CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.PostAsJsonAsync(
                "Role/Filter",
                req,
                cancellationToken);

            return await HttpClientUtils.ReadResultAsync<IEnumerable<RoleDto>>(response, cancellationToken);
        }

        public async Task<ErrorValidationResult<RoleDto>> Insert(InsertUpdateRoleRequest req)
        {
            using var response = await _httpClient.PostAsJsonAsync("Role", req);
            return await HttpClientUtils.ReadResultAsync<RoleDto>(response);
        }

        public async Task<ErrorValidationResult<RoleDto>> Update(
            int RoleId,
            InsertUpdateRoleRequest req)
        {
            using var response = await _httpClient.PutAsJsonAsync($"Role/{RoleId}", req);
            return await HttpClientUtils.ReadResultAsync<RoleDto>(response);
        }

        public async Task<ErrorValidationResult> Delete(int RoleId, string currentUser)
        {
            var query = Uri.EscapeDataString(currentUser);
            using var response = await _httpClient.DeleteAsync(
                $"Role/{RoleId}?currentUser={query}");

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