using Contract.Security;
using Contract.Security.Permission;
using Data.Security;
using Data.Security.Converters;
using Dto.Security.Permission;
using Dto.Security.Permission.Logic;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using Contract.Security.Application;
using Shared.Logic;
using Shared.Logic.Validators;
using Shared.Logic.Common;
using Data.Security.Models;
using static Shared.Logic.Common.Constants;
using Shared.Data.Models;
using System.Text.Json;
using Shared.Models.Dtos;
using Shared.Data.Converters;

namespace Logic.Security.Logic
{
    public class PermissionLogic : IPermissionLogic
    {
        private readonly ISecurityConnectionStrings _connectionStrings;
        private readonly SecurityDBContextFactory _dbContextFactory;

        private IValidator<FilterPermissionLogicRequest> _filterPermissionLogicRequestValidator;
        private IValidator<InsertUpdatePermissionRequest> _insertUpdatePermissionRequestValidator;

        public PermissionLogic(
                            ISecurityConnectionStrings connectionStrings,
                            IValidator<FilterPermissionLogicRequest> filterPermissionLogicRequestValidator,
                            IValidator<InsertUpdatePermissionRequest> insertUpdatePermissionRequestValidator
        )
        {
            _connectionStrings = connectionStrings;
            _dbContextFactory = new SecurityDBContextFactory(_connectionStrings);
            _filterPermissionLogicRequestValidator = filterPermissionLogicRequestValidator;
            _insertUpdatePermissionRequestValidator = insertUpdatePermissionRequestValidator;
        }

        #region GetAll

        /// <summary>
        /// Retrieves a collection of Permissions based on the specified request parameters.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<PermissionDto>>> GetAll(BaseLogicGet req, CancellationToken cancellationToken = default)
        {
            var ret = await this.Filter(new FilterPermissionLogicRequest { IncludeInactive = req.IncludeInactive, IncludeReadOnly = req.IncludeReadOnly, CurrentUser = req.CurrentUser }, cancellationToken);
            return ret;
        }

        #endregion

        #region GetById

        /// <summary>
        /// Retrieves an Permission by its unique identifier.
        /// </summary>
        public async Task<ErrorValidationResult<PermissionDto>> GetById(int PermissionId, BaseLogicGet req, CancellationToken cancellationToken = default)
        {
            var res = await this.Filter(new FilterPermissionLogicRequest { PermissionIds = new List<int> { PermissionId }, IncludeInactive = req.IncludeInactive, IncludeReadOnly = req.IncludeReadOnly, CurrentUser = req.CurrentUser }, cancellationToken);

            return new ErrorValidationResult<PermissionDto> { Response = res.Response.FirstOrDefault() };
        }

        #endregion

        #region GetAuditLogsByPermissionId

        /// <summary>
        /// Retrieves the audit logs for a permission by its unique identifier.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByPermissionId(int permissionId, CancellationToken cancellationToken = default)
        {
            using (var dbContext = _dbContextFactory.CreateContextReadOnly())
            {
                var query = dbContext.AuditLogs.AsQueryable().AsNoTracking().Where(al => al.ReferenceType == EntityFieldNames.Permission && al.ReferenceId == permissionId);
                return new ErrorValidationResult<IEnumerable<AuditLogDto>> { Response = await query.ToDtos(cancellationToken) };
            }
        }

        #endregion

        #region Filter

        /// <summary>
        /// Filters Permissions based on the specified criteria.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<PermissionDto>>> Filter(FilterPermissionLogicRequest req, CancellationToken cancellationToken = default)
        {
            var errorValidationResult = await _validatePermissionFilter(req);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadOnly())
            {
                var query = dbContext.Permissions.AsQueryable().AsNoTracking();

                query = query.ApplyIncludeInactiveFilter(req);
                query = query.ApplyIncludeReadOnlyFilter(req);
                query = query.ApplyAuditableFilters(req);

                if (req.PermissionIds != null && req.PermissionIds.Count > 0)
                {
                    query = query.Where(x => req.PermissionIds.Contains(x.PermissionId));
                }
                
                if (!string.IsNullOrWhiteSpace(req.Name))
                {
                    query = query.Where(x => x.Name == req.Name);
                }

                if (req.ApplicationId != null)
                {
                    query = query.Where(x => x.ApplicationId == req.ApplicationId);
                }

                return new ErrorValidationResult<IEnumerable<PermissionDto>> { Response = await query.ToDtos(cancellationToken) };
            }
        }

        #endregion

        #region Insert

        /// <summary>
        /// Inserts a new Permission into the data store.
        /// </summary>
        public async Task<ErrorValidationResult<PermissionDto>> Insert(InsertUpdatePermissionRequest req, IApplicationLogic applicationLogic)
        {
            var errorValidationResult = await _validatePermissionOnInsertUpdate(applicationLogic, req);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = req.ToEntityOnInsert();

                await dbContext.Permissions.AddAsync(entity);
                await dbContext.SaveChangesAsync();

                return new ErrorValidationResult<PermissionDto> { Response = entity.ToDto() };
            }
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates the details of an existing Permission.
        /// </summary>
        public async Task<ErrorValidationResult<PermissionDto>> Update(int PermissionId, InsertUpdatePermissionRequest req, IApplicationLogic applicationLogic)
        {
            var errorValidationResult = await _validatePermissionOnInsertUpdate(applicationLogic, req, PermissionId);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.Permissions.FirstOrDefaultAsync(ent => ent.PermissionId == PermissionId);

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
                return new ErrorValidationResult<PermissionDto> { Response = entity.ToDto() };
            }
        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes the Permission with the specified identifier.
        /// </summary>
        public async Task<ErrorValidationResult> Delete(int permissionId, string currentUser)
        {
            var errorValidationResult = await _validatePermissionOnDelete(permissionId);

            if (errorValidationResult.Errors.Count == 0)
            {
                using (var dbContext = _dbContextFactory.CreateContextReadWrite())
                {
                    var entity = await dbContext.Permissions.FirstOrDefaultAsync(ent => ent.PermissionId == permissionId && !ent.ReadOnly);
                    
                    await LogDelete(dbContext, entity, currentUser);
                    
                    dbContext.Permissions.Remove(entity);
                    await dbContext.SaveChangesAsync();
                    errorValidationResult.Response = null;
                }
            }

            return errorValidationResult;
        }

        #endregion

        #region Validation

        private async Task<ErrorValidationResult<IEnumerable<PermissionDto>>> _validatePermissionFilter(FilterPermissionLogicRequest req)
        {
            ValidationResult result = await _filterPermissionLogicRequestValidator.ValidateAsync(req);
            var errorValidationResult = ValidatorUtilities.CreateDefaultValidationResponse<IEnumerable<PermissionDto>>(result);
            return errorValidationResult;
        }

        private async Task<ErrorValidationResult<PermissionDto>> _validatePermissionOnInsertUpdate(IApplicationLogic applicationLogic, InsertUpdatePermissionRequest req, int? permissionId = null)
        {
            ValidationResult result = await _insertUpdatePermissionRequestValidator.ValidateAsync(req);
            var errorValidationResult = ValidatorUtilities.CreateDefaultValidationResponse<PermissionDto>(result);

            if (errorValidationResult.Errors.Count == 0)
            {
                // Validate Application exists
                var applicationResponse = await applicationLogic.GetById(req.ApplicationId, new BaseLogicGet { IncludeInactive = true, IncludeReadOnly = true });
                
                if (applicationResponse.Response == null)
                {
                    errorValidationResult.Errors.Add(Constants.EntityFieldNames.ApplicationId, new List<string> { ValidatorUtilities.CreateRecordDoesNotExistValidationErrorMessage(Constants.EntityFieldNames.ApplicationId) });
                    return errorValidationResult;
                }

                // Validate Permission name is unique
                var nameCheck = await this.Filter(new FilterPermissionLogicRequest { Name = req.Name, IncludeInactive = true, IncludeReadOnly = true });

                if (nameCheck.Errors.Count == 0 && nameCheck.Response.Count() > 0)
                {
                    if ((permissionId == null || permissionId == 0) || (nameCheck.Response.FirstOrDefault().PermissionId != permissionId))
                    {
                        errorValidationResult.Errors.Add(Constants.EntityFieldNames.Name, new List<string> { ValidatorUtilities.CreateUniqueValidationErrorMessage(Constants.EntityFieldNames.Name) });
                    }
                }
            }

            return errorValidationResult;
        }

        private async Task<ErrorValidationResult<PermissionDto>> _validatePermissionOnDelete(int permissionId)
        {
            var permissionErrorValidationResult = await GetById(permissionId, new BaseLogicGet { IncludeInactive = true, IncludeRelated = true, IncludeReadOnly = true });

            if (permissionErrorValidationResult.Response == null)
            {
                //permission for given id does not exist
                permissionErrorValidationResult.Errors = AddRecordNotFoundErrorToErrorValidationResult(permissionErrorValidationResult.Errors);
                return permissionErrorValidationResult;
            }

            if (permissionErrorValidationResult.Response != null && permissionErrorValidationResult.Response.ReadOnly)
            {
                return await _returnReadOnlyRecordErrorValidationResult();
            }

            return permissionErrorValidationResult;
        }

        private Dictionary<string, List<string>> AddRecordNotFoundErrorToErrorValidationResult(Dictionary<string, List<string>> errors)
        {
            return LogicUtilities.AddRecordNotFoundErrorToErrorValidationResult(errors, Constants.EntityFieldNames.Permission, Constants.EntityFieldNames.PermissionId);
        }

        private async Task<ErrorValidationResult<PermissionDto>> _returnReadOnlyRecordErrorValidationResult()
        {
            var errorValidationResult = new ErrorValidationResult<PermissionDto>();
            errorValidationResult.Errors.Add(Constants.EntityFieldNames.Permission, new List<string> { ValidatorUtilities.CreateRecordIsReadOnlyValidationErrorMessage() });
            return errorValidationResult;
        }

        #endregion

        #region Audit Log

        private async Task LogChange(SecurityDBContext dbContext, Permission oldRecord, InsertUpdatePermissionRequest req) 
        {
            var newRecord = req.ToEntityOnInsert();
            
            // Only capture fields that actually changed, not the full entity graph
            var changeLog = new Dictionary<string, object?>();

            if (oldRecord.Name != newRecord.Name)
            {
                changeLog[nameof(Permission.Name)] = newRecord.Name;
            }

            if (oldRecord.Description != newRecord.Description)
            {
                changeLog[nameof(Permission.Description)] = newRecord.Description;
            }

            if (oldRecord.ApplicationId != newRecord.ApplicationId)
            {
                changeLog[nameof(ApplicationUser.ApplicationId)] = newRecord.ApplicationId;
            }

            if (oldRecord.Active != newRecord.Active)
            {
                changeLog[nameof(Permission.Active)] = newRecord.Active;
            }

            if (oldRecord.UpdatedBy != req.CurrentUser)
            {
                changeLog[nameof(Permission.UpdatedBy)] = req.CurrentUser;
            }
            
            changeLog[nameof(Permission.UpdatedOn)] = oldRecord.UpdatedOn;
            
            await dbContext.AuditLogs.AddAsync(new AuditLog {
                LogType = AuditLogLogTypes.Update,
                ReferenceType = EntityFieldNames.Permission,
                ReferenceId = oldRecord.PermissionId,
                ChangeLogJson = JsonSerializer.Serialize(changeLog),
                RecordStateBeforeChangeJson = GetRecordStateBeforeChangeJson(oldRecord),
                CreatedBy = req.CurrentUser,
                CreatedOn = CommonUtilities.GetDateTimeUtcNow()
            });
        }

        private async Task LogDelete(SecurityDBContext dbContext, Permission record, string currentUser) 
        {
            await dbContext.AuditLogs.AddAsync(new AuditLog {
                LogType = AuditLogLogTypes.Delete,
                ReferenceType = EntityFieldNames.Permission,
                ReferenceId = record.PermissionId,
                ChangeLogJson = JsonSerializer.Serialize(new {}),
                RecordStateBeforeChangeJson = GetRecordStateBeforeChangeJson(record),
                CreatedBy = currentUser,
                CreatedOn = CommonUtilities.GetDateTimeUtcNow()
            });
        }

        private string GetRecordStateBeforeChangeJson(Permission record)
        {
            var log = new Dictionary<string, object?>();
            log[nameof(Permission.Name)] = record.Name;
            log[nameof(Permission.Description)] = record.Description;
            log[nameof(Permission.ApplicationId)] = record.ApplicationId;
            log[nameof(Permission.Active)] = record.Active;
            log[nameof(Permission.CreatedBy)] = record.CreatedBy;
            log[nameof(Permission.CreatedOn)] = record.CreatedOn;
            log[nameof(Permission.UpdatedBy)] = record.UpdatedBy;
            log[nameof(Permission.UpdatedOn)] = record.UpdatedOn;
            
            return JsonSerializer.Serialize(log);
        }

        #endregion
    }
}
