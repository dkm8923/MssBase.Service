using Contract.Security;
using Contract.Security.Application;
using Contract.Security.ApplicationUser;
using Data.Security;
using Data.Security.Converters;
using Dto.Security.ApplicationUser;
using Dto.Security.ApplicationUser.Logic;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using Shared.Logic;
using Shared.Logic.Validators;
using Shared.Logic.Common;
using Microsoft.AspNetCore.Identity;
using Dto.Security.Authentication;

namespace Logic.Security.Logic
{
    public class ApplicationUserLogic : IApplicationUserLogic
    {
        private readonly ISecurityConnectionStrings _connectionStrings;
        private readonly SecurityDBContextFactory _dbContextFactory;

        private IValidator<FilterApplicationUserLogicRequest> _filterApplicationUserLogicRequestValidator;
        private IValidator<InsertUpdateApplicationUserRequest> _insertUpdateApplicationUserRequestValidator;
        private IValidator<ChangePasswordRequest> _changePasswordRequestValidator;

        public ApplicationUserLogic(
                            ISecurityConnectionStrings connectionStrings,
                            IValidator<FilterApplicationUserLogicRequest> filterApplicationUserLogicRequestValidator,
                            IValidator<InsertUpdateApplicationUserRequest> insertUpdateApplicationUserRequestValidator,
                            IValidator<ChangePasswordRequest> changePasswordRequestValidator
        )
        {
            _connectionStrings = connectionStrings;
            _dbContextFactory = new SecurityDBContextFactory(_connectionStrings);
            _filterApplicationUserLogicRequestValidator = filterApplicationUserLogicRequestValidator;
            _insertUpdateApplicationUserRequestValidator = insertUpdateApplicationUserRequestValidator;
            _changePasswordRequestValidator = changePasswordRequestValidator;
        }

        /// <summary>
        /// Retrieves a collection of application users based on the specified request parameters.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<ApplicationUserDto>>> GetAll(BaseLogicGet req, CancellationToken cancellationToken = default)
        {
            var ret = await this.Filter(new FilterApplicationUserLogicRequest { IncludeInactive = req.IncludeInactive, CurrentUser = req.CurrentUser, IncludeRelated = req.IncludeRelated }, cancellationToken);
            return ret;
        }

        /// <summary>
        /// Retrieves an application user by its unique identifier.
        /// </summary>
        public async Task<ErrorValidationResult<ApplicationUserDto>> GetById(int applicationUserId, BaseLogicGet req, CancellationToken cancellationToken = default)
        {
            var res = await this.Filter(new FilterApplicationUserLogicRequest { ApplicationUserIds = new List<int> { applicationUserId }, IncludeInactive = req.IncludeInactive, CurrentUser = req.CurrentUser, IncludeRelated = req.IncludeRelated }, cancellationToken);

            return new ErrorValidationResult<ApplicationUserDto> { Response = res.Response.FirstOrDefault() };
        }

        /// <summary>
        /// Filters application users based on the specified criteria.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<ApplicationUserDto>>> Filter(FilterApplicationUserLogicRequest req, CancellationToken cancellationToken = default)
        {
            var errorValidationResult = await _validateApplicationUserFilter(req);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadOnly())
            {
                var query = dbContext.ApplicationUsers.AsQueryable().AsNoTracking();

                query = query.ApplyIncludeInactiveFilter(req);
                query = query.ApplyAuditableFilters(req);

                if (req.IncludeRelated)
                {
                    query = query.Include(application => application.ApplicationUserPermissions.Where(aup => req.IncludeInactive || aup.Active)).ThenInclude(rp => rp.Permission)
                                 .Include(application => application.ApplicationUserRoles
                                    .Where(aur => req.IncludeInactive || aur.Active))
                                    .ThenInclude(aur => aur.Role)
                                    .ThenInclude(r => r.RolePermissions)
                                    .ThenInclude(rp => rp.Permission);
                }

                if (req.ApplicationUserIds != null && req.ApplicationUserIds.Count > 0)
                {
                    query = query.Where(x => req.ApplicationUserIds.Contains(x.ApplicationUserId));
                }
                
                if (req.Email != null)
                {
                    query = query.Where(x => x.Email == req.Email);
                }

                if (req.FirstName != null)
                {
                    query = query.Where(x => x.FirstName == req.FirstName);
                }

                if (req.LastName != null)
                {
                    query = query.Where(x => x.LastName == req.LastName);
                }

                if (req.DateOfBirth != null)
                {
                    query = query.Where(x => x.DateOfBirth == req.DateOfBirth);
                }

                if (req.ApplicationId != null)
                {
                    query = query.Where(x => x.ApplicationId == req.ApplicationId);
                }

                return new ErrorValidationResult<IEnumerable<ApplicationUserDto>> { Response = await query.ToDtosWithoutPassword(cancellationToken) };
            }
        }

        /// <summary>
        /// Inserts a new application user into the data store.
        /// </summary>
        public async Task<ErrorValidationResult<ApplicationUserDto>> Insert(InsertUpdateApplicationUserRequest req, IApplicationLogic applicationLogic)
        {
            var errorValidationResult = await _validateApplicationUserOnInsertUpdate(applicationLogic, req, null);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = req.ToEntityOnInsert();

                var randomPassword = _generateRandomPassword();

                entity.Password = _hashPassword(randomPassword);
                entity.PasswordResetRequired = true;

                await dbContext.ApplicationUsers.AddAsync(entity);
                await dbContext.SaveChangesAsync();

                entity.Password = randomPassword;

                return new ErrorValidationResult<ApplicationUserDto> { Response = entity.ToDto() };
            }
        }

        /// <summary>
        /// Updates the details of an existing application user.
        /// </summary>
        public async Task<ErrorValidationResult<ApplicationUserDto>> Update(int applicationUserId, InsertUpdateApplicationUserRequest req, IApplicationLogic applicationLogic)
        {
            var errorValidationResult = await _validateApplicationUserOnInsertUpdate(applicationLogic, req, applicationUserId);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.ApplicationUsers.FirstOrDefaultAsync(ent => ent.ApplicationUserId == applicationUserId);

                if (entity != null)
                {
                    entity = entity.UpdateEntityFromRequest(req);
                    await dbContext.SaveChangesAsync();
                    return new ErrorValidationResult<ApplicationUserDto> { Response = entity.ToDtoWithoutPassword() };
                }
                else
                {
                    return _createUserNotFoundError<ApplicationUserDto>(null);
                }
            }
        }

        /// <summary>
        /// Deletes the application user with the specified identifier.
        /// </summary>
        public async Task<ErrorValidationResult> Delete(int applicationUserId)
        {
            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.ApplicationUsers.FirstOrDefaultAsync(ent => ent.ApplicationUserId == applicationUserId);
                
                if (entity != null)
                {
                    dbContext.ApplicationUsers.Remove(entity);

                    await dbContext.SaveChangesAsync();
                    
                    return new ErrorValidationResult();
                }
                else
                {
                    return _createUserNotFoundError<object?>();
                }
            }
        }

        public async Task<ErrorValidationResult<ResetPasswordResponse>> ResetPassword(int applicationUserId)
        {
            //TODO: Send email to user with new password instead of returning in response

            var newPassword = _generateRandomPassword();
            
            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.ApplicationUsers.FirstOrDefaultAsync(ent => ent.ApplicationUserId == applicationUserId);
                
                if (entity != null)
                {
                    var newHashedPassword = _hashPassword(newPassword);
                    entity.Password = newHashedPassword;
                    entity.PasswordResetRequired = true;
                    entity.LastPasswordChangeDate = CommonUtilities.GetDateTimeUtcNow();
                    
                    await dbContext.SaveChangesAsync();

                    var ret = new ResetPasswordResponse { NewPassword = newPassword };
                    return new ErrorValidationResult<ResetPasswordResponse> { Response = ret };
                }
                else
                {
                    return _createUserNotFoundError<ResetPasswordResponse>(null);
                }
            }
        }

        public async Task<ErrorValidationResult> ChangePassword(ChangePasswordRequest req)
        {
            ValidationResult result = await _changePasswordRequestValidator.ValidateAsync(req);
            var errorValidationResult = ValidatorUtilities.CreateDefaultValidationResponse<object>(result);

            if (errorValidationResult.Errors.Count > 0) 
            {
                return errorValidationResult;
            }
            
            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.ApplicationUsers.FirstOrDefaultAsync(ent => ent.ApplicationUserId == req.ApplicationUserId);
                
                if (entity is null) 
                {
                    return _createUserNotFoundError<object?>();
                }

                var passwordsMatch = SecurityLogicUtilities.VerifyPasswordMatchesHash(entity.Password, req.NewPassword);

                if (passwordsMatch)
                {
                    return new ErrorValidationResult { Errors = new Dictionary<string, List<string>> { { "ChangePassword", new List<string> { $"New password must be different from the old password!" } } } };
                }

                entity.Password = _hashPassword(req.NewPassword);
                entity.PasswordResetRequired = false;
                entity.LastPasswordChangeDate = CommonUtilities.GetDateTimeUtcNow();
                await dbContext.SaveChangesAsync();

                return new ErrorValidationResult();
            }
        }

        #region Private Methods

        /// <summary>
        /// Generates a random 16 character alphanumeric password.
        /// </summary>
        /// <returns></returns>
        private string _generateRandomPassword()
        {
            var randomPassword = CommonUtilities.GenerateRandomAlphaNumericString(16, true);
            return randomPassword;
        }

        /// <summary>
        /// Generates a hashed password using ASP.NET Core Identity's PasswordHasher.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private string _hashPassword(string password)
        {
            var hasher = new PasswordHasher<object>();
            string passwordHash = hasher.HashPassword(user: null, password: password);
            return passwordHash;
        }

        #region Validation

        private async Task<ErrorValidationResult<IEnumerable<ApplicationUserDto>>> _validateApplicationUserFilter(FilterApplicationUserLogicRequest req)
        {
            ValidationResult result = await _filterApplicationUserLogicRequestValidator.ValidateAsync(req);
            var errorValidationResult = ValidatorUtilities.CreateDefaultValidationResponse<IEnumerable<ApplicationUserDto>>(result);
            return errorValidationResult;
        }

        private async Task<ErrorValidationResult<ApplicationUserDto>> _validateApplicationUserOnInsertUpdate(IApplicationLogic applicationLogic, InsertUpdateApplicationUserRequest req, int? applicationUserId = null)
        {
            ValidationResult result = await _insertUpdateApplicationUserRequestValidator.ValidateAsync(req);
            var errorValidationResult = ValidatorUtilities.CreateDefaultValidationResponse<ApplicationUserDto>(result);

            if (errorValidationResult.Errors.Count == 0)
            {
                // Validate Application exists
                var applicationResponse = await applicationLogic.GetById(req.ApplicationId, new BaseLogicGet { IncludeInactive = true });
                
                if (applicationResponse.Response == null)
                {
                    errorValidationResult.Errors.Add("ApplicationId", new List<string> { ValidatorUtilities.CreateRecordDoesNotExistValidationErrorMessage("ApplicationId") });
                    return errorValidationResult;
                }

                // Validate Application user email is unique
                var emailCheck = await this.Filter(new FilterApplicationUserLogicRequest { Email = req.Email });

                if (emailCheck.Errors.Count == 0 && emailCheck.Response.Count() > 0)
                {
                    if ((applicationUserId == null || applicationUserId == 0) || (emailCheck.Response.FirstOrDefault().ApplicationUserId != applicationUserId))
                    {
                        errorValidationResult.Errors.Add("Email", new List<string> { ValidatorUtilities.CreateUniqueValidationErrorMessage("Email") });
                    }
                }
            }

            return errorValidationResult;
        }

        private ErrorValidationResult<T> _createUserNotFoundError<T>(T? response = default)
        {
            return new ErrorValidationResult<T>
            {
                Response = response,
                Errors = new Dictionary<string, List<string>>
                {
                    { "ApplicationUser", new List<string> { ValidatorUtilities.CreateRecordDoesNotExistValidationErrorMessage("ApplicationUserId") } }
                }
            };
        }

        #endregion

        #endregion

        
    }
}
