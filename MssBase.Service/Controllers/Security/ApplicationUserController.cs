using Contract.Security.ApplicationUser;
using Dto.Security.ApplicationUser;
using Dto.Security.ApplicationUser.Service;
using Microsoft.AspNetCore.Mvc;
using MssBase.Service.Controllers.Shared;
using Shared.Models;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes;

namespace MssBase.Service.Controllers.Security
{
    [Route("api/security/[controller]")]
    [ApiController]
    [Tags("ApplicationUser")]
    [AutoValidationAttribute]
    public class ApplicationUserController : ApiBaseController
    {
        private readonly IApplicationUserService _applicationUserSvc;

        public ApplicationUserController(IApplicationUserService applicationUserSvc)
        {
            _applicationUserSvc = applicationUserSvc;
        }

        #region GetAll

        [HttpGet()]
        public async Task<IActionResult> GetApplicationUsers([FromQuery] bool deleteCache = false, [FromQuery] bool includeInactive = false, [FromQuery] bool includeRelated = false)
        {
            try
            {
                var records = await _applicationUserSvc.GetAll(new BaseServiceGet { DeleteCache = deleteCache, IncludeInactive = includeInactive }, includeRelated);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        #endregion

        #region GetById

        [HttpGet("{applicationUserId}", Name = "GetApplicationUser")]
        public async Task<IActionResult> GetApplicationUser(int applicationUserId, [FromQuery] bool deleteCache = false, [FromQuery] bool includeInactive = false, [FromQuery] bool includeRelated = false)
        {
            try
            {
                var record = await _applicationUserSvc.GetById(applicationUserId, new BaseServiceGet { DeleteCache = deleteCache, IncludeInactive = includeInactive }, includeRelated);

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
        public async Task<IActionResult> FilterApplicationUsers(FilterApplicationUserServiceRequest req)
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
        public async Task<IActionResult> InsertApplicationUser(InsertUpdateApplicationUserRequest req)
        {
            try
            {
                var result = await _applicationUserSvc.Insert(req);

                if (result.Errors.Count > 0)
                {
                    return BadRequest(result);
                }

                return CreatedAtRoute("GetApplicationUser", new { applicationUserId = result.Response.ApplicationUserId }, result);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        #endregion

        #region Update

        [HttpPut("{applicationUserId}")]
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
