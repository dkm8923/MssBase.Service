using Contract.Security;
using Contract.Security.Application;
using Contract.Security.RolePermission;
using Contract.Security.Permission;
using Data.Security;
using Data.Security.Converters;
using Dto.Security.RolePermission;
using Dto.Security.RolePermission.Logic;
using Dto.Security.Permission.Logic;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using Shared.Logic;
using Shared.Logic.Validators;
using Dto.Security.Role.Logic;
using Contract.Security.Role;
using Shared.Logic.Common;
using Shared.Models.Dtos;
using static Shared.Logic.Common.Constants;
using Shared.Data.Converters;
using Data.Security.Models;
using Shared.Data.Models;
using System.Text.Json;

namespace Logic.Security.Logic
{
    public class RolePermissionLogic : IRolePermissionLogic
    {
        private readonly ISecurityConnectionStrings _connectionStrings;
        private readonly SecurityDBContextFactory _dbContextFactory;

        private IValidator<FilterRolePermissionLogicRequest> _filterRolePermissionLogicRequestValidator;
        private IValidator<InsertUpdateRolePermissionRequest> _insertUpdateRolePermissionRequestValidator;

        public RolePermissionLogic(
                            ISecurityConnectionStrings connectionStrings,
                            IValidator<FilterRolePermissionLogicRequest> filterRolePermissionLogicRequestValidator,
                            IValidator<InsertUpdateRolePermissionRequest> insertUpdateRolePermissionRequestValidator
        )
        {
            _connectionStrings = connectionStrings;
            _dbContextFactory = new SecurityDBContextFactory(_connectionStrings);
            _filterRolePermissionLogicRequestValidator = filterRolePermissionLogicRequestValidator;
            _insertUpdateRolePermissionRequestValidator = insertUpdateRolePermissionRequestValidator;
        }

        #region GetAll

        /// <summary>
        /// Retrieves a collection of role permissions based on the specified request parameters.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<RolePermissionDto>>> GetAll(BaseLogicGet req, CancellationToken cancellationToken = default)
        {
            var ret = await this.Filter(new FilterRolePermissionLogicRequest { IncludeInactive = req.IncludeInactive, IncludeReadOnly = req.IncludeReadOnly, CurrentUser = req.CurrentUser, IncludeRelated = req.IncludeRelated }, cancellationToken);
            return ret;
        }

        #endregion

        #region GetById

        /// <summary>
        /// Retrieves a role permission by its unique identifier.
        /// </summary>
        public async Task<ErrorValidationResult<RolePermissionDto>> GetById(int rolePermissionId, BaseLogicGet req, CancellationToken cancellationToken = default)
        {
            var res = await this.Filter(new FilterRolePermissionLogicRequest { RolePermissionIds = new List<int> { rolePermissionId }, IncludeInactive = req.IncludeInactive, IncludeReadOnly = req.IncludeReadOnly, CurrentUser = req.CurrentUser, IncludeRelated = req.IncludeRelated }, cancellationToken);

            return new ErrorValidationResult<RolePermissionDto> { Response = res.Response.FirstOrDefault() };
        }

        #endregion

        #region GetAuditLogsByRolePermissionId

        /// <summary>
        /// Retrieves the audit logs for a role permission by its unique identifier.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByRolePermissionId(int rolePermissionId, CancellationToken cancellationToken = default)
        {
            using (var dbContext = _dbContextFactory.CreateContextReadOnly())
            {
                var query = dbContext.AuditLogs.AsQueryable().AsNoTracking().Where(al => al.ReferenceType == EntityFieldNames.RolePermission && al.ReferenceId == rolePermissionId);
                return new ErrorValidationResult<IEnumerable<AuditLogDto>> { Response = await query.ToDtos(cancellationToken) };
            }
        }

        #endregion

        #region Filter

        /// <summary>
        /// Filters role permissions based on the specified criteria.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<RolePermissionDto>>> Filter(FilterRolePermissionLogicRequest req, CancellationToken cancellationToken = default)
        {
            var errorValidationResult = await _validateRolePermissionFilter(req);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadOnly())
            {
                var query = dbContext.RolePermissions.AsQueryable().AsNoTracking();

                query = query.ApplyIncludeInactiveFilter(req);
                query = query.ApplyIncludeReadOnlyFilter(req);
                query = query.ApplyAuditableFilters(req);

                if (req.IncludeRelated)
                {
                    query = query.Include(rp => rp.Permission).Where(rp => req.IncludeInactive || rp.Active);
                }

                if (req.RolePermissionIds != null && req.RolePermissionIds.Count > 0)
                {
                    query = query.Where(x => req.RolePermissionIds.Contains(x.RolePermissionId));
                }
                
                if (req.ApplicationId != null)
                {
                    query = query.Where(x => x.ApplicationId == req.ApplicationId);
                }

                if (req.RoleId != null)
                {
                    query = query.Where(x => x.RoleId == req.RoleId);
                }

                if (req.PermissionId != null)
                {
                    query = query.Where(x => x.PermissionId == req.PermissionId);
                }

                return new ErrorValidationResult<IEnumerable<RolePermissionDto>> { Response = await query.ToDtos(cancellationToken) };
            }
        }

        #endregion

        #region Insert

        /// <summary>
        /// Inserts a new role permission into the data store.
        /// </summary>
        public async Task<ErrorValidationResult<RolePermissionDto>> Insert(InsertUpdateRolePermissionRequest req, 
                                                                                      IApplicationLogic applicationLogic,
                                                                                      IRoleLogic roleLogic,
                                                                                      IPermissionLogic permissionLogic
                                                                                     )
        {
            var errorValidationResult = await _validateRolePermissionOnInsertUpdate(applicationLogic, roleLogic, permissionLogic, req);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = req.ToEntityOnInsert();

                await dbContext.RolePermissions.AddAsync(entity);
                await dbContext.SaveChangesAsync();

                return new ErrorValidationResult<RolePermissionDto> { Response = entity.ToDto() };
            }
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates the details of an existing role permission.
        /// </summary>
        public async Task<ErrorValidationResult<RolePermissionDto>> Update(int rolePermissionId, 
                                                                                      InsertUpdateRolePermissionRequest req, 
                                                                                      IApplicationLogic applicationLogic,
                                                                                      IRoleLogic roleLogic,
                                                                                      IPermissionLogic permissionLogic
                                                                                     )
        {
            var errorValidationResult = await _validateRolePermissionOnInsertUpdate(applicationLogic, roleLogic, permissionLogic, req, rolePermissionId);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.RolePermissions.FirstOrDefaultAsync(ent => ent.RolePermissionId == rolePermissionId);

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
                return new ErrorValidationResult<RolePermissionDto> { Response = entity.ToDto() };
            }
        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes the role permission with the specified identifier.
        /// </summary>
        public async Task<ErrorValidationResult> Delete(int rolePermissionId, string currentUser)
        {
            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.RolePermissions.FirstOrDefaultAsync(ent => ent.RolePermissionId == rolePermissionId);
                var errorValidationResult = new ErrorValidationResult();

                if (entity != null)
                {
                    if (entity.ReadOnly)
                    {
                        return await _returnReadOnlyRecordErrorValidationResult();
                    }

                    await LogDelete(dbContext, entity, currentUser);

                    dbContext.RolePermissions.Remove(entity);

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

        private async Task<ErrorValidationResult<IEnumerable<RolePermissionDto>>> _validateRolePermissionFilter(FilterRolePermissionLogicRequest req)
        {
            ValidationResult result = await _filterRolePermissionLogicRequestValidator.ValidateAsync(req);
            var errorValidationResult = ValidatorUtilities.CreateDefaultValidationResponse<IEnumerable<RolePermissionDto>>(result);
            return errorValidationResult;
        }

        private async Task<ErrorValidationResult<RolePermissionDto>> _validateRolePermissionOnInsertUpdate(IApplicationLogic applicationLogic,
                                                                                                           IRoleLogic roleLogic,
                                                                                                           IPermissionLogic permissionLogic,         
                                                                                                           InsertUpdateRolePermissionRequest req,
                                                                                                           int? rolePermissionId = null
                                                                                                        )
        {
            ValidationResult result = await _insertUpdateRolePermissionRequestValidator.ValidateAsync(req);
            var errorValidationResult = ValidatorUtilities.CreateDefaultValidationResponse<RolePermissionDto>(result);

            if (errorValidationResult.Errors.Count == 0)
            {
                // Validate Application exists
                var applicationResponse = await applicationLogic.GetById(req.ApplicationId, new BaseLogicGet { IncludeInactive = true, IncludeReadOnly = true });
                
                if (applicationResponse.Response == null)
                {
                    errorValidationResult.Errors.Add(Constants.EntityFieldNames.ApplicationId, new List<string> { ValidatorUtilities.CreateRecordDoesNotExistValidationErrorMessage(Constants.EntityFieldNames.ApplicationId) });
                    return errorValidationResult;
                }

                // Validate Role exists
                var roleResponse = await roleLogic.Filter(new FilterRoleLogicRequest { RoleIds = new List<int> { req.RoleId }, ApplicationId = req.ApplicationId, IncludeInactive = true, IncludeReadOnly = true });

                if (roleResponse.Response == null || roleResponse.Response.Count() == 0)
                {
                    errorValidationResult.Errors.Add(Constants.EntityFieldNames.RoleId, new List<string> { ValidatorUtilities.CreateRecordDoesNotExistValidationErrorMessage(Constants.EntityFieldNames.RoleId) });
                    return errorValidationResult;
                }

                // Validate Permission exists
                var permissionResponse = await permissionLogic.Filter(new FilterPermissionLogicRequest { PermissionIds = new List<int> { req.PermissionId }, ApplicationId = req.ApplicationId, IncludeInactive = true, IncludeReadOnly = true });

                if (permissionResponse.Response == null || permissionResponse.Response.Count() == 0)
                {
                    errorValidationResult.Errors.Add(Constants.EntityFieldNames.PermissionId, new List<string> { ValidatorUtilities.CreateRecordDoesNotExistValidationErrorMessage(Constants.EntityFieldNames.PermissionId) });
                    return errorValidationResult;
                }

                // Validate RolePermission is unique
                var uniqueRolePermissionCheck = await this.Filter(new FilterRolePermissionLogicRequest { 
                    ApplicationId = req.ApplicationId, 
                    PermissionId = req.PermissionId, 
                    RoleId = req.RoleId, 
                    IncludeInactive = true,
                    IncludeReadOnly = true 
                });

                if (uniqueRolePermissionCheck.Errors.Count == 0 && uniqueRolePermissionCheck.Response.Count() > 0)
                {
                    if ((rolePermissionId == null || rolePermissionId == 0) || (uniqueRolePermissionCheck.Response.FirstOrDefault().RolePermissionId != rolePermissionId))
                    {
                        errorValidationResult.Errors.Add(Constants.EntityFieldNames.RolePermission, new List<string> { ValidatorUtilities.CreateUniqueValidationErrorMessage(Constants.EntityFieldNames.RolePermission) });
                    }
                }
            }

            return errorValidationResult;
        }

        private Dictionary<string, List<string>> AddRecordNotFoundErrorToErrorValidationResult(Dictionary<string, List<string>> errors)
        {
            return LogicUtilities.AddRecordNotFoundErrorToErrorValidationResult(errors, Constants.EntityFieldNames.RolePermission, Constants.EntityFieldNames.RolePermissionId);
        }

        private async Task<ErrorValidationResult<RolePermissionDto>> _returnReadOnlyRecordErrorValidationResult()
        {
            var errorValidationResult = new ErrorValidationResult<RolePermissionDto>();
            errorValidationResult.Errors.Add(Constants.EntityFieldNames.RolePermission, new List<string> { ValidatorUtilities.CreateRecordIsReadOnlyValidationErrorMessage() });
            return errorValidationResult;
        }

        #endregion

        #region Audit Log

        private async Task LogChange(SecurityDBContext dbContext, RolePermission oldRecord, InsertUpdateRolePermissionRequest req) 
        {
            var newRecord = req.ToEntityOnInsert();
            
            // Only capture fields that actually changed, not the full entity graph
            var changeLog = new Dictionary<string, object?>();

            if (oldRecord.ApplicationId != newRecord.ApplicationId)
            {
                changeLog[nameof(RolePermission.ApplicationId)] = newRecord.ApplicationId;
            }
            
            if (oldRecord.PermissionId != newRecord.PermissionId)
            {
                changeLog[nameof(RolePermission.PermissionId)] = newRecord.PermissionId;
            }

            if (oldRecord.RoleId != newRecord.RoleId)
            {
                changeLog[nameof(RolePermission.RoleId)] = newRecord.RoleId;
            }

            if (oldRecord.Active != newRecord.Active)
            {
                changeLog[nameof(RolePermission.Active)] = newRecord.Active;
            }

            if (oldRecord.UpdatedBy != req.CurrentUser)
            {
                changeLog[nameof(RolePermission.UpdatedBy)] = req.CurrentUser;
            }
            
            changeLog[nameof(RolePermission.UpdatedOn)] = oldRecord.UpdatedOn;
            
            await dbContext.AuditLogs.AddAsync(new AuditLog {
                LogType = AuditLogLogTypes.Update,
                ReferenceType = EntityFieldNames.RolePermission,
                ReferenceId = oldRecord.RolePermissionId,
                ChangeLogJson = JsonSerializer.Serialize(changeLog),
                RecordStateBeforeChangeJson = GetRecordStateBeforeChangeJson(oldRecord),
                CreatedBy = req.CurrentUser,
                CreatedOn = CommonUtilities.GetDateTimeUtcNow()
            });
        }

        private async Task LogDelete(SecurityDBContext dbContext, RolePermission record, string currentUser) 
        {
            await dbContext.AuditLogs.AddAsync(new AuditLog {
                LogType = AuditLogLogTypes.Delete,
                ReferenceType = EntityFieldNames.RolePermission,
                ReferenceId = record.RolePermissionId,
                ChangeLogJson = JsonSerializer.Serialize(new {}),
                RecordStateBeforeChangeJson = GetRecordStateBeforeChangeJson(record),
                CreatedBy = currentUser,
                CreatedOn = CommonUtilities.GetDateTimeUtcNow()
            });
        }

        private string GetRecordStateBeforeChangeJson(RolePermission record)
        {
            var log = new Dictionary<string, object?>();
            log[nameof(RolePermission.ApplicationId)] = record.ApplicationId;
            log[nameof(RolePermission.PermissionId)] = record.PermissionId;
            log[nameof(RolePermission.RoleId)] = record.RoleId;
            log[nameof(RolePermission.Active)] = record.Active;
            log[nameof(RolePermission.CreatedBy)] = record.CreatedBy;
            log[nameof(RolePermission.CreatedOn)] = record.CreatedOn;
            log[nameof(RolePermission.UpdatedBy)] = record.UpdatedBy;
            log[nameof(RolePermission.UpdatedOn)] = record.UpdatedOn;
            
            return JsonSerializer.Serialize(log);
        }

        #endregion
    }
}
