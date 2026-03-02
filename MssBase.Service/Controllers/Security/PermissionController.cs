using Contract.Security.Permission;
using Dto.Security.Permission;
using Dto.Security.Permission.Service;
using Microsoft.AspNetCore.Mvc;
using MssBase.Service.Controllers.Shared;
using Shared.Models;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes;

namespace MssBase.Service.Controllers.Security
{
    [Route("api/security/[controller]")]
    [ApiController]
    [Tags("Permission")]
    [AutoValidationAttribute]
    public class PermissionController : ApiBaseController
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        #region GetAll

        [HttpGet()]
        public async Task<IActionResult> GetPermissions([FromQuery] bool deleteCache = false, [FromQuery] bool includeInactive = false, [FromQuery] bool includeRelated = false)
        {
            try
            {
                var records = await _permissionService.GetAll(new BaseServiceGet { DeleteCache = deleteCache, IncludeInactive = includeInactive }, includeRelated);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        #endregion

        #region GetById

        [HttpGet("{permissionId}", Name = "GetPermission")]
        public async Task<IActionResult> GetPermission(int permissionId, [FromQuery] bool deleteCache = false, [FromQuery] bool includeInactive = false, [FromQuery] bool includeRelated = false)
        {
            try
            {
                var record = await _permissionService.GetById(permissionId, new BaseServiceGet { DeleteCache = deleteCache, IncludeInactive = includeInactive }, includeRelated);

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
        public async Task<IActionResult> FilterPermissions(FilterPermissionServiceRequest req)
        {
            try
            {
                var records = await _permissionService.Filter(req);
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
        public async Task<IActionResult> InsertPermission(InsertUpdatePermissionRequest req)
        {
            try
            {
                var result = await _permissionService.Insert(req);

                if (result.Errors.Count > 0)
                {
                    return BadRequest(result);
                }

                return CreatedAtRoute("GetPermission", new { permissionId = result.Response.PermissionId }, result);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        #endregion

        #region Update

        [HttpPut("{permissionId}")]
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
        public async Task<IActionResult> DeletePermission(int permissionId)
        {
            try
            {
                var result = await _permissionService.Delete(permissionId);
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
