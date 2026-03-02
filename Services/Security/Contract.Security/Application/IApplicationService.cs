using System;
using Dto.Security.Application;
using Dto.Security.Application.Service;
using Shared.Models;

namespace Contract.Security.Application;

public interface IApplicationService
{
    public Task<ErrorValidationResult<IEnumerable<ApplicationDto>>> GetAll(BaseServiceGet req);
    public Task<ErrorValidationResult<ApplicationDto>> GetById(int applicationId, BaseServiceGet req);
    public Task<ErrorValidationResult<IEnumerable<ApplicationDto>>> Filter(FilterApplicationServiceRequest req);
    public Task<ErrorValidationResult<ApplicationDto>> Insert(InsertUpdateApplicationRequest req);
    public Task<ErrorValidationResult<ApplicationDto>> Update(int applicationId, InsertUpdateApplicationRequest req);
    public Task<ErrorValidationResult<ApplicationDto>> Delete(int applicationId);
}
