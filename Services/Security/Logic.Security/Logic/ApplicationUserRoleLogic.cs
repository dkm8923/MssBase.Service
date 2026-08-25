using Contract.Security;
using Contract.Security.Application;
using Contract.Security.ApplicationUser;
using Contract.Security.ApplicationUserRole;
using Contract.Security.Role;
using Data.Security;
using Data.Security.Converters;
using Dto.Security.ApplicationUser.Logic;
using Dto.Security.ApplicationUserRole;
using Dto.Security.ApplicationUserRole.Logic;
using Dto.Security.Role.Logic;
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
using Shared.Data.Models;
using System.Text.Json;

namespace Logic.Security.Logic
{
    public class ApplicationUserRoleLogic : IApplicationUserRoleLogic
    {
        private readonly ISecurityConnectionStrings _connectionStrings;
        private readonly SecurityDBContextFactory _dbContextFactory;

        private IValidator<FilterApplicationUserRoleLogicRequest> _filterApplicationUserRoleLogicRequestValidator;
        private IValidator<InsertUpdateApplicationUserRoleRequest> _insertUpdateApplicationUserRoleRequestValidator;

        public ApplicationUserRoleLogic(
                            ISecurityConnectionStrings connectionStrings,
                            IValidator<FilterApplicationUserRoleLogicRequest> filterApplicationUserRoleLogicRequestValidator,
                            IValidator<InsertUpdateApplicationUserRoleRequest> insertUpdateApplicationUserRoleRequestValidator
        )
        {
            _connectionStrings = connectionStrings;
            _dbContextFactory = new SecurityDBContextFactory(_connectionStrings);
            _filterApplicationUserRoleLogicRequestValidator = filterApplicationUserRoleLogicRequestValidator;
            _insertUpdateApplicationUserRoleRequestValidator = insertUpdateApplicationUserRoleRequestValidator;
        }

        #region GetAll

        /// <summary>
        /// Retrieves a collection of application users based on the specified request parameters.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<ApplicationUserRoleDto>>> GetAll(BaseLogicGet req, CancellationToken cancellationToken = default)
        {
            var ret = await this.Filter(new FilterApplicationUserRoleLogicRequest { IncludeInactive = req.IncludeInactive, IncludeRelated = req.IncludeRelated, IncludeReadOnly = req.IncludeReadOnly, CurrentUser = req.CurrentUser }, cancellationToken);
            return ret;
        }

        #endregion

        #region GetById

        /// <summary>
        /// Retrieves an application user by its unique identifier.
        /// </summary>
        public async Task<ErrorValidationResult<ApplicationUserRoleDto>> GetById(int applicationUserRoleId, BaseLogicGet req, CancellationToken cancellationToken = default)
        {
            var res = await this.Filter(new FilterApplicationUserRoleLogicRequest { ApplicationUserRoleIds = new List<int> { applicationUserRoleId }, IncludeInactive = req.IncludeInactive, IncludeRelated = req.IncludeRelated, IncludeReadOnly = req.IncludeReadOnly, CurrentUser = req.CurrentUser }, cancellationToken);

            return new ErrorValidationResult<ApplicationUserRoleDto> { Response = res.Response.FirstOrDefault() };
        }

        #endregion

        #region GetAuditLogsByApplicationUserRoleId

        /// <summary>
        /// Retrieves the audit logs for an application user role by its unique identifier.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByApplicationUserRoleId(int applicationUserRoleId, CancellationToken cancellationToken = default)
        {
            using (var dbContext = _dbContextFactory.CreateContextReadOnly())
            {
                var query = dbContext.AuditLogs.AsQueryable().AsNoTracking().Where(al => al.ReferenceType == EntityFieldNames.ApplicationUserRole && al.ReferenceId == applicationUserRoleId);
                return new ErrorValidationResult<IEnumerable<AuditLogDto>> { Response = await query.ToDtos(cancellationToken) };
            }
        }

        #endregion

        #region Filter

        /// <summary>
        /// Filters application users based on the specified criteria.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<ApplicationUserRoleDto>>> Filter(FilterApplicationUserRoleLogicRequest req, CancellationToken cancellationToken = default)
        {
            var errorValidationResult = await _validateApplicationUserRoleFilter(req);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadOnly())
            {
                var query = dbContext.ApplicationUserRoles.AsQueryable().AsNoTracking();

                query = query.ApplyIncludeInactiveFilter(req);
                query = query.ApplyIncludeReadOnlyFilter(req);
                query = query.ApplyAuditableFilters(req);

                if (req.IncludeRelated)
                {
                    query = query.Include(aur => aur.Role).Where(aur => req.IncludeInactive || aur.Active);
                }

                if (req.ApplicationUserRoleIds != null && req.ApplicationUserRoleIds.Count > 0)
                {
                    query = query.Where(x => req.ApplicationUserRoleIds.Contains(x.ApplicationUserRoleId));
                }
                
                if (req.ApplicationId != null)
                {
                    query = query.Where(x => x.ApplicationId == req.ApplicationId);
                }

                if (req.ApplicationUserId != null)
                {
                    query = query.Where(x => x.ApplicationUserId == req.ApplicationUserId);
                }

                if (req.RoleId != null)
                {
                    query = query.Where(x => x.RoleId == req.RoleId);
                }

                return new ErrorValidationResult<IEnumerable<ApplicationUserRoleDto>> { Response = await query.ToDtos(cancellationToken) };
            }
        }

        #endregion

        #region Insert

        /// <summary>
        /// Inserts a new application user into the data store.
        /// </summary>
        public async Task<ErrorValidationResult<ApplicationUserRoleDto>> Insert(InsertUpdateApplicationUserRoleRequest req, 
                                                                                      IApplicationLogic applicationLogic,
                                                                                      IApplicationUserLogic applicationUserLogic,
                                                                                      IRoleLogic roleLogic
                                                                                     )
        {
            var errorValidationResult = await _validateApplicationUserRoleOnInsertUpdate(applicationLogic, applicationUserLogic, roleLogic, req);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = req.ToEntityOnInsert();

                await dbContext.ApplicationUserRoles.AddAsync(entity);
                await dbContext.SaveChangesAsync();

                return new ErrorValidationResult<ApplicationUserRoleDto> { Response = entity.ToDto() };
            }
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates the details of an existing application user.
        /// </summary>
        public async Task<ErrorValidationResult<ApplicationUserRoleDto>> Update(int applicationUserRoleId, 
                                                                                      InsertUpdateApplicationUserRoleRequest req, 
                                                                                      IApplicationLogic applicationLogic,
                                                                                      IApplicationUserLogic applicationUserLogic,
                                                                                      IRoleLogic roleLogic
                                                                                     )
        {
            var errorValidationResult = await _validateApplicationUserRoleOnInsertUpdate(applicationLogic, applicationUserLogic, roleLogic, req, applicationUserRoleId);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.ApplicationUserRoles.FirstOrDefaultAsync(ent => ent.ApplicationUserRoleId == applicationUserRoleId);

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
                return new ErrorValidationResult<ApplicationUserRoleDto> { Response = entity.ToDto() };
            }
        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes the application user with the specified identifier.
        /// </summary>
        public async Task<ErrorValidationResult> Delete(int applicationUserRoleId, string currentUser)
        {
            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.ApplicationUserRoles.FirstOrDefaultAsync(ent => ent.ApplicationUserRoleId == applicationUserRoleId);
                var errorValidationResult = new ErrorValidationResult();

                if (entity != null)
                {
                    if (entity.ReadOnly)
                    {
                        return await _returnReadOnlyRecordErrorValidationResult();
                    }

                    await LogDelete(dbContext, entity, currentUser);

                    dbContext.ApplicationUserRoles.Remove(entity);

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

        private async Task<ErrorValidationResult<IEnumerable<ApplicationUserRoleDto>>> _validateApplicationUserRoleFilter(FilterApplicationUserRoleLogicRequest req)
        {
            ValidationResult result = await _filterApplicationUserRoleLogicRequestValidator.ValidateAsync(req);
            var errorValidationResult = ValidatorUtilities.CreateDefaultValidationResponse<IEnumerable<ApplicationUserRoleDto>>(result);
            return errorValidationResult;
        }

        private async Task<ErrorValidationResult<ApplicationUserRoleDto>> _validateApplicationUserRoleOnInsertUpdate(IApplicationLogic applicationLogic,
                                                                                                                                 IApplicationUserLogic applicationUserLogic,
                                                                                                                                 IRoleLogic roleLogic,         
                                                                                                                                 InsertUpdateApplicationUserRoleRequest req,
                                                                                                                                 int? applicationUserRoleId = null
                                                                                                                                )
        {
            ValidationResult result = await _insertUpdateApplicationUserRoleRequestValidator.ValidateAsync(req);
            var errorValidationResult = ValidatorUtilities.CreateDefaultValidationResponse<ApplicationUserRoleDto>(result);

            if (errorValidationResult.Errors.Count == 0)
            {
                // Validate Application exists
                var applicationResponse = await applicationLogic.GetById(req.ApplicationId, new BaseLogicGet { IncludeInactive = true, IncludeReadOnly = true });
                
                if (applicationResponse.Response == null)
                {
                    errorValidationResult.Errors.Add(Constants.EntityFieldNames.ApplicationId, new List<string> { ValidatorUtilities.CreateRecordDoesNotExistValidationErrorMessage(Constants.EntityFieldNames.ApplicationId) });
                    return errorValidationResult;
                }

                // Validate ApplicationUser exists
                var applicationUserResponse = await applicationUserLogic.Filter(new FilterApplicationUserLogicRequest { ApplicationUserIds = new List<int> { req.ApplicationUserId }, ApplicationId = req.ApplicationId, IncludeInactive = true, IncludeReadOnly = true });

                if (applicationUserResponse.Response == null || applicationUserResponse.Response.Count() == 0)
                {
                    errorValidationResult.Errors.Add(Constants.EntityFieldNames.ApplicationUserId, new List<string> { ValidatorUtilities.CreateRecordDoesNotExistValidationErrorMessage(Constants.EntityFieldNames.ApplicationUserId) });
                    return errorValidationResult;
                }

                // Validate Role exists
                var roleResponse = await roleLogic.Filter(new FilterRoleLogicRequest { RoleIds = new List<int> { req.RoleId }, ApplicationId = req.ApplicationId, IncludeInactive = true, IncludeReadOnly = true });

                if (roleResponse.Response == null || roleResponse.Response.Count() == 0)
                {
                    errorValidationResult.Errors.Add(Constants.EntityFieldNames.RoleId, new List<string> { ValidatorUtilities.CreateRecordDoesNotExistValidationErrorMessage(Constants.EntityFieldNames.RoleId) });
                    return errorValidationResult;
                }

                // Validate ApplicationUserRole is unique
                var uniqueApplicationUserRoleCheck = await this.Filter(new FilterApplicationUserRoleLogicRequest { 
                    ApplicationId = req.ApplicationId, 
                    ApplicationUserId = req.ApplicationUserId, 
                    RoleId = req.RoleId, 
                    IncludeInactive = true,
                    IncludeReadOnly = true 
                });

                if (uniqueApplicationUserRoleCheck.Errors.Count == 0 && uniqueApplicationUserRoleCheck.Response.Count() > 0)
                {
                    if ((applicationUserRoleId == null || applicationUserRoleId == 0) || (uniqueApplicationUserRoleCheck.Response.FirstOrDefault().ApplicationUserRoleId != applicationUserRoleId))
                    {
                        errorValidationResult.Errors.Add(Constants.EntityFieldNames.ApplicationUserRole, new List<string> { ValidatorUtilities.CreateUniqueValidationErrorMessage(Constants.EntityFieldNames.ApplicationUserRole) });
                    }
                }
            }

            return errorValidationResult;
        }

        private Dictionary<string, List<string>> AddRecordNotFoundErrorToErrorValidationResult(Dictionary<string, List<string>> errors)
        {
            return LogicUtilities.AddRecordNotFoundErrorToErrorValidationResult(errors, Constants.EntityFieldNames.ApplicationUserRole, Constants.EntityFieldNames.ApplicationUserRoleId);
        }

        private async Task<ErrorValidationResult<ApplicationUserRoleDto>> _returnReadOnlyRecordErrorValidationResult()
        {
            var errorValidationResult = new ErrorValidationResult<ApplicationUserRoleDto>();
            errorValidationResult.Errors.Add(Constants.EntityFieldNames.ApplicationUserRole, new List<string> { ValidatorUtilities.CreateRecordIsReadOnlyValidationErrorMessage() });
            return errorValidationResult;
        }

        #endregion

        #region Audit Log

        private async Task LogChange(SecurityDBContext dbContext, ApplicationUserRole oldRecord, InsertUpdateApplicationUserRoleRequest req) 
        {
            var newRecord = req.ToEntityOnInsert();
            
            // Only capture fields that actually changed, not the full entity graph
            var changeLog = new Dictionary<string, object?>();

            if (oldRecord.ApplicationId != newRecord.ApplicationId)
            {
                changeLog[nameof(ApplicationUserRole.ApplicationId)] = newRecord.ApplicationId;
            }
            
            if (oldRecord.ApplicationUserId != newRecord.ApplicationUserId)
            {
                changeLog[nameof(ApplicationUserRole.ApplicationUserId)] = newRecord.ApplicationUserId;
            }

            if (oldRecord.RoleId != newRecord.RoleId)
            {
                changeLog[nameof(ApplicationUserRole.RoleId)] = newRecord.RoleId;
            }

            if (oldRecord.Active != newRecord.Active)
            {
                changeLog[nameof(ApplicationUserRole.Active)] = newRecord.Active;
            }

            if (oldRecord.UpdatedBy != req.CurrentUser)
            {
                changeLog[nameof(ApplicationUserRole.UpdatedBy)] = req.CurrentUser;
            }
            
            changeLog[nameof(ApplicationUserRole.UpdatedOn)] = oldRecord.UpdatedOn;
            
            await dbContext.AuditLogs.AddAsync(new AuditLog {
                LogType = AuditLogLogTypes.Update,
                ReferenceType = EntityFieldNames.ApplicationUserRole,
                ReferenceId = oldRecord.ApplicationUserRoleId,
                ChangeLogJson = JsonSerializer.Serialize(changeLog),
                RecordStateBeforeChangeJson = GetRecordStateBeforeChangeJson(oldRecord),
                CreatedBy = req.CurrentUser,
                CreatedOn = CommonUtilities.GetDateTimeUtcNow()
            });
        }

        private async Task LogDelete(SecurityDBContext dbContext, ApplicationUserRole record, string currentUser) 
        {
            await dbContext.AuditLogs.AddAsync(new AuditLog {
                LogType = AuditLogLogTypes.Delete,
                ReferenceType = EntityFieldNames.ApplicationUserRole,
                ReferenceId = record.ApplicationUserRoleId,
                ChangeLogJson = JsonSerializer.Serialize(new {}),
                RecordStateBeforeChangeJson = GetRecordStateBeforeChangeJson(record),
                CreatedBy = currentUser,
                CreatedOn = CommonUtilities.GetDateTimeUtcNow()
            });
        }

        private string GetRecordStateBeforeChangeJson(ApplicationUserRole record)
        {
            var log = new Dictionary<string, object?>();
            log[nameof(ApplicationUserRole.ApplicationId)] = record.ApplicationId;
            log[nameof(ApplicationUserRole.ApplicationUserId)] = record.ApplicationUserId;
            log[nameof(ApplicationUserRole.RoleId)] = record.RoleId;
            log[nameof(ApplicationUserRole.Active)] = record.Active;
            log[nameof(ApplicationUserRole.CreatedBy)] = record.CreatedBy;
            log[nameof(ApplicationUserRole.CreatedOn)] = record.CreatedOn;
            log[nameof(ApplicationUserRole.UpdatedBy)] = record.UpdatedBy;
            log[nameof(ApplicationUserRole.UpdatedOn)] = record.UpdatedOn;
            
            return JsonSerializer.Serialize(log);
        }

        #endregion
    }
}
