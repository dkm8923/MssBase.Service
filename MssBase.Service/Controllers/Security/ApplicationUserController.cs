using MssBase.Service.Controllers.Shared;
using MssBase.Service.Shared;
using Dto.Security.ApplicationUser;
using Dto.Security.ApplicationUser.Service;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes;
using Swashbuckle.AspNetCore.Annotations;
using Contract.Security.ApplicationUser;

namespace MssBase.Service.Controllers.Security
{
    [Route("api/security/[controller]")]
    [ApiController]
    [Tags("ApplicationUser")]
    [SwaggerTag("ApplicationUser specific end points within Security Service")]
    [AutoValidationAttribute]
    public class ApplicationUserController : ApiBaseController
    {
        private readonly IApplicationUserService _applicationUserSvc;

        public ApplicationUserController(IApplicationUserService applicationUserSvc)
        {
            _applicationUserSvc = applicationUserSvc;
        }

        #region GetAll

        #region swagger

        [SwaggerOperation(
            Summary = "Retrieves list of all available ApplicationUser record(s)",
            Description = $@"{SwaggerUiDocumentation.QueryStringParms.QueryStringParmTitle}
                            {SwaggerUiDocumentation.QueryStringParms.DeleteCache}
                            {SwaggerUiDocumentation.QueryStringParms.IncludeInactive}
                            {SwaggerUiDocumentation.QueryStringParms.IncludeRelated}
                            "
        )]
        [SwaggerResponse(StatusCodes.Status200OK, SwaggerUiDocumentation.DefaultResponseMessage.GetAllRecords, typeof(ErrorValidationResult<IEnumerable<ApplicationUserDto>>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, SwaggerUiDocumentation.DefaultResponseMessage.InternalServerError)]

        #endregion

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

        #region swagger

        [SwaggerOperation(
        Summary = "Retrieves specified ApplicationUser record by Id",
            Description = $@"{SwaggerUiDocumentation.QueryStringParms.QueryStringParmTitle}
                            {SwaggerUiDocumentation.QueryStringParms.DeleteCache}
                            {SwaggerUiDocumentation.QueryStringParms.IncludeInactive}
                            {SwaggerUiDocumentation.QueryStringParms.IncludeRelated}
                            "
        )]
        [SwaggerResponse(StatusCodes.Status200OK, SwaggerUiDocumentation.DefaultResponseMessage.GetRecordById, typeof(ErrorValidationResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, SwaggerUiDocumentation.DefaultResponseMessage.InternalServerError)]

        #endregion

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

        #region swagger

        [SwaggerOperation(
            Summary = "Retrieves list of ApplicationUsers that match filter parameters in request body"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, SwaggerUiDocumentation.DefaultResponseMessage.FilterRecords, typeof(ErrorValidationResult<IEnumerable<ApplicationUserDto>>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, SwaggerUiDocumentation.DefaultResponseMessage.InternalServerError)]

        #endregion

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

        #region swagger

        [SwaggerOperation(
            Summary = "Creates new ApplicationUser record"
        )]
        [SwaggerResponse(StatusCodes.Status201Created, SwaggerUiDocumentation.DefaultResponseMessage.InsertRecord, typeof(ErrorValidationResult<ApplicationUserDto>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, SwaggerUiDocumentation.DefaultResponseMessage.InternalServerError)]

        #endregion

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

        #region swagger

        [SwaggerOperation(
            Summary = "Updates Existing ApplicationUser record"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, SwaggerUiDocumentation.DefaultResponseMessage.UpdateRecord, typeof(ErrorValidationResult<ApplicationUserDto>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, SwaggerUiDocumentation.DefaultResponseMessage.InternalServerError)]

        #endregion

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

        #region swagger

        [SwaggerOperation(
            Summary = "Deletes ApplicationUser record by Id"
        )]
        [SwaggerResponse(StatusCodes.Status204NoContent, SwaggerUiDocumentation.DefaultResponseMessage.DeleteRecord, typeof(ErrorValidationResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, SwaggerUiDocumentation.DefaultResponseMessage.InternalServerError)]

        #endregion

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
