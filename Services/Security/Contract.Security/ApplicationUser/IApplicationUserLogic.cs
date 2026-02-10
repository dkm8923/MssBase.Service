using Dto.Security.ApplicationUser;
using Dto.Security.ApplicationUser.Logic;
using Shared.Models;

namespace Contract.Security.ApplicationUser
{
    public interface IApplicationUserLogic
    {
        public Task<ErrorValidationResult<IEnumerable<ApplicationUserDto>>> GetAll(BaseLogicGet req);
        public Task<ErrorValidationResult<ApplicationUserDto>> GetById(int applicationUserId, BaseLogicGet req);
        public Task<ErrorValidationResult<IEnumerable<ApplicationUserDto>>> Filter(FilterApplicationUserLogicRequest req);
        public Task<ErrorValidationResult<ApplicationUserDto>> Insert(InsertUpdateApplicationUserRequest req);
        public Task<ErrorValidationResult<ApplicationUserDto>> Update(int applicationUserId, InsertUpdateApplicationUserRequest req);
        public Task<ErrorValidationResult> Delete(int applicationUserId);
    }
}
