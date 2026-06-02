using Contract.Security.ApplicationUser;
using Dto.Security.ApplicationUser;
using Dto.Security.ApplicationUser.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MssBase.Service.Controllers.Shared;
using MssBase.Service.Shared.Authorization;
using Shared.Models;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes;

namespace MssBase.Service.Controllers.Security
{
    [Route("api/security/[controller]")]
    [ApiController]
    [Tags("ApplicationUser")]
    [AutoValidationAttribute]
    [Authorize]
    public class ApplicationUserController : ApiBaseController
    {
        private readonly IApplicationUserService _applicationUserSvc;

        public ApplicationUserController(IApplicationUserService applicationUserSvc)
        {
            _applicationUserSvc = applicationUserSvc;
        }

        #region GetAll

        [HttpGet()]
        [RequiredPermission(UserApiPermissions.ApplicationUserRead)]
        public async Task<IActionResult> GetApplicationUsers([FromQuery] bool deleteCache = false, [FromQuery] bool includeInactive = false, [FromQuery] bool includeRelated = false, CancellationToken cancellationToken = default)
        {
            try
            {
                var records = await _applicationUserSvc.GetAll(new BaseServiceGet { DeleteCache = deleteCache, IncludeInactive = includeInactive, IncludeRelated = includeRelated }, cancellationToken);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        #endregion

        #region GetById

        [HttpGet("{applicationUserId}", Name = "GetApplicationUserById")]
        [RequiredPermission(UserApiPermissions.ApplicationUserRead)]
        public async Task<IActionResult> GetApplicationUserById(int applicationUserId, [FromQuery] bool deleteCache = false, [FromQuery] bool includeInactive = false, [FromQuery] bool includeRelated = false, CancellationToken cancellationToken = default)
        {
            try
            {
                var record = await _applicationUserSvc.GetById(applicationUserId, new BaseServiceGet { DeleteCache = deleteCache, IncludeInactive = includeInactive, IncludeRelated = includeRelated }, cancellationToken);

                if (record.Response == null)
                {
                    return NotFound();
                }

                return Ok(record);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        #endregion

        #region Filter

        [HttpPost("Filter")]
        [RequiredPermission(UserApiPermissions.ApplicationUserRead)]
        public async Task<IActionResult> FilterApplicationUsers(FilterApplicationUserServiceRequest req, CancellationToken cancellationToken = default)
        {
            try
            {
                var records = await _applicationUserSvc.Filter(req, cancellationToken);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        #endregion

        #region Insert

        [HttpPost()]
        [RequiredPermission(UserApiPermissions.ApplicationUserInsert)]
        public async Task<IActionResult> InsertApplicationUser(InsertUpdateApplicationUserRequest req)
        {
            try
            {
                var result = await _applicationUserSvc.Insert(req);

                if (result.Errors.Count > 0)
                {
                    return BadRequest(result);
                }

                return CreatedAtRoute("GetApplicationUserById", new { applicationUserId = result.Response.ApplicationUserId }, result);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        #endregion

        #region Update

        [HttpPut("{applicationUserId}")]
        [RequiredPermission(UserApiPermissions.ApplicationUserUpdate)]
        public async Task<IActionResult> UpdateApplicationUser(int applicationUserId, InsertUpdateApplicationUserRequest req)
        {
            try
            {
                var result = await _applicationUserSvc.Update(applicationUserId, req);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        #endregion

        #region Delete

        [HttpDelete("{applicationUserId}")]
        [RequiredPermission(UserApiPermissions.ApplicationUserDelete)]
        public async Task<IActionResult> DeleteApplicationUser(int applicationUserId)
        {
            try
            {
                var result = await _applicationUserSvc.Delete(applicationUserId);
                if (result.Errors.Count > 0)
                {
                    return BadRequest(result);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        #endregion
    }
}
