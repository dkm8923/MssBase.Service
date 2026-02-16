using System;
using Dto.Security.ApplicationUser;
using Dto.Security.ApplicationUser.Service;
using Shared.Models;

namespace Contract.Security.ApplicationUser;

public interface IApplicationUserService
{
    public Task<ErrorValidationResult<IEnumerable<ApplicationUserDto>>> GetAll(BaseServiceGet req, bool includeRelated = false);
    public Task<ErrorValidationResult<ApplicationUserDto>> GetById(int applicationUserId, BaseServiceGet req, bool includeRelated = false);
    public Task<ErrorValidationResult<IEnumerable<ApplicationUserDto>>> Filter(FilterApplicationUserServiceRequest req);
    public Task<ErrorValidationResult<ApplicationUserDto>> Insert(InsertUpdateApplicationUserRequest req);
    public Task<ErrorValidationResult<ApplicationUserDto>> Update(int applicationUserId, InsertUpdateApplicationUserRequest req);
    public Task<ErrorValidationResult> Delete(int applicationUserId);
}
