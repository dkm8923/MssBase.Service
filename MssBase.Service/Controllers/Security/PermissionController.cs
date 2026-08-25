using Contract.Security.Permission;
using Dto.Security.Permission;
using Dto.Security.Permission.Service;
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
    [Tags("Permission")]
    [AutoValidationAttribute]
    [Authorize]
    public class PermissionController : ApiBaseController
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        #region GetAll

        [HttpGet()]
        [RequiredPermission(UserApiPermissions.PermissionRead)] 
        public async Task<IActionResult> GetPermissions([FromQuery] bool deleteCache = false, [FromQuery] bool includeInactive = false, [FromQuery] bool includeReadOnly = false, CancellationToken cancellationToken = default)
        {
            try
            {
                var records = await _permissionService.GetAll(new BaseServiceGet { DeleteCache = deleteCache, IncludeInactive = includeInactive, IncludeReadOnly = includeReadOnly }, cancellationToken);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        #endregion

        #region GetById

        [HttpGet("{permissionId}", Name = "GetPermissionById")]
        [RequiredPermission(UserApiPermissions.PermissionRead)] 
        public async Task<IActionResult> GetPermissionById(int permissionId, [FromQuery] bool deleteCache = false, [FromQuery] bool includeInactive = false, [FromQuery] bool includeReadOnly = false, CancellationToken cancellationToken = default)
        {
            try
            {
                var record = await _permissionService.GetById(permissionId, new BaseServiceGet { DeleteCache = deleteCache, IncludeInactive = includeInactive, IncludeReadOnly = includeReadOnly }, cancellationToken);

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

        // GET: api/Security/Permission/{permissionId}/AuditLogs

        [HttpGet("{permissionId}/AuditLogs", Name = "GetPermissionAuditLogsById")]
        [RequiredPermission(UserApiPermissions.PermissionRead)]
        public async Task<IActionResult> GetPermissionAuditLogsById(int permissionId, [FromQuery] bool deleteCache = false, CancellationToken cancellationToken = default)
        {
            try
            {
                var record = await _permissionService.GetAuditLogsByPermissionId(permissionId, new BaseServiceGet { DeleteCache = deleteCache }, cancellationToken);

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
        [RequiredPermission(UserApiPermissions.PermissionRead)] 
        public async Task<IActionResult> FilterPermissions(FilterPermissionServiceRequest req, CancellationToken cancellationToken = default)
        {
            try
            {
                var records = await _permissionService.Filter(req, cancellationToken);
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
        [RequiredPermission(UserApiPermissions.PermissionInsert)]
        public async Task<IActionResult> InsertPermission(InsertUpdatePermissionRequest req)
        {
            try
            {
                var result = await _permissionService.Insert(req);

                if (result.Errors.Count > 0)
                {
                    return BadRequest(result);
                }

                return CreatedAtRoute("GetPermissionById", new { permissionId = result.Response.PermissionId }, result);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        #endregion

        #region Update

        [HttpPut("{permissionId}")]
        [RequiredPermission(UserApiPermissions.PermissionUpdate)]
        public async Task<IActionResult> UpdatePermission(int permissionId, InsertUpdatePermissionRequest req)
        {
            try
            {
                var result = await _permissionService.Update(permissionId, req);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        #endregion

        #region Delete

        [HttpDelete("{permissionId}")]
        [RequiredPermission(UserApiPermissions.PermissionDelete)]
        public async Task<IActionResult> DeletePermission(int permissionId, [FromQuery] string currentUser = Constants.ApplicationName)
        {
            try
            {
                var result = await _permissionService.Delete(permissionId, currentUser);
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
