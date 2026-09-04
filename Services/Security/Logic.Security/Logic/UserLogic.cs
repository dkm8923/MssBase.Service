using Contract.Security;
using Data.Security;
using Data.Security.Converters;
using Dto.Security.User;
using Dto.Security.User.Logic;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using Shared.Logic;
using Shared.Logic.Validators;
using Shared.Logic.Common;
using Shared.Data.Converters;
using Dto.Security.Authentication;
using Data.Security.Models;
using Microsoft.Extensions.Options;
using Shared.Models.Dtos;
using static Shared.Logic.Common.Constants;
using System.Text.Json;
using Shared.Data.Models;
using Contract.Security.User;
using Dto.Common.CommonRelationalData;

namespace Logic.Security.Logic
{
    public class UserLogic : IUserLogic
    {
        private readonly ISecurityConnectionStrings _connectionStrings;
        private readonly SecurityDBContextFactory _dbContextFactory;

        private IValidator<FilterUserLogicRequest> _filterUserLogicRequestValidator;
        private IValidator<InsertUpdateUserRequest> _insertUpdateUserRequestValidator;
        private IValidator<ChangePasswordRequest> _changePasswordRequestValidator;
        private IOptions<PasswordValidationConfig> _passwordValidationConfig;
        
        public UserLogic(
                            ISecurityConnectionStrings connectionStrings,
                            IValidator<FilterUserLogicRequest> filterUserLogicRequestValidator,
                            IValidator<InsertUpdateUserRequest> insertUpdateUserRequestValidator,
                            IValidator<ChangePasswordRequest> changePasswordRequestValidator,
                            IOptions<PasswordValidationConfig> passwordValidationConfig
        )
        {
            _connectionStrings = connectionStrings;
            _dbContextFactory = new SecurityDBContextFactory(_connectionStrings);
            _filterUserLogicRequestValidator = filterUserLogicRequestValidator;
            _insertUpdateUserRequestValidator = insertUpdateUserRequestValidator;
            _changePasswordRequestValidator = changePasswordRequestValidator;
            _passwordValidationConfig = passwordValidationConfig;
        }

        #region GetAll

        /// <summary>
        /// Retrieves a collection of application users based on the specified request parameters.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<UserDto>>> GetAll(BaseLogicGet req, CancellationToken cancellationToken = default)
        {
            var ret = await this.Filter(new FilterUserLogicRequest { IncludeInactive = req.IncludeInactive, IncludeRelated = req.IncludeRelated, IncludeReadOnly = req.IncludeReadOnly, CurrentUser = req.CurrentUser }, cancellationToken);
            return ret;
        }

        #endregion

        #region GetById

        /// <summary>
        /// Retrieves a user by its unique identifier.
        /// </summary>
        public async Task<ErrorValidationResult<UserDto>> GetById(int userId, BaseLogicGet req, CancellationToken cancellationToken = default)
        {
            var res = await this.Filter(new FilterUserLogicRequest { UserIds = new List<int> { userId }, IncludeInactive = req.IncludeInactive, IncludeRelated = req.IncludeRelated, IncludeReadOnly = req.IncludeReadOnly, CurrentUser = req.CurrentUser }, cancellationToken);

            return new ErrorValidationResult<UserDto> { Response = res.Response.FirstOrDefault() };
        }

        #endregion

        #region GetAuditLogsByUserId

        /// <summary>
        /// Retrieves the audit logs for a user by its unique identifier.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByUserId(int userId, CancellationToken cancellationToken = default)
        {
            using (var dbContext = _dbContextFactory.CreateContextReadOnly())
            {
                var query = dbContext.AuditLogs.AsQueryable().AsNoTracking().Where(al => al.ReferenceType == EntityFieldNames.User && al.ReferenceId == userId);
                return new ErrorValidationResult<IEnumerable<AuditLogDto>> { Response = await query.ToDtos(cancellationToken) };
            }
        }

        #endregion

        #region Filter

        /// <summary>
        /// Filters application users based on the specified criteria.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<UserDto>>> Filter(FilterUserLogicRequest req, CancellationToken cancellationToken = default)
        {
            var errorValidationResult = await _validateUserFilter(req);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadOnly())
            {
                var query = dbContext.Users.AsQueryable().AsNoTracking();
                
                query = query.Include(aul => aul.UserLogin);
                query = query.ApplyIncludeInactiveFilter(req);
                query = query.ApplyIncludeReadOnlyFilter(req);
                query = query.ApplyAuditableFilters(req);

                if (req.IncludeRelated)
                {
                    query = query.Include(user => user.ApplicationUsers
                                       .Where(applicationUser => (req.IncludeInactive || applicationUser.Active) && (req.IncludeReadOnly || !applicationUser.ReadOnly)))
                                 .ThenInclude(applicationUser => applicationUser.ApplicationUserPermissions
                                     .Where(permission => (req.IncludeInactive || permission.Active) && (req.IncludeReadOnly || !permission.ReadOnly)))
                                 .ThenInclude(permission => permission.Permission);
                    
                    query = query.Include(user => user.ApplicationUsers
                                       .Where(applicationUser => (req.IncludeInactive || applicationUser.Active) && (req.IncludeReadOnly || !applicationUser.ReadOnly)))
                                 .ThenInclude(applicationUser => applicationUser.ApplicationUserRoles
                                     .Where(role => (req.IncludeInactive || role.Active) && (req.IncludeReadOnly || !role.ReadOnly)))
                                 .ThenInclude(role => role.Role)
                                 .ThenInclude(role => role.RolePermissions
                                     .Where(permission => (req.IncludeInactive || permission.Active) && (req.IncludeReadOnly || !permission.ReadOnly)))
                                 .ThenInclude(permission => permission.Permission);
                }

                if (req.UserIds != null && req.UserIds.Count > 0)
                {
                    query = query.Where(x => req.UserIds.Contains(x.UserId));
                }
                
                if (req.Email != null)
                {
                    query = query.Where(x => x.Email == req.Email);
                }

                if (req.Title != null)
                {
                    query = query.Where(x => x.Title == req.Title);
                }

                if (req.FirstName != null)
                {
                    query = query.Where(x => x.FirstName == req.FirstName);
                }

                if (req.MiddleName != null)
                {
                    query = query.Where(x => x.MiddleName == req.MiddleName);
                }

                if (req.LastName != null)
                {
                    query = query.Where(x => x.LastName == req.LastName);
                }

                if (req.PreferredName != null)
                {
                    query = query.Where(x => x.PreferredName == req.PreferredName);
                }

                if (req.Suffix != null)
                {
                    query = query.Where(x => x.Suffix == req.Suffix);
                }

                if (req.DateOfBirth != null)
                {
                    query = query.Where(x => x.DateOfBirth == req.DateOfBirth);
                }

                if (req.TimeZone != null)
                {
                    query = query.Where(x => x.TimeZone == req.TimeZone);
                }

                return new ErrorValidationResult<IEnumerable<UserDto>> { Response = await query.ToDtosWithoutPassword(cancellationToken) };
            }
        }

        #endregion

        #region Insert

        /// <summary>
        /// Inserts a new application user into the data store.
        /// </summary>
        public async Task<ErrorValidationResult<UserDto>> Insert(InsertUpdateUserRequest req, FilterCommonRelationalDataDto commonRelationalData)
        {
            var errorValidationResult = await _validateUserOnInsertUpdate(req, commonRelationalData, null);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = req.ToEntityOnInsert();

                var randomPassword = _generateRandomPassword();

                // Assign via navigation property so EF Core fixes up UserId after it's generated on save
                entity.UserLogin = new UserLogin
                {
                    Password = LogicUtilities.HashPassword(randomPassword),
                    PasswordResetRequired = true
                };

                await dbContext.Users.AddAsync(entity);

                await dbContext.SaveChangesAsync();

                entity.UserLogin.Password = randomPassword;

                return new ErrorValidationResult<UserDto> { Response = entity.ToDto() };
            }
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates the details of an existing User.
        /// </summary>
        public async Task<ErrorValidationResult<UserDto>> Update(int userId, InsertUpdateUserRequest req, FilterCommonRelationalDataDto commonRelationalData)
        {
            var errorValidationResult = await _validateUserOnInsertUpdate(req, commonRelationalData, userId);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.Users.FirstOrDefaultAsync(ent => ent.UserId == userId);
                
                if (entity == null)
                {
                    errorValidationResult.Errors = AddRecordNotFoundErrorToErrorValidationResult(errorValidationResult.Errors); 
                    return errorValidationResult;
                }

                if (entity.ReadOnly)
                {
                    return await _returnReadOnlyRecordErrorValidationResult();
                }

                await LogChange(dbContext, entity, req);

                var userLoginEntity = await dbContext.UserLogins.FirstOrDefaultAsync(ent => ent.UserId == userId);
                
                entity = entity.UpdateEntityFromRequest(req);
                
                userLoginEntity.UserId = entity.UserId;

                await dbContext.SaveChangesAsync();
                return new ErrorValidationResult<UserDto> { Response = entity.ToDtoWithoutPassword() };
            }
        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes the user with the specified identifier.
        /// </summary>
        public async Task<ErrorValidationResult> Delete(int userId, string currentUser)
        {
            var errorValidationResult = await _validateUserOnDelete(userId);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.Users.FirstOrDefaultAsync(ent => ent.UserId == userId && !ent.ReadOnly);
                
                if (entity != null)
                {
                    dbContext.UserLogChangePasswords.RemoveRange(dbContext.UserLogChangePasswords.Where(log => log.UserId == userId));
                    dbContext.UserLogLogins.RemoveRange(dbContext.UserLogLogins.Where(log => log.UserId == userId));
                    dbContext.UserRefreshTokens.RemoveRange(dbContext.UserRefreshTokens.Where(token => token.UserId == userId));
                    dbContext.UserLogins.RemoveRange(dbContext.UserLogins.Where(login => login.UserId == userId));

                    await LogDelete(dbContext, entity, currentUser);

                    dbContext.Users.Remove(entity);

                    await dbContext.SaveChangesAsync();
                    
                    return new ErrorValidationResult();
                }
                else
                {
                    return _createUserNotFoundError<object?>();
                }
            }
        }

        #endregion

        #region Password Logic

        /// <summary>
        /// Retrieves the password change history for a specific user by their unique identifier. This includes a list of previous password changes, along with details such as the old password (hashed), the date of the change, and who initiated the change.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A result containing the list of password change history records for the specified user.</returns>
        public async Task<ErrorValidationResult<IEnumerable<UserLogChangePasswordDto>>> GetPasswordChangeHistoryByUserId(int userId, CancellationToken cancellationToken = default)
        {
            using (var dbContext = _dbContextFactory.CreateContextReadOnly())
            {
                var query = dbContext.UserLogChangePasswords.AsQueryable().AsNoTracking().Where(log => log.UserId == userId);
                return new ErrorValidationResult<IEnumerable<UserLogChangePasswordDto>> { Response = await query.ToDtos(cancellationToken) };
            }
        }

        public async Task<ErrorValidationResult<ResetPasswordResponse>> ResetPassword(int userId)
        {
            //TODO: Send email to user with new password instead of returning in response

            var newPassword = _generateRandomPassword();
            
            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.Users.Include(aul => aul.UserLogin).FirstOrDefaultAsync(ent => ent.UserId == userId);
                
                if (entity != null)
                {
                    var currentUser = "UserLogic.ResetPassword";
                    var utcNow = CommonUtilities.GetDateTimeUtcNow();
                    var newHashedPassword = LogicUtilities.HashPassword(newPassword);
                    entity.UserLogin.Password = newHashedPassword;
                    entity.UserLogin.PasswordResetRequired = true;
                    entity.UserLogin.LastPasswordChangeDate = utcNow;

                    //delete any existing refresh tokens when password is changed
                    var userRefreshTokenEntities = await dbContext.UserRefreshTokens.Where(ent => ent.UserId == userId).ToListAsync();
                    dbContext.UserRefreshTokens.RemoveRange(userRefreshTokenEntities);

                    //log password change
                    await dbContext.UserLogChangePasswords.AddAsync(new UserLogChangePassword
                    {
                        UserId = entity.UserId,
                        OldPassword = entity.UserLogin.Password,
                        CreatedBy = currentUser,
                        CreatedOn = utcNow
                    });

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
                var userEntity = await dbContext.Users.Include(aul => aul.UserLogin).FirstOrDefaultAsync(ent => ent.UserId == req.UserId);
                
                if (userEntity is null) 
                {
                    return _createUserNotFoundError<object?>();
                }

                var passwordsMatch = SecurityLogicUtilities.VerifyPasswordMatchesHash(userEntity.UserLogin.Password, req.NewPassword);

                if (passwordsMatch)
                {
                    return new ErrorValidationResult { Errors = new Dictionary<string, List<string>> { { EntityFieldNames.ChangePassword, new List<string> { $"New password must be different from the old password!" } } } };
                }

                //verify new password is not the same as last 5 passwords
                if (_passwordValidationConfig.Value.RequirePasswordHistoryCheck)
                {
                    var oldPasswords = dbContext.UserLogChangePasswords.Where(log => log.UserId == req.UserId)
                    .OrderByDescending(log => log.CreatedOn)
                    .Take(_passwordValidationConfig.Value.RequirePasswordHistoryCheckOldPasswordCount)
                    .Select(log => log.OldPassword)
                    .ToList();

                    if (oldPasswords.Any(oldPassword => SecurityLogicUtilities.VerifyPasswordMatchesHash(oldPassword, req.NewPassword)))
                    {
                        return new ErrorValidationResult { Errors = new Dictionary<string, List<string>> { { EntityFieldNames.ChangePassword, new List<string> { $"New password must be different from the last {_passwordValidationConfig.Value.RequirePasswordHistoryCheckOldPasswordCount} passwords!" } } } };
                    }
                }
                
                var utcNow = CommonUtilities.GetDateTimeUtcNow();

                //log password change
                await dbContext.UserLogChangePasswords.AddAsync(new UserLogChangePassword
                {
                    UserId = req.UserId,
                    OldPassword = userEntity.UserLogin.Password,
                    CreatedBy = req.CurrentUser,
                    CreatedOn = utcNow
                });

                //change password
                userEntity.UserLogin.Password = LogicUtilities.HashPassword(req.NewPassword);
                userEntity.UserLogin.PasswordResetRequired = false;
                userEntity.UserLogin.LastPasswordChangeDate = utcNow;
                
                //delete any existing refresh tokens when password is changed
                var userRefreshTokenEntities = await dbContext.UserRefreshTokens.Where(ent => ent.UserId == userEntity.UserId).ToListAsync();
                dbContext.UserRefreshTokens.RemoveRange(userRefreshTokenEntities);

                await dbContext.SaveChangesAsync();

                return new ErrorValidationResult();
            }
        }

        #endregion

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

        #endregion

        #region Validation

        private async Task<ErrorValidationResult<IEnumerable<UserDto>>> _validateUserFilter(FilterUserLogicRequest req)
        {
            ValidationResult result = await _filterUserLogicRequestValidator.ValidateAsync(req);
            var errorValidationResult = ValidatorUtilities.CreateDefaultValidationResponse<IEnumerable<UserDto>>(result);
            return errorValidationResult;
        }

        private async Task<ErrorValidationResult<UserDto>> _validateUserOnInsertUpdate(InsertUpdateUserRequest req, FilterCommonRelationalDataDto commonRelationalData, int? userId = null)
        {
            ValidationResult result = await _insertUpdateUserRequestValidator.ValidateAsync(req);
            var errorValidationResult = ValidatorUtilities.CreateDefaultValidationResponse<UserDto>(result);

            if (errorValidationResult.Errors.Count == 0)
            {
                //validate common relational data fields have valid values for Title / Suffix / TimeZone
                errorValidationResult = CommonLogicUtilities.ValidateCommonRelationalDataNameIsValid(commonRelationalData.PersonTitle, req.Title, EntityFieldNames.Title, CommonRelationalDataReferenceTypes.PersonTitle, errorValidationResult);
                errorValidationResult = CommonLogicUtilities.ValidateCommonRelationalDataNameIsValid(commonRelationalData.PersonSuffix, req.Suffix, EntityFieldNames.Suffix, CommonRelationalDataReferenceTypes.PersonSuffix, errorValidationResult);
                errorValidationResult = CommonLogicUtilities.ValidateCommonRelationalDataValueIsValid(commonRelationalData.UsaTimeZone, req.TimeZone, EntityFieldNames.TimeZone, CommonRelationalDataReferenceTypes.UsaTimeZone, errorValidationResult);
                
                // Validate user email is unique
                var emailCheck = await this.Filter(new FilterUserLogicRequest { Email = req.Email, IncludeReadOnly = true });

                if (emailCheck.Errors.Count == 0 && emailCheck.Response.Count() > 0)
                {
                    if ((userId == null || userId == 0) || (emailCheck.Response.FirstOrDefault().UserId != userId))
                    {
                        errorValidationResult.Errors.Add(EntityFieldNames.Email, new List<string> { ValidatorUtilities.CreateUniqueValidationErrorMessage(EntityFieldNames.Email) });
                    }
                }
            }

            return errorValidationResult;
        }

        private async Task<ErrorValidationResult<UserDto>> _validateUserOnDelete(int userId)
        {
            var userErrorValidationResult = await GetById(userId, new BaseLogicGet { IncludeInactive = true, IncludeRelated = true, IncludeReadOnly = true });

            if (userErrorValidationResult.Response == null)
            {
                //application user for given id does not exist
                return _createUserNotFoundError<UserDto>();
            }

            if (userErrorValidationResult.Response.ReadOnly)
            {
                return await _returnReadOnlyRecordErrorValidationResult();
            }

            //verify no dependencies exist on application user record
            if (userErrorValidationResult.Response.ApplicationUsers.NotNullAndHasRecords())
            {
                userErrorValidationResult.Errors.Add(EntityFieldNames.ApplicationUsers, new List<string> { ValidatorUtilities.CreateDependencyExistsValidationErrorMessage(EntityFieldNames.ApplicationUsers) });
            }

            return userErrorValidationResult;
        }

        private ErrorValidationResult<T> _createUserNotFoundError<T>(T? response = default)
        {
            return new ErrorValidationResult<T>
            {
                Response = response,
                Errors = new Dictionary<string, List<string>>
                {
                    { EntityFieldNames.User, new List<string> { ValidatorUtilities.CreateRecordDoesNotExistValidationErrorMessage(EntityFieldNames.UserId) } }
                }
            };
        }

        private Dictionary<string, List<string>> AddRecordNotFoundErrorToErrorValidationResult(Dictionary<string, List<string>> errors)
        {
            return LogicUtilities.AddRecordNotFoundErrorToErrorValidationResult(errors, EntityFieldNames.User, EntityFieldNames.UserId);
        }

        private async Task<ErrorValidationResult<UserDto>> _returnReadOnlyRecordErrorValidationResult()
        {
            var errorValidationResult = new ErrorValidationResult<UserDto>();
            errorValidationResult.Errors.Add(EntityFieldNames.User, new List<string> { ValidatorUtilities.CreateRecordIsReadOnlyValidationErrorMessage() });
            return errorValidationResult;
        }

        #endregion

        #region Audit Log

        private async Task LogChange(SecurityDBContext dbContext, User oldRecord, InsertUpdateUserRequest req) 
        {
            var newRecord = req.ToEntityOnInsert();
            
            // Only capture fields that actually changed, not the full entity graph
            var changeLog = new Dictionary<string, object?>();

            if (oldRecord.Email != newRecord.Email)
            {
                changeLog[nameof(User.Email)] = newRecord.Email;
            }

            if (oldRecord.Title != newRecord.Title)
            {
                changeLog[nameof(User.Title)] = newRecord.Title;
            }
            
            if (oldRecord.FirstName != newRecord.FirstName)
            {
                changeLog[nameof(User.FirstName)] = newRecord.FirstName;
            }

            if (oldRecord.MiddleName != newRecord.MiddleName)
            {
                changeLog[nameof(User.MiddleName)] = newRecord.MiddleName;
            }
            
            if (oldRecord.LastName != newRecord.LastName)
            {
                changeLog[nameof(User.LastName)] = newRecord.LastName;
            }

            if (oldRecord.PreferredName != newRecord.PreferredName)
            {
                changeLog[nameof(User.PreferredName)] = newRecord.PreferredName;
            }
            if (oldRecord.Suffix != newRecord.Suffix)
            {
                changeLog[nameof(User.Suffix)] = newRecord.Suffix;
            }

            if (oldRecord.DateOfBirth != newRecord.DateOfBirth)
            {
                changeLog[nameof(User.DateOfBirth)] = newRecord.DateOfBirth;
            }

            if (oldRecord.TimeZone != newRecord.TimeZone)
            {
                changeLog[nameof(User.TimeZone)] = newRecord.TimeZone;
            }

            if (oldRecord.Active != newRecord.Active)
            {
                changeLog[nameof(User.Active)] = newRecord.Active;
            }
            
            if (oldRecord.UpdatedBy != req.CurrentUser)
            {
                changeLog[nameof(User.UpdatedBy)] = req.CurrentUser;
            }
            
            changeLog[nameof(User.UpdatedOn)] = oldRecord.UpdatedOn;
            
            await dbContext.AuditLogs.AddAsync(new AuditLog {
                LogType = AuditLogLogTypes.Update,
                ReferenceType = EntityFieldNames.User,
                ReferenceId = oldRecord.UserId,
                ChangeLogJson = JsonSerializer.Serialize(changeLog),
                RecordStateBeforeChangeJson = GetRecordStateBeforeChangeJson(oldRecord),
                CreatedBy = req.CurrentUser,
                CreatedOn = CommonUtilities.GetDateTimeUtcNow()
            });
        }

        private async Task LogDelete(SecurityDBContext dbContext, User record, string currentUser) 
        {
            await dbContext.AuditLogs.AddAsync(new AuditLog {
                LogType = AuditLogLogTypes.Delete,
                ReferenceType = EntityFieldNames.User,
                ReferenceId = record.UserId,
                ChangeLogJson = JsonSerializer.Serialize(new {}),
                RecordStateBeforeChangeJson = GetRecordStateBeforeChangeJson(record),
                CreatedBy = currentUser,
                CreatedOn = CommonUtilities.GetDateTimeUtcNow()
            });
        }

        private string GetRecordStateBeforeChangeJson(User record)
        {
            var log = new Dictionary<string, object?>();
            log[nameof(User.Email)] = record.Email;
            log[nameof(User.Title)] = record.Title;
            log[nameof(User.FirstName)] = record.FirstName;
            log[nameof(User.MiddleName)] = record.MiddleName;
            log[nameof(User.LastName)] = record.LastName;
            log[nameof(User.PreferredName)] = record.PreferredName;
            log[nameof(User.Suffix)] = record.Suffix;
            log[nameof(User.DateOfBirth)] = record.DateOfBirth;
            log[nameof(User.TimeZone)] = record.TimeZone;
            log[nameof(User.Active)] = record.Active;
            log[nameof(User.ReadOnly)] = record.ReadOnly;
            log[nameof(User.CreatedBy)] = record.CreatedBy;
            log[nameof(User.CreatedOn)] = record.CreatedOn;
            log[nameof(User.UpdatedBy)] = record.UpdatedBy;
            log[nameof(User.UpdatedOn)] = record.UpdatedOn;
            
            return JsonSerializer.Serialize(log);
        }

        #endregion
    }
}
