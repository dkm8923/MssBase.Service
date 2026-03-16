using Contract.Security.ApplicationUserRole;
using Dto.Security.ApplicationUserRole;
using Dto.Security.ApplicationUserRole.Service;
using Microsoft.AspNetCore.Mvc;
using MssBase.Service.Controllers.Shared;
using Shared.Models;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes;

namespace MssBase.Service.Controllers.Security
{
    [Route("api/security/[controller]")]
    [ApiController]
    [Tags("ApplicationUserRole")]
    [AutoValidationAttribute]
    public class ApplicationUserRoleController : ApiBaseController
    {
        private readonly IApplicationUserRoleService _applicationUserSvc;

        public ApplicationUserRoleController(IApplicationUserRoleService applicationUserSvc)
        {
            _applicationUserSvc = applicationUserSvc;
        }

        #region GetAll

        [HttpGet()]
        public async Task<IActionResult> GetApplicationUserRoles([FromQuery] bool deleteCache = false, [FromQuery] bool includeInactive = false, [FromQuery] bool includeRelated = false)
        {
            try
            {
                var records = await _applicationUserSvc.GetAll(new BaseServiceGet { DeleteCache = deleteCache, IncludeInactive = includeInactive });
                return Ok(records);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        #endregion

        #region GetById

        [HttpGet("{applicationUserId}", Name = "GetApplicationUserRole")]
        public async Task<IActionResult> GetApplicationUserRole(int applicationUserId, [FromQuery] bool deleteCache = false, [FromQuery] bool includeInactive = false, [FromQuery] bool includeRelated = false)
        {
            try
            {
                var record = await _applicationUserSvc.GetById(applicationUserId, new BaseServiceGet { DeleteCache = deleteCache, IncludeInactive = includeInactive });

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
        public async Task<IActionResult> FilterApplicationUserRoles(FilterApplicationUserRoleServiceRequest req)
        {
            try
            {
                var records = await _applicationUserSvc.Filter(req);
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
        public async Task<IActionResult> InsertApplicationUserRole(InsertUpdateApplicationUserRoleRequest req)
        {
            try
            {
                var result = await _applicationUserSvc.Insert(req);

                if (result.Errors.Count > 0)
                {
                    return BadRequest(result);
                }

                return CreatedAtRoute("GetApplicationUserRole", new { applicationUserId = result.Response.ApplicationUserRoleId }, result);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        #endregion

        #region Update

        [HttpPut("{applicationUserId}")]
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
