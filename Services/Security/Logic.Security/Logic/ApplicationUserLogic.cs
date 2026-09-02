using Contract.Security;
using Contract.Security.Application;
using Contract.Security.ApplicationUser;
using Contract.Security.Permission;
using Data.Security;
using Data.Security.Converters;
using Dto.Security.ApplicationUser.Logic;
using Dto.Security.ApplicationUser;
using Dto.Security.Permission.Logic;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using Shared.Logic;
using Shared.Logic.Validators;
using Shared.Logic.Common;
using Shared.Models.Dtos;
using static Shared.Logic.Common.Constants;
using Shared.Data.Converters;
using Data.Security.Models;
using System.Text.Json;
using Shared.Data.Models;
using Contract.Security.User;
using Dto.Security.User.Logic;

namespace Logic.Security.Logic
{
    public class ApplicationUserLogic : IApplicationUserLogic
    {
        private readonly ISecurityConnectionStrings _connectionStrings;
        private readonly SecurityDBContextFactory _dbContextFactory;

        private IValidator<FilterApplicationUserLogicRequest> _filterApplicationUserLogicRequestValidator;
        private IValidator<InsertUpdateApplicationUserRequest> _insertUpdateApplicationUserRequestValidator;

        public ApplicationUserLogic(
                            ISecurityConnectionStrings connectionStrings,
                            IValidator<FilterApplicationUserLogicRequest> filterApplicationUserLogicRequestValidator,
                            IValidator<InsertUpdateApplicationUserRequest> insertUpdateApplicationUserRequestValidator
        )
        {
            _connectionStrings = connectionStrings;
            _dbContextFactory = new SecurityDBContextFactory(_connectionStrings);
            _filterApplicationUserLogicRequestValidator = filterApplicationUserLogicRequestValidator;
            _insertUpdateApplicationUserRequestValidator = insertUpdateApplicationUserRequestValidator;
        }

        #region GetAll

        /// <summary>
        /// Retrieves a collection of application users based on the specified request parameters.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<ApplicationUserDto>>> GetAll(BaseLogicGet req, CancellationToken cancellationToken = default)
        {
            var ret = await this.Filter(new FilterApplicationUserLogicRequest { IncludeInactive = req.IncludeInactive, IncludeRelated = req.IncludeRelated, IncludeReadOnly = req.IncludeReadOnly, CurrentUser = req.CurrentUser }, cancellationToken);
            return ret;
        }

        #endregion

        #region GetById

        /// <summary>
        /// Retrieves an application user by its unique identifier.
        /// </summary>
        public async Task<ErrorValidationResult<ApplicationUserDto>> GetById(int applicationUserId, BaseLogicGet req, CancellationToken cancellationToken = default)
        {
            var res = await this.Filter(new FilterApplicationUserLogicRequest { ApplicationUserIds = new List<int> { applicationUserId }, IncludeInactive = req.IncludeInactive, IncludeRelated = req.IncludeRelated, IncludeReadOnly = req.IncludeReadOnly, CurrentUser = req.CurrentUser });

            return new ErrorValidationResult<ApplicationUserDto> { Response = res.Response.FirstOrDefault() };
        }

        #endregion

        #region GetAuditLogsByApplicationUserId

        /// <summary>
        /// Retrieves the audit logs for an application user permission by its unique identifier.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByApplicationUserId(int applicationUserId, CancellationToken cancellationToken = default)
        {
            using (var dbContext = _dbContextFactory.CreateContextReadOnly())
            {
                var query = dbContext.AuditLogs.AsQueryable().AsNoTracking().Where(al => al.ReferenceType == EntityFieldNames.ApplicationUser && al.ReferenceId == applicationUserId);
                return new ErrorValidationResult<IEnumerable<AuditLogDto>> { Response = await query.ToDtos(cancellationToken) };
            }
        }

        #endregion

        #region Filter

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
                query = query.ApplyIncludeReadOnlyFilter(req);
                query = query.ApplyAuditableFilters(req);

                if (req.IncludeRelated)
                {
                    query = query.Include(applicationUser => applicationUser.ApplicationUserPermissions
                                       .Where(permission => (req.IncludeInactive || permission.Active) && (req.IncludeReadOnly || !permission.ReadOnly)))
                                 .ThenInclude(permission => permission.Permission);
                    
                    query = query.Include(applicationUser => applicationUser.ApplicationUserRoles
                                       .Where(role => (req.IncludeInactive || role.Active) && (req.IncludeReadOnly || !role.ReadOnly)))
                                 .ThenInclude(role => role.Role)
                                 .ThenInclude(role => role.RolePermissions)
                                 .ThenInclude(rolePermission => rolePermission.Permission);
                }

                if (req.ApplicationUserIds != null && req.ApplicationUserIds.Count > 0)
                {
                    query = query.Where(x => req.ApplicationUserIds.Contains(x.ApplicationUserId));
                }
                
                if (req.ApplicationId != null)
                {
                    query = query.Where(x => x.ApplicationId == req.ApplicationId);
                }

                if (req.UserId != null)
                {
                    query = query.Where(x => x.UserId == req.UserId);
                }

                return new ErrorValidationResult<IEnumerable<ApplicationUserDto>> { Response = await query.ToDtos(cancellationToken) };
            }
        }

        #endregion

        #region Insert

        /// <summary>
        /// Inserts a new application user into the data store.
        /// </summary>
        public async Task<ErrorValidationResult<ApplicationUserDto>> Insert(InsertUpdateApplicationUserRequest req, 
                                                                                      IApplicationLogic applicationLogic,
                                                                                      IApplicationUserLogic applicationUserLogic,
                                                                                      IUserLogic userLogic
                                                                                     )
        {
            var errorValidationResult = await _validateApplicationUserOnInsertUpdate(applicationLogic, applicationUserLogic, userLogic, req);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = req.ToEntityOnInsert();

                await dbContext.ApplicationUsers.AddAsync(entity);
                await dbContext.SaveChangesAsync();

                return new ErrorValidationResult<ApplicationUserDto> { Response = entity.ToDto() };
            }
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates the details of an existing application user.
        /// </summary>
        public async Task<ErrorValidationResult<ApplicationUserDto>> Update(int applicationUserId, 
                                                                                      InsertUpdateApplicationUserRequest req, 
                                                                                      IApplicationLogic applicationLogic,
                                                                                      IApplicationUserLogic applicationUserLogic,
                                                                                      IUserLogic userLogic
                                                                                     )
        {
            var errorValidationResult = await _validateApplicationUserOnInsertUpdate(applicationLogic, applicationUserLogic, userLogic, req, applicationUserId);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.ApplicationUsers.FirstOrDefaultAsync(ent => ent.ApplicationUserId == applicationUserId);

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

                entity = entity.UpdateEntityFromRequest(req);
                await dbContext.SaveChangesAsync();
                return new ErrorValidationResult<ApplicationUserDto> { Response = entity.ToDto() };
            }
        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes the application user with the specified identifier.
        /// </summary>
        public async Task<ErrorValidationResult> Delete(int applicationUserId, string currentUser)
        {
            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.ApplicationUsers.FirstOrDefaultAsync(ent => ent.ApplicationUserId == applicationUserId);
                var errorValidationResult = new ErrorValidationResult();

                if (entity != null)
                {
                    if (entity.ReadOnly)
                    {
                        return await _returnReadOnlyRecordErrorValidationResult();
                    }

                    await LogDelete(dbContext, entity, currentUser);

                    dbContext.ApplicationUsers.Remove(entity);

                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    errorValidationResult.Errors = AddRecordNotFoundErrorToErrorValidationResult(errorValidationResult.Errors);
                }

                return errorValidationResult;
            }
        }

        #endregion

        #region Validation

        private async Task<ErrorValidationResult<IEnumerable<ApplicationUserDto>>> _validateApplicationUserFilter(FilterApplicationUserLogicRequest req)
        {
            ValidationResult result = await _filterApplicationUserLogicRequestValidator.ValidateAsync(req);
            var errorValidationResult = ValidatorUtilities.CreateDefaultValidationResponse<IEnumerable<ApplicationUserDto>>(result);
            return errorValidationResult;
        }

        private async Task<ErrorValidationResult<ApplicationUserDto>> _validateApplicationUserOnInsertUpdate(IApplicationLogic applicationLogic,
                                                                                                             IApplicationUserLogic applicationUserLogic,
                                                                                                             IUserLogic userLogic,         
                                                                                                             InsertUpdateApplicationUserRequest req,
                                                                                                             int? applicationUserId = null
                                                                                                            )
        {
            ValidationResult result = await _insertUpdateApplicationUserRequestValidator.ValidateAsync(req);
            var errorValidationResult = ValidatorUtilities.CreateDefaultValidationResponse<ApplicationUserDto>(result);

            if (errorValidationResult.Errors.Count == 0)
            {
                // Validate Application exists
                var applicationResponse = await applicationLogic.GetById(req.ApplicationId, new BaseLogicGet { IncludeInactive = true, IncludeReadOnly = true });
                
                if (applicationResponse.Response == null)
                {
                    errorValidationResult.Errors.Add(Constants.EntityFieldNames.ApplicationId, new List<string> { ValidatorUtilities.CreateRecordDoesNotExistValidationErrorMessage(Constants.EntityFieldNames.ApplicationId) });
                    return errorValidationResult;
                }

                // Validate User exists
                var userResponse = await userLogic.Filter(new FilterUserLogicRequest { UserIds = new List<int> { req.UserId }, ApplicationId = req.ApplicationId, IncludeInactive = true, IncludeReadOnly = true });

                if (userResponse.Response == null || userResponse.Response.Count() == 0)
                {
                    errorValidationResult.Errors.Add(Constants.EntityFieldNames.UserId, new List<string> { ValidatorUtilities.CreateRecordDoesNotExistValidationErrorMessage(Constants.EntityFieldNames.UserId) });
                    return errorValidationResult;
                }

                // Validate ApplicationUser is unique
                var uniqueApplicationUserCheck = await this.Filter(new FilterApplicationUserLogicRequest { 
                    ApplicationId = req.ApplicationId, 
                    UserId = req.UserId, 
                    IncludeInactive = true,
                    IncludeReadOnly = true 
                });

                if (uniqueApplicationUserCheck.Errors.Count == 0 && uniqueApplicationUserCheck.Response.Count() > 0)
                {
                    if ((applicationUserId == null || applicationUserId == 0) || (uniqueApplicationUserCheck.Response.FirstOrDefault().ApplicationUserId != applicationUserId))
                    {
                        errorValidationResult.Errors.Add(Constants.EntityFieldNames.ApplicationUser, new List<string> { ValidatorUtilities.CreateUniqueValidationErrorMessage(Constants.EntityFieldNames.ApplicationUser) });
                    }
                }
            }

            return errorValidationResult;
        }

        private Dictionary<string, List<string>> AddRecordNotFoundErrorToErrorValidationResult(Dictionary<string, List<string>> errors)
        {
            return LogicUtilities.AddRecordNotFoundErrorToErrorValidationResult(errors, Constants.EntityFieldNames.ApplicationUser, Constants.EntityFieldNames.ApplicationUserId);
        }

        private async Task<ErrorValidationResult<ApplicationUserDto>> _returnReadOnlyRecordErrorValidationResult()
        {
            var errorValidationResult = new ErrorValidationResult<ApplicationUserDto>();
            errorValidationResult.Errors.Add(Constants.EntityFieldNames.ApplicationUser, new List<string> { ValidatorUtilities.CreateRecordIsReadOnlyValidationErrorMessage() });
            return errorValidationResult;
        }

        #endregion

        #region Audit Log

        private async Task LogChange(SecurityDBContext dbContext, ApplicationUser oldRecord, InsertUpdateApplicationUserRequest req) 
        {
            var newRecord = req.ToEntityOnInsert();
            
            // Only capture fields that actually changed, not the full entity graph
            var changeLog = new Dictionary<string, object?>();

            if (oldRecord.ApplicationId != newRecord.ApplicationId)
            {
                changeLog[nameof(ApplicationUser.ApplicationId)] = newRecord.ApplicationId;
            }
            
            if (oldRecord.ApplicationUserId != newRecord.ApplicationUserId)
            {
                changeLog[nameof(ApplicationUser.ApplicationUserId)] = newRecord.ApplicationUserId;
            }

            if (oldRecord.UserId != newRecord.UserId)
            {
                changeLog[nameof(ApplicationUser.UserId)] = newRecord.UserId;
            }

            if (oldRecord.Active != newRecord.Active)
            {
                changeLog[nameof(ApplicationUser.Active)] = newRecord.Active;
            }

            if (oldRecord.UpdatedBy != req.CurrentUser)
            {
                changeLog[nameof(ApplicationUser.UpdatedBy)] = req.CurrentUser;
            }
            
            changeLog[nameof(ApplicationUser.UpdatedOn)] = oldRecord.UpdatedOn;
            
            await dbContext.AuditLogs.AddAsync(new AuditLog {
                LogType = AuditLogLogTypes.Update,
                ReferenceType = EntityFieldNames.ApplicationUser,
                ReferenceId = oldRecord.ApplicationUserId,
                ChangeLogJson = JsonSerializer.Serialize(changeLog),
                RecordStateBeforeChangeJson = GetRecordStateBeforeChangeJson(oldRecord),
                CreatedBy = req.CurrentUser,
                CreatedOn = CommonUtilities.GetDateTimeUtcNow()
            });
        }

        private async Task LogDelete(SecurityDBContext dbContext, ApplicationUser record, string currentUser) 
        {
            await dbContext.AuditLogs.AddAsync(new AuditLog {
                LogType = AuditLogLogTypes.Delete,
                ReferenceType = EntityFieldNames.ApplicationUser,
                ReferenceId = record.ApplicationUserId,
                ChangeLogJson = JsonSerializer.Serialize(new {}),
                RecordStateBeforeChangeJson = GetRecordStateBeforeChangeJson(record),
                CreatedBy = currentUser,
                CreatedOn = CommonUtilities.GetDateTimeUtcNow()
            });
        }

        private string GetRecordStateBeforeChangeJson(ApplicationUser record)
        {
            var log = new Dictionary<string, object?>();
            log[nameof(ApplicationUser.ApplicationId)] = record.ApplicationId;
            log[nameof(ApplicationUser.ApplicationUserId)] = record.ApplicationUserId;
            log[nameof(ApplicationUser.UserId)] = record.UserId;
            log[nameof(ApplicationUser.Active)] = record.Active;
            log[nameof(ApplicationUser.CreatedBy)] = record.CreatedBy;
            log[nameof(ApplicationUser.CreatedOn)] = record.CreatedOn;
            log[nameof(ApplicationUser.UpdatedBy)] = record.UpdatedBy;
            log[nameof(ApplicationUser.UpdatedOn)] = record.UpdatedOn;
            
            return JsonSerializer.Serialize(log);
        }

        #endregion
    }
}
