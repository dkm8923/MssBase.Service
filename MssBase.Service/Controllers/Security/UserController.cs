using Contract.Security.User;
using Dto.Security.User;
using Dto.Security.User.Service;
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
    [Tags("User")]
    [AutoValidationAttribute]
    [Authorize]
    public class UserController : ApiBaseController
    {
        private readonly IUserService _userSvc;

        public UserController(IUserService userSvc)
        {
            _userSvc = userSvc;
        }

        #region GetAll

        [HttpGet()]
        [RequiredPermission(UserApiPermissions.UserRead)]
        public async Task<IActionResult> GetUsers([FromQuery] bool deleteCache = false, [FromQuery] bool includeInactive = false, [FromQuery] bool includeRelated = false, [FromQuery] bool includeReadOnly = false, CancellationToken cancellationToken = default)
        {
            try
            {
                var records = await _userSvc.GetAll(new BaseServiceGet { DeleteCache = deleteCache, IncludeInactive = includeInactive, IncludeRelated = includeRelated, IncludeReadOnly = includeReadOnly }, cancellationToken);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        #endregion

        #region GetById

        [HttpGet("{userId}", Name = "GetUserById")]
        [RequiredPermission(UserApiPermissions.UserRead)]
        public async Task<IActionResult> GetUserById(int userId, [FromQuery] bool deleteCache = false, [FromQuery] bool includeInactive = false, [FromQuery] bool includeRelated = false, [FromQuery] bool includeReadOnly = false, CancellationToken cancellationToken = default)
        {
            try
            {
                var record = await _userSvc.GetById(userId, new BaseServiceGet { DeleteCache = deleteCache, IncludeInactive = includeInactive, IncludeRelated = includeRelated, IncludeReadOnly = includeReadOnly }, cancellationToken);

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

        [HttpGet("PasswordChangeHistory/{userId}")]
        [RequiredPermission(UserApiPermissions.UserPasswordChangeHistoryRead)]
        public async Task<IActionResult> GetPasswordChangeHistory(int userId, [FromQuery] bool deleteCache = false, CancellationToken cancellationToken = default)
        {
            try
            {
                var records = await _userSvc.GetPasswordChangeHistoryByUserId(userId, deleteCache, cancellationToken);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        // GET: api/Security/User/{userId}/AuditLogs

        [HttpGet("{userId}/AuditLogs", Name = "GetUserAuditLogsById")]
        [RequiredPermission(UserApiPermissions.UserRead)]
        public async Task<IActionResult> GetUserAuditLogsById(int userId, [FromQuery] bool deleteCache = false, CancellationToken cancellationToken = default)
        {
            try
            {
                var record = await _userSvc.GetAuditLogsByUserId(userId, new BaseServiceGet { DeleteCache = deleteCache }, cancellationToken);

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
        [RequiredPermission(UserApiPermissions.UserRead)]
        public async Task<IActionResult> FilterUsers(FilterUserServiceRequest req, CancellationToken cancellationToken = default)
        {
            try
            {
                var records = await _userSvc.Filter(req, cancellationToken);
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
        [RequiredPermission(UserApiPermissions.UserInsert)]
        public async Task<IActionResult> InsertUser(InsertUpdateUserRequest req)
        {
            try
            {
                var result = await _userSvc.Insert(req);

                if (result.Errors.Count > 0)
                {
                    return BadRequest(result);
                }

                return CreatedAtRoute("GetUserById", new { userId = result.Response.UserId }, result);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        #endregion

        #region Update

        [HttpPut("{userId}")]
        [RequiredPermission(UserApiPermissions.UserUpdate)]
        public async Task<IActionResult> UpdateUser(int userId, InsertUpdateUserRequest req)
        {
            try
            {
                var result = await _userSvc.Update(userId, req);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        #endregion

        #region Delete

        [HttpDelete("{userId}")]
        [RequiredPermission(UserApiPermissions.UserDelete)]
        public async Task<IActionResult> DeleteUser(int userId, [FromQuery] string currentUser = Constants.ApplicationName)
        {
            try
            {
                var result = await _userSvc.Delete(userId, currentUser);
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

        [HttpPost("ResetPassword/{userId}")]
        [RequiredPermission(UserApiPermissions.UserResetPassword)]
        public async Task<IActionResult> ResetPassword(int userId)
        {
            try
            {
                var result = await _userSvc.ResetPassword(userId);
                if (result.Errors.Count > 0)                
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        [HttpPost("ChangePassword")]
        [RequiredPermission(UserApiPermissions.UserChangePassword)]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest req)
        {
            try
            {
                var result = await _userSvc.ChangePassword(req);
                if (result.Errors.Count > 0)                
                {
                    return BadRequest(result);
                }
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }
    }
}
