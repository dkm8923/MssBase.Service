using Contract.Security.RolePermission;
using Dto.Security.RolePermission;
using Dto.Security.RolePermission.Service;
using Microsoft.AspNetCore.Mvc;
using MssBase.Service.Controllers.Shared;
using Shared.Models;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes;

namespace MssBase.Service.Controllers.Security
{
    [Route("api/security/[controller]")]
    [ApiController]
    [Tags("RolePermission")]
    [AutoValidationAttribute]
    public class RolePermissionController : ApiBaseController
    {
        private readonly IRolePermissionService _rolePermissionSvc;

        public RolePermissionController(IRolePermissionService rolePermissionSvc)
        {
            _rolePermissionSvc = rolePermissionSvc;
        }

        #region GetAll

        [HttpGet()]
        public async Task<IActionResult> GetRolePermissions([FromQuery] bool deleteCache = false, [FromQuery] bool includeInactive = false, [FromQuery] bool includeRelated = false)
        {
            try
            {
                var records = await _rolePermissionSvc.GetAll(new BaseServiceGet { DeleteCache = deleteCache, IncludeInactive = includeInactive, IncludeRelated = includeRelated });
                return Ok(records);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        #endregion

        #region GetById

        [HttpGet("{applicationUserId}", Name = "GetRolePermission")]
        public async Task<IActionResult> GetRolePermission(int applicationUserId, [FromQuery] bool deleteCache = false, [FromQuery] bool includeInactive = false, [FromQuery] bool includeRelated = false)
        {
            try
            {
                var record = await _rolePermissionSvc.GetById(applicationUserId, new BaseServiceGet { DeleteCache = deleteCache, IncludeInactive = includeInactive, IncludeRelated = includeRelated });

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
        public async Task<IActionResult> FilterRolePermissions(FilterRolePermissionServiceRequest req)
        {
            try
            {
                var records = await _rolePermissionSvc.Filter(req);
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
        public async Task<IActionResult> InsertRolePermission(InsertUpdateRolePermissionRequest req)
        {
            try
            {
                var result = await _rolePermissionSvc.Insert(req);

                if (result.Errors.Count > 0)
                {
                    return BadRequest(result);
                }

                return CreatedAtRoute("GetRolePermission", new { applicationUserId = result.Response.RolePermissionId }, result);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        #endregion

        #region Update

        [HttpPut("{applicationUserId}")]
        public async Task<IActionResult> UpdateRolePermission(int applicationUserId, InsertUpdateRolePermissionRequest req)
        {
            try
            {
                var result = await _rolePermissionSvc.Update(applicationUserId, req);
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
        public async Task<IActionResult> DeleteRolePermission(int applicationUserId)
        {
            try
            {
                var result = await _rolePermissionSvc.Delete(applicationUserId);
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
