using Contract.Security.RolePermission;
using Dto.Security.RolePermission;
using Dto.Security.RolePermission.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MssBase.Service.Controllers.Shared;
using MssBase.Service.Shared.Authorization;
using Shared.Logic.Common;
using Shared.Models;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes;

namespace MssBase.Service.Controllers.Security
{
    [Route("api/security/[controller]")]
    [ApiController]
    [Tags("RolePermission")]
    [AutoValidationAttribute]
    [Authorize]
    public class RolePermissionController : ApiBaseController
    {
        private readonly IRolePermissionService _rolePermissionSvc;

        public RolePermissionController(IRolePermissionService rolePermissionSvc)
        {
            _rolePermissionSvc = rolePermissionSvc;
        }

        #region GetAll

        [HttpGet()]
        [RequiredPermission(UserApiPermissions.RolePermissionRead)] 
        public async Task<IActionResult> GetRolePermissions([FromQuery] bool deleteCache = false, [FromQuery] bool includeInactive = false, [FromQuery] bool includeRelated = false, [FromQuery] bool includeReadOnly = false, CancellationToken cancellationToken = default)
        {
            try
            {
                var records = await _rolePermissionSvc.GetAll(new BaseServiceGet { DeleteCache = deleteCache, IncludeInactive = includeInactive, IncludeRelated = includeRelated, IncludeReadOnly = includeReadOnly }, cancellationToken);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        #endregion

        #region GetById

        [HttpGet("{rolePermissionId}", Name = "GetRolePermissionById")]
        [RequiredPermission(UserApiPermissions.RolePermissionRead)]
        public async Task<IActionResult> GetRolePermissionById(int rolePermissionId, [FromQuery] bool deleteCache = false, [FromQuery] bool includeInactive = false, [FromQuery] bool includeRelated = false, [FromQuery] bool includeReadOnly = false, CancellationToken cancellationToken = default)
        {
            try
            {
                var record = await _rolePermissionSvc.GetById(rolePermissionId, new BaseServiceGet { DeleteCache = deleteCache, IncludeInactive = includeInactive, IncludeRelated = includeRelated, IncludeReadOnly = includeReadOnly }, cancellationToken);

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

        // GET: api/Security/RolePermission/{rolePermissionId}/AuditLogs

        [HttpGet("{rolePermissionId}/AuditLogs", Name = "GetRolePermissionAuditLogsById")]
        [RequiredPermission(UserApiPermissions.RolePermissionRead)]
        public async Task<IActionResult> GetRolePermissionAuditLogsById(int rolePermissionId, [FromQuery] bool deleteCache = false, CancellationToken cancellationToken = default)
        {
            try
            {
                var record = await _rolePermissionSvc.GetAuditLogsByRolePermissionId(rolePermissionId, new BaseServiceGet { DeleteCache = deleteCache }, cancellationToken);

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
        [RequiredPermission(UserApiPermissions.RolePermissionRead)] 
        public async Task<IActionResult> FilterRolePermissions(FilterRolePermissionServiceRequest req, CancellationToken cancellationToken = default)
        {
            try
            {
                var records = await _rolePermissionSvc.Filter(req, cancellationToken);
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
        [RequiredPermission(UserApiPermissions.RolePermissionInsert)]
        public async Task<IActionResult> InsertRolePermission(InsertUpdateRolePermissionRequest req)
        {
            try
            {
                var result = await _rolePermissionSvc.Insert(req);

                if (result.Errors.Count > 0)
                {
                    return BadRequest(result);
                }

                return CreatedAtRoute("GetRolePermissionById", new { rolePermissionId = result.Response.RolePermissionId }, result);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        #endregion

        #region Update

        [HttpPut("{rolePermissionId}")]
        [RequiredPermission(UserApiPermissions.RolePermissionUpdate)]
        public async Task<IActionResult> UpdateRolePermission(int rolePermissionId, InsertUpdateRolePermissionRequest req)
        {
            try
            {
                var result = await _rolePermissionSvc.Update(rolePermissionId, req);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        #endregion

        #region Delete

        [HttpDelete("{rolePermissionId}")]
        [RequiredPermission(UserApiPermissions.RolePermissionDelete)]
        public async Task<IActionResult> DeleteRolePermission(int rolePermissionId, [FromQuery] string currentUser = Constants.ApplicationName)
        {
            try
            {
                var result = await _rolePermissionSvc.Delete(rolePermissionId, currentUser);
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
