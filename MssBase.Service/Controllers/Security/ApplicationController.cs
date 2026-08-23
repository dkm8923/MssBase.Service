using Contract.Security.Application;
using Dto.Security.Application;
using Dto.Security.Application.Service;
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
    [Tags("Application")]
    [AutoValidationAttribute]
    [Authorize]
    public class ApplicationController : ApiBaseController
    {
        //TODO: Global exception handling?
        private readonly IApplicationService _applicationSvc;

        public ApplicationController(IApplicationService applicationSvc)
        {
            _applicationSvc = applicationSvc;
        }

        #region GetAll

        // GET: api/Security/Application
        
        [HttpGet()]
        [RequiredPermission(UserApiPermissions.ApplicationRead)] 
        public async Task<IActionResult> GetApplications([FromQuery] bool deleteCache = false, [FromQuery] bool includeInactive = false, [FromQuery] bool includeRelated = false, [FromQuery] bool includeReadOnly = false, CancellationToken cancellationToken = default)
        {
            try
            {
                var records = await _applicationSvc.GetAll(new BaseServiceGet { DeleteCache = deleteCache, IncludeInactive = includeInactive, IncludeRelated = includeRelated, IncludeReadOnly = includeReadOnly }, cancellationToken);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        #endregion

        #region GetById

        // GET: api/Security/Application/{applicationId}

        [HttpGet("{applicationId}", Name = "GetApplicationById")]
        [RequiredPermission(UserApiPermissions.ApplicationRead)]
        public async Task<IActionResult> GetApplicationById(int applicationId, [FromQuery] bool deleteCache = false, [FromQuery] bool includeInactive = false, [FromQuery] bool includeRelated = false, [FromQuery] bool includeReadOnly = false, CancellationToken cancellationToken = default)
        {
            try
            {
                var record = await _applicationSvc.GetById(applicationId, new BaseServiceGet { DeleteCache = deleteCache, IncludeInactive = includeInactive, IncludeRelated = includeRelated, IncludeReadOnly = includeReadOnly }, cancellationToken);

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

        // GET: api/Security/Application/{applicationId}/AuditLogs

        [HttpGet("{applicationId}/AuditLogs", Name = "GetApplicationAuditLogsById")]
        [RequiredPermission(UserApiPermissions.ApplicationRead)]
        public async Task<IActionResult> GetApplicationAuditLogsById(int applicationId, [FromQuery] bool deleteCache = false, CancellationToken cancellationToken = default)
        {
            try
            {
                var record = await _applicationSvc.GetAuditLogsByApplicationId(applicationId, new BaseServiceGet { DeleteCache = deleteCache }, cancellationToken);

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

        // POST: api/Security/Application/Filter

        [HttpPost("Filter")]
        [RequiredPermission(UserApiPermissions.ApplicationRead)]
        public async Task<IActionResult> FilterApplications(FilterApplicationServiceRequest req, CancellationToken cancellationToken = default)
        {
            try
            {
                var records = await _applicationSvc.Filter(req, cancellationToken);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        #endregion

        #region Insert

        // POST: api/Security/Application

        [HttpPost()]
        [RequiredPermission(UserApiPermissions.ApplicationInsert)]
        public async Task<IActionResult> InsertApplication(InsertUpdateApplicationRequest req)
        {
            try
            {
                var result = await _applicationSvc.Insert(req);

                if (result.Errors.Count > 0 || result.Response is null)
                {
                    return BadRequest(result);
                }

                return CreatedAtRoute("GetApplicationById", new { applicationId = result.Response.ApplicationId }, result);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        #endregion

        #region Update

        // PUT: api/Security/Application

        [HttpPut("{applicationId}")]
        [RequiredPermission(UserApiPermissions.ApplicationUpdate)]
        public async Task<IActionResult> UpdateApplication(int applicationId, InsertUpdateApplicationRequest? req)
        {
            try
            {
                if (req is null)
                {
                    return BadRequest();
                }

                var result = await _applicationSvc.Update(applicationId, req!);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        #endregion

        #region Delete

        // DELETE: api/Security/Application

        [HttpDelete("{applicationId}")]
        [RequiredPermission(UserApiPermissions.ApplicationDelete)]
        public async Task<IActionResult> DeleteApplication(int applicationId, [FromQuery] string currentUser = Constants.ApplicationName)
        {
            try
            {
                var result = await _applicationSvc.Delete(applicationId, currentUser);
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
