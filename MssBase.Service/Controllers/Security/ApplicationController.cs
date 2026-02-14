using MssBase.Service.Controllers.Shared;
using MssBase.Service.Shared;
using Dto.Security.Application;
using Dto.Security.Application.Service;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes;
using Swashbuckle.AspNetCore.Annotations;
using Contract.Security.Application;

namespace MssBase.Service.Controllers.Security
{
    [Route("api/security/[controller]")]
    [ApiController]
    [Tags("Application")]
    [SwaggerTag("Application specific end points within Security Service")]
    [AutoValidationAttribute]
    public class ApplicationController : ApiBaseController
    {
        //TODO: Global exception handling?
        private readonly IApplicationService _applicationSvc;

        public ApplicationController(IApplicationService applicationSvc)
        {
            _applicationSvc = applicationSvc;
        }

        // GET: api/Security/Application

        #region swagger

        [SwaggerOperation(
            Summary = "Retrieves list of all available Application record(s)",
            Description = $@"{SwaggerUiDocumentation.QueryStringParms.QueryStringParmTitle}
                            {SwaggerUiDocumentation.QueryStringParms.DeleteCache}
                            {SwaggerUiDocumentation.QueryStringParms.IncludeInactive}
                            {SwaggerUiDocumentation.QueryStringParms.IncludeRelated}
                            "
        )]
        [SwaggerResponse(StatusCodes.Status200OK, SwaggerUiDocumentation.DefaultResponseMessage.GetAllRecords, typeof(ErrorValidationResult<IEnumerable<ApplicationDto>>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, SwaggerUiDocumentation.DefaultResponseMessage.InternalServerError)]

        #endregion

        [HttpGet()]
        public async Task<IActionResult> GetApplications([FromQuery] bool deleteCache = false, [FromQuery] bool includeInactive = false, [FromQuery] bool includeRelated = false)
        {
            try
            {
                var records = await _applicationSvc.GetAll(new BaseServiceGet { DeleteCache = deleteCache, IncludeInactive = includeInactive }, includeRelated);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        // GET: api/Security/Application/{applicationId}

        #region swagger

        [SwaggerOperation(
        Summary = "Retrieves specified Application record by Id",
            Description = $@"{SwaggerUiDocumentation.QueryStringParms.QueryStringParmTitle}
                            {SwaggerUiDocumentation.QueryStringParms.DeleteCache}
                            {SwaggerUiDocumentation.QueryStringParms.IncludeInactive}
                            {SwaggerUiDocumentation.QueryStringParms.IncludeRelated}
                            "
        )]
        [SwaggerResponse(StatusCodes.Status200OK, SwaggerUiDocumentation.DefaultResponseMessage.GetRecordById, typeof(ErrorValidationResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, SwaggerUiDocumentation.DefaultResponseMessage.InternalServerError)]

        #endregion

        [HttpGet("{applicationId}", Name = "GetApplication")]
        public async Task<IActionResult> GetApplication(int applicationId, [FromQuery] bool deleteCache = false, [FromQuery] bool includeInactive = false, [FromQuery] bool includeRelated = false)
        {
            try
            {
                var record = await _applicationSvc.GetById(applicationId, new BaseServiceGet { DeleteCache = deleteCache, IncludeInactive = includeInactive }, includeRelated);

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

        // POST: api/Security/Application/Filter

        #region swagger

        [SwaggerOperation(
            Summary = "Retrieves list of Applications that match filter parameters in request body"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, SwaggerUiDocumentation.DefaultResponseMessage.FilterRecords, typeof(ErrorValidationResult<IEnumerable<ApplicationDto>>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, SwaggerUiDocumentation.DefaultResponseMessage.InternalServerError)]

        #endregion

        [HttpPost("Filter")]
        public async Task<IActionResult> FilterApplications(FilterApplicationServiceRequest req)
        {
            try
            {
                var records = await _applicationSvc.Filter(req);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        // POST: api/Security/Application

        #region swagger

        [SwaggerOperation(
            Summary = "Creates new Application record"
        )]
        [SwaggerResponse(StatusCodes.Status201Created, SwaggerUiDocumentation.DefaultResponseMessage.InsertRecord, typeof(ErrorValidationResult<ApplicationDto>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, SwaggerUiDocumentation.DefaultResponseMessage.InternalServerError)]

        #endregion

        [HttpPost()]
        public async Task<IActionResult> InsertApplication(InsertUpdateApplicationRequest req)
        {
            try
            {
                var result = await _applicationSvc.Insert(req);

                if (result.Errors.Count > 0)
                {
                    return BadRequest(result);
                }

                return CreatedAtRoute("GetApplication", new { applicationId = result.Response.ApplicationId }, result);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        // PUT: api/Security/Application

        #region swagger

        [SwaggerOperation(
            Summary = "Updates Existing Application record"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, SwaggerUiDocumentation.DefaultResponseMessage.UpdateRecord, typeof(ErrorValidationResult<ApplicationDto>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, SwaggerUiDocumentation.DefaultResponseMessage.InternalServerError)]

        #endregion

        [HttpPut("{applicationId}")]
        public async Task<IActionResult> UpdateApplication(int applicationId, InsertUpdateApplicationRequest req)
        {
            try
            {
                var result = await _applicationSvc.Update(applicationId, req);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        // DELETE: api/Security/Application

        #region swagger

        [SwaggerOperation(
            Summary = "Deletes Application record by Id"
        )]
        [SwaggerResponse(StatusCodes.Status204NoContent, SwaggerUiDocumentation.DefaultResponseMessage.DeleteRecord, typeof(ErrorValidationResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, SwaggerUiDocumentation.DefaultResponseMessage.InternalServerError)]

        #endregion

        [HttpDelete("{applicationId}")]
        public async Task<IActionResult> DeleteApplication(int applicationId)
        {
            try
            {
                var result = await _applicationSvc.Delete(applicationId);
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
    }
}
