using Contract.Security.ApplicationUserRole;
using Dto.Security.ApplicationUserRole;
using Dto.Security.ApplicationUserRole.Service;
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
    [Tags("ApplicationUserRole")]
    [AutoValidationAttribute]
    [Authorize]
    public class ApplicationUserRoleController : ApiBaseController
    {
        private readonly IApplicationUserRoleService _applicationUserSvc;

        public ApplicationUserRoleController(IApplicationUserRoleService applicationUserSvc)
        {
            _applicationUserSvc = applicationUserSvc;
        }

        #region GetAll

        [HttpGet()]
        [RequiredPermission(UserApiPermissions.ApplicationUserRoleRead)]
        public async Task<IActionResult> GetApplicationUserRoles([FromQuery] bool deleteCache = false, [FromQuery] bool includeInactive = false, [FromQuery] bool includeRelated = false, CancellationToken cancellationToken = default)
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

        [HttpGet("{applicationUserId}", Name = "GetApplicationUserRoleById")]
        [RequiredPermission(UserApiPermissions.ApplicationUserRoleRead)]
        public async Task<IActionResult> GetApplicationUserRoleById(int applicationUserId, [FromQuery] bool deleteCache = false, [FromQuery] bool includeInactive = false, [FromQuery] bool includeRelated = false, CancellationToken cancellationToken = default)
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
        [RequiredPermission(UserApiPermissions.ApplicationUserRoleRead)]
        public async Task<IActionResult> FilterApplicationUserRoles(FilterApplicationUserRoleServiceRequest req, CancellationToken cancellationToken = default)
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
        [RequiredPermission(UserApiPermissions.ApplicationUserRoleInsert)]
        public async Task<IActionResult> InsertApplicationUserRole(InsertUpdateApplicationUserRoleRequest req)
        {
            try
            {
                var result = await _applicationUserSvc.Insert(req);

                if (result.Errors.Count > 0)
                {
                    return BadRequest(result);
                }

                return CreatedAtRoute("GetApplicationUserRoleById", new { applicationUserId = result.Response.ApplicationUserRoleId }, result);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        #endregion

        #region Update

        [HttpPut("{applicationUserId}")]
        [RequiredPermission(UserApiPermissions.ApplicationUserRoleUpdate)]
        public async Task<IActionResult> UpdateApplicationUserRole(int applicationUserId, InsertUpdateApplicationUserRoleRequest req)
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
        [RequiredPermission(UserApiPermissions.ApplicationUserRoleDelete)]
        public async Task<IActionResult> DeleteApplicationUserRole(int applicationUserId)
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
