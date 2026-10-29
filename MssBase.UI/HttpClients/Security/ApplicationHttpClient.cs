using Contract.Security.Application;
using Microsoft.Extensions.Options;
using MssBase.UI.Configuration;
using Shared.Models;
using Shared.Models.Dtos;
using Dto.Security.Application;
using Dto.Security.Application.Service;
using System.Net;
using MssBase.UI.HttpClients.Shared;

namespace MssBase.UI.HttpClients.Security
{
    public class ApplicationHttpClient: IApplicationService
    {
        private readonly HttpClient _httpClient;

        public ApplicationHttpClient(HttpClient httpClient, IOptions<SecurityApiOptions> options)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(options.Value.BaseAddress, UriKind.Absolute);
        }

        public Task<ErrorValidationResult<IEnumerable<ApplicationDto>>> GetAll(
            BaseServiceGet req,
            CancellationToken cancellationToken = default)
        {
            return HttpClientUtils.GetResultAsync<IEnumerable<ApplicationDto>>(
                _httpClient,
                $"Application{HttpClientUtils.BuildQuery(req)}",
                cancellationToken);
        }

        public Task<ErrorValidationResult<ApplicationDto>> GetById(
            int applicationId,
            BaseServiceGet req,
            CancellationToken cancellationToken = default)
        {
            return HttpClientUtils.GetResultAsync<ApplicationDto>(
                _httpClient,
                $"Application/{applicationId}{HttpClientUtils.BuildQuery(req)}",
                cancellationToken);
        }

        public Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByApplicationId(
            int applicationId,
            BaseServiceGet req,
            CancellationToken cancellationToken = default)
        {
            return HttpClientUtils.GetResultAsync<IEnumerable<AuditLogDto>>(
                _httpClient,
                $"Application/{applicationId}/AuditLogs{HttpClientUtils.BuildQuery(req)}",
                cancellationToken);
        }

        public async Task<ErrorValidationResult<IEnumerable<ApplicationDto>>> Filter(
            FilterApplicationServiceRequest req,
            CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.PostAsJsonAsync(
                "Application/Filter",
                req,
                cancellationToken);

            return await HttpClientUtils.ReadResultAsync<IEnumerable<ApplicationDto>>(response, cancellationToken);
        }

        public async Task<ErrorValidationResult<ApplicationDto>> Insert(InsertUpdateApplicationRequest req)
        {
            using var response = await _httpClient.PostAsJsonAsync("Application", req);
            return await HttpClientUtils.ReadResultAsync<ApplicationDto>(response);
        }

        public async Task<ErrorValidationResult<ApplicationDto>> Update(
            int applicationId,
            InsertUpdateApplicationRequest req)
        {
            using var response = await _httpClient.PutAsJsonAsync($"Application/{applicationId}", req);
            return await HttpClientUtils.ReadResultAsync<ApplicationDto>(response);
        }

        public async Task<ErrorValidationResult<ApplicationDto>> Delete(int applicationId, string currentUser)
        {
            var query = Uri.EscapeDataString(currentUser);
            using var response = await _httpClient.DeleteAsync(
                $"Application/{applicationId}?currentUser={query}");

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return await response.Content.ReadFromJsonAsync<ErrorValidationResult<ApplicationDto>>()
                    ?? new ErrorValidationResult<ApplicationDto>();
            }

            response.EnsureSuccessStatusCode();
            return new ErrorValidationResult<ApplicationDto>();
        }
    }
}