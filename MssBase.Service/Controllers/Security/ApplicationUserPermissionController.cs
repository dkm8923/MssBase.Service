using Contract.Security.ApplicationUserPermission;
using Dto.Security.ApplicationUserPermission;
using Dto.Security.ApplicationUserPermission.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MssBase.Service.Controllers.Shared;
using MssBase.Service.Shared.Authorization;
using Shared.Models;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes;
using Shared.Logic.Common;

namespace MssBase.Service.Controllers.Security
{
    [Route("api/security/[controller]")]
    [ApiController]
    [Tags("ApplicationUserPermission")]
    [AutoValidationAttribute]
    [Authorize]
    public class ApplicationUserPermissionController : ApiBaseController
    {
        private readonly IApplicationUserPermissionService _applicationUserSvc;

        public ApplicationUserPermissionController(IApplicationUserPermissionService applicationUserSvc)
        {
            _applicationUserSvc = applicationUserSvc;
        }

        #region GetAll

        [HttpGet()]
        [RequiredPermission(UserApiPermissions.ApplicationUserPermissionRead)]
        public async Task<IActionResult> GetApplicationUserPermissions([FromQuery] bool deleteCache = false, [FromQuery] bool includeInactive = false, [FromQuery] bool includeRelated = false, [FromQuery] bool includeReadOnly = false, CancellationToken cancellationToken = default)
        {
            try
            {
                var records = await _applicationUserSvc.GetAll(new BaseServiceGet { DeleteCache = deleteCache, IncludeInactive = includeInactive, IncludeRelated = includeRelated, IncludeReadOnly = includeReadOnly }, cancellationToken);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        #endregion

        #region GetById

        [HttpGet("{applicationUserId}", Name = "GetApplicationUserPermissionById")]
        [RequiredPermission(UserApiPermissions.ApplicationUserPermissionRead)]
        public async Task<IActionResult> GetApplicationUserPermissionById(int applicationUserId, [FromQuery] bool deleteCache = false, [FromQuery] bool includeInactive = false, [FromQuery] bool includeRelated = false, [FromQuery] bool includeReadOnly = false, CancellationToken cancellationToken = default)
        {
            try
            {
                var record = await _applicationUserSvc.GetById(applicationUserId, new BaseServiceGet { DeleteCache = deleteCache, IncludeInactive = includeInactive, IncludeRelated = includeRelated, IncludeReadOnly = includeReadOnly }, cancellationToken);

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
        [RequiredPermission(UserApiPermissions.ApplicationUserPermissionRead)]
        public async Task<IActionResult> FilterApplicationUserPermissions(FilterApplicationUserPermissionServiceRequest req, CancellationToken cancellationToken = default)
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
        [RequiredPermission(UserApiPermissions.ApplicationUserPermissionInsert)]
        public async Task<IActionResult> InsertApplicationUserPermission(InsertUpdateApplicationUserPermissionRequest req)
        {
            try
            {
                var result = await _applicationUserSvc.Insert(req);

                if (result.Errors.Count > 0)
                {
                    return BadRequest(result);
                }

                return CreatedAtRoute("GetApplicationUserPermissionById", new { applicationUserId = result.Response.ApplicationUserPermissionId }, result);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        #endregion

        #region Update

        [HttpPut("{applicationUserId}")]
        [RequiredPermission(UserApiPermissions.ApplicationUserPermissionUpdate)]    
        public async Task<IActionResult> UpdateApplicationUserPermission(int applicationUserId, InsertUpdateApplicationUserPermissionRequest req)
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
        [RequiredPermission(UserApiPermissions.ApplicationUserPermissionDelete)]
        public async Task<IActionResult> DeleteApplicationUserPermission(int applicationUserId, [FromQuery] string currentUser = Constants.ApplicationName)
        {
            try
            {
                var result = await _applicationUserSvc.Delete(applicationUserId, currentUser);
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
