using Contract.Security.Application;
using Dto.Security.Application;
using Dto.Security.Application.Service;
using Microsoft.AspNetCore.Mvc;
using MssBase.Service.Controllers.Shared;
using Shared.Models;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes;

namespace MssBase.Service.Controllers.Security
{
    [Route("api/security/[controller]")]
    [ApiController]
    [Tags("Application")]
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
