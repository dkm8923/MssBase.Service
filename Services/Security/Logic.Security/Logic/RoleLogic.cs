using Contract.Security;
using Contract.Security.Role;
using Data.Security;
using Data.Security.Converters;
using Dto.Security.Role;
using Dto.Security.Role.Logic;
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
    public class RoleLogic : IRoleLogic
    {
        private readonly ISecurityConnectionStrings _connectionStrings;
        private readonly SecurityDBContextFactory _dbContextFactory;

        private IValidator<FilterRoleLogicRequest> _filterRoleLogicRequestValidator;
        private IValidator<InsertUpdateRoleRequest> _insertUpdateRoleRequestValidator;

        public RoleLogic(
                            ISecurityConnectionStrings connectionStrings,
                            IValidator<FilterRoleLogicRequest> filterRoleLogicRequestValidator,
                            IValidator<InsertUpdateRoleRequest> insertUpdateRoleRequestValidator
        )
        {
            _connectionStrings = connectionStrings;
            _dbContextFactory = new SecurityDBContextFactory(_connectionStrings);
            _filterRoleLogicRequestValidator = filterRoleLogicRequestValidator;
            _insertUpdateRoleRequestValidator = insertUpdateRoleRequestValidator;
        }

        #region GetAll

        /// <summary>
        /// Retrieves a collection of Roles based on the specified request parameters.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<RoleDto>>> GetAll(BaseLogicGet req, CancellationToken cancellationToken = default)
        {
            var ret = await this.Filter(new FilterRoleLogicRequest { IncludeInactive = req.IncludeInactive, IncludeRelated = req.IncludeRelated, IncludeReadOnly = req.IncludeReadOnly, CurrentUser = req.CurrentUser }, cancellationToken);
            return ret;
        }

        #endregion

        #region GetById

        /// <summary>
        /// Retrieves an Role by its unique identifier.
        /// </summary>
        public async Task<ErrorValidationResult<RoleDto>> GetById(int RoleId, BaseLogicGet req, CancellationToken cancellationToken = default)
        {
            var res = await this.Filter(new FilterRoleLogicRequest { RoleIds = new List<int> { RoleId }, IncludeInactive = req.IncludeInactive, IncludeRelated = req.IncludeRelated, IncludeReadOnly = req.IncludeReadOnly, CurrentUser = req.CurrentUser }, cancellationToken);

            return new ErrorValidationResult<RoleDto> { Response = res.Response.FirstOrDefault() };
        }

        #endregion
        
        #region GetAuditLogsByRoleId
        
        /// <summary>
        /// Retrieves the audit logs for a role by its unique identifier.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByRoleId(int roleId, CancellationToken cancellationToken = default)
        {
            using (var dbContext = _dbContextFactory.CreateContextReadOnly())
            {
                var query = dbContext.AuditLogs.AsQueryable().AsNoTracking().Where(al => al.ReferenceType == EntityFieldNames.Role && al.ReferenceId == roleId);
                return new ErrorValidationResult<IEnumerable<AuditLogDto>> { Response = await query.ToDtos(cancellationToken) };
            }
        }

        #endregion

        #region Filter

        /// <summary>
        /// Filters Roles based on the specified criteria.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<RoleDto>>> Filter(FilterRoleLogicRequest req, CancellationToken cancellationToken = default)
        {
            var errorValidationResult = await _validateRoleFilter(req);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadOnly())
            {
                var query = dbContext.Roles.AsQueryable().AsNoTracking();

                query = query.ApplyIncludeInactiveFilter(req);
                query = query.ApplyIncludeReadOnlyFilter(req);
                query = query.ApplyAuditableFilters(req);

                if (req.IncludeRelated)
                {
                    query = query.Include(role => role.RolePermissions.Where(rp => req.IncludeInactive || rp.Active))
                                 .ThenInclude(rp => rp.Permission);
                }

                if (req.RoleIds != null && req.RoleIds.Count > 0)
                {
                    query = query.Where(x => req.RoleIds.Contains(x.RoleId));
                }
                
                if (!string.IsNullOrWhiteSpace(req.Name))
                {
                    query = query.Where(x => x.Name == req.Name);
                }

                if (req.ApplicationId != null)
                {
                    query = query.Where(x => x.ApplicationId == req.ApplicationId);
                }

                return new ErrorValidationResult<IEnumerable<RoleDto>> { Response = await query.ToDtos(cancellationToken) };
            }
        }

        #endregion

        #region Insert

        /// <summary>
        /// Inserts a new Role into the data store.
        /// </summary>
        public async Task<ErrorValidationResult<RoleDto>> Insert(InsertUpdateRoleRequest req, IApplicationLogic applicationLogic)
        {
            var errorValidationResult = await _validateRoleOnInsertUpdate(applicationLogic, req);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = req.ToEntityOnInsert();

                await dbContext.Roles.AddAsync(entity);
                await dbContext.SaveChangesAsync();

                return new ErrorValidationResult<RoleDto> { Response = entity.ToDto() };
            }
        }

        #endregion

        #region Update  

        /// <summary>
        /// Updates the details of an existing Role.
        /// </summary>
        public async Task<ErrorValidationResult<RoleDto>> Update(int roleId, InsertUpdateRoleRequest req, IApplicationLogic applicationLogic)
        {
            var errorValidationResult = await _validateRoleOnInsertUpdate(applicationLogic, req, roleId);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.Roles.FirstOrDefaultAsync(ent => ent.RoleId == roleId);

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
                return new ErrorValidationResult<RoleDto> { Response = entity.ToDto() };
            }
        }

        #endregion

        #region Delete  

        /// <summary>
        /// Deletes the Role with the specified identifier.
        /// </summary>
        public async Task<ErrorValidationResult> Delete(int roleId, string currentUser)
        {
            var errorValidationResult = await _validateRoleOnDelete(roleId);

            if (errorValidationResult.Errors.Count == 0)
            {
                using (var dbContext = _dbContextFactory.CreateContextReadWrite())
                {
                    var entity = await dbContext.Roles.FirstOrDefaultAsync(ent => ent.RoleId == roleId && !ent.ReadOnly);
                    
                    await LogDelete(dbContext, entity, currentUser);
                    
                    dbContext.Roles.Remove(entity);
                    await dbContext.SaveChangesAsync();
                    errorValidationResult.Response = null;
                }
            }

            return errorValidationResult;
        }

        #endregion

        #region Validation

        private async Task<ErrorValidationResult<IEnumerable<RoleDto>>> _validateRoleFilter(FilterRoleLogicRequest req)
        {
            ValidationResult result = await _filterRoleLogicRequestValidator.ValidateAsync(req);
            var errorValidationResult = ValidatorUtilities.CreateDefaultValidationResponse<IEnumerable<RoleDto>>(result);
            return errorValidationResult;
        }

        private async Task<ErrorValidationResult<RoleDto>> _validateRoleOnInsertUpdate(IApplicationLogic applicationLogic, InsertUpdateRoleRequest req, int? roleId = null)
        {
            ValidationResult result = await _insertUpdateRoleRequestValidator.ValidateAsync(req);
            var errorValidationResult = ValidatorUtilities.CreateDefaultValidationResponse<RoleDto>(result);

            if (errorValidationResult.Errors.Count == 0)
            {
                // Validate Application exists
                var applicationResponse = await applicationLogic.GetById(req.ApplicationId, new BaseLogicGet { IncludeInactive = true, IncludeReadOnly = true });
                
                if (applicationResponse.Response == null)
                {
                    errorValidationResult.Errors.Add(Constants.EntityFieldNames.ApplicationId, new List<string> { ValidatorUtilities.CreateRecordDoesNotExistValidationErrorMessage(Constants.EntityFieldNames.ApplicationId) });
                    return errorValidationResult;
                }

                // Validate Role name is unique
                var nameCheck = await this.Filter(new FilterRoleLogicRequest { Name = req.Name });

                if (nameCheck.Errors.Count == 0 && nameCheck.Response.Count() > 0)
                {
                    if ((roleId == null || roleId == 0) || (nameCheck.Response.FirstOrDefault().RoleId != roleId))
                    {
                        errorValidationResult.Errors.Add(Constants.EntityFieldNames.Name, new List<string> { ValidatorUtilities.CreateUniqueValidationErrorMessage(Constants.EntityFieldNames.Name) });
                    }
                }
            }

            return errorValidationResult;
        }

        private async Task<ErrorValidationResult<RoleDto>> _validateRoleOnDelete(int roleId)
        {
            var roleErrorValidationResult = await GetById(roleId, new BaseLogicGet { IncludeInactive = true, IncludeRelated = true, IncludeReadOnly = true });

            if (roleErrorValidationResult.Response == null)
            {
                //role for given id does not exist
                roleErrorValidationResult.Errors = AddRecordNotFoundErrorToErrorValidationResult(roleErrorValidationResult.Errors);
                return roleErrorValidationResult;
            }

            if (roleErrorValidationResult.Response != null && roleErrorValidationResult.Response.ReadOnly)
            {
                return await _returnReadOnlyRecordErrorValidationResult();
            }

            return roleErrorValidationResult;
        }

        private Dictionary<string, List<string>> AddRecordNotFoundErrorToErrorValidationResult(Dictionary<string, List<string>> errors)
        {
            errors.Add(Constants.EntityFieldNames.Role, new List<string> { ValidatorUtilities.CreateRecordDoesNotExistValidationErrorMessage(Constants.EntityFieldNames.RoleId) });
            return errors;
        }

        private async Task<ErrorValidationResult<RoleDto>> _returnReadOnlyRecordErrorValidationResult()
        {
            var errorValidationResult = new ErrorValidationResult<RoleDto>();
            errorValidationResult.Errors.Add(Constants.EntityFieldNames.Role, new List<string> { ValidatorUtilities.CreateRecordIsReadOnlyValidationErrorMessage() });
            return errorValidationResult;
        }

        #endregion

        #region Audit Log

        private async Task LogChange(SecurityDBContext dbContext, Role oldRecord, InsertUpdateRoleRequest req) 
        {
            var newRecord = req.ToEntityOnInsert();
            
            // Only capture fields that actually changed, not the full entity graph
            var changeLog = new Dictionary<string, object?>();

            if (oldRecord.Name != newRecord.Name)
            {
                changeLog[nameof(Role.Name)] = newRecord.Name;
            }

            if (oldRecord.Description != newRecord.Description)
            {
                changeLog[nameof(Role.Description)] = newRecord.Description;
            }

            if (oldRecord.ApplicationId != newRecord.ApplicationId)
            {
                changeLog[nameof(Role.ApplicationId)] = newRecord.ApplicationId;
            }

            if (oldRecord.Active != newRecord.Active)
            {
                changeLog[nameof(Role.Active)] = newRecord.Active;
            }

            if (oldRecord.UpdatedBy != req.CurrentUser)
            {
                changeLog[nameof(Role.UpdatedBy)] = req.CurrentUser;
            }
            
            changeLog[nameof(Role.UpdatedOn)] = oldRecord.UpdatedOn;
            
            await dbContext.AuditLogs.AddAsync(new AuditLog {
                LogType = AuditLogLogTypes.Update,
                ReferenceType = EntityFieldNames.Role,
                ReferenceId = oldRecord.RoleId,
                ChangeLogJson = JsonSerializer.Serialize(changeLog),
                RecordStateBeforeChangeJson = GetRecordStateBeforeChangeJson(oldRecord),
                CreatedBy = req.CurrentUser,
                CreatedOn = CommonUtilities.GetDateTimeUtcNow()
            });
        }

        private async Task LogDelete(SecurityDBContext dbContext, Role record, string currentUser) 
        {
            await dbContext.AuditLogs.AddAsync(new AuditLog {
                LogType = AuditLogLogTypes.Delete,
                ReferenceType = EntityFieldNames.Role,
                ReferenceId = record.RoleId,
                ChangeLogJson = JsonSerializer.Serialize(new {}),
                RecordStateBeforeChangeJson = GetRecordStateBeforeChangeJson(record),
                CreatedBy = currentUser,
                CreatedOn = CommonUtilities.GetDateTimeUtcNow()
            });
        }

        private string GetRecordStateBeforeChangeJson(Role record)
        {
            var log = new Dictionary<string, object?>();
            log[nameof(Role.Name)] = record.Name;
            log[nameof(Role.Description)] = record.Description;
            log[nameof(Role.Active)] = record.Active;
            log[nameof(Role.CreatedBy)] = record.CreatedBy;
            log[nameof(Role.CreatedOn)] = record.CreatedOn;
            log[nameof(Role.UpdatedBy)] = record.UpdatedBy;
            log[nameof(Role.UpdatedOn)] = record.UpdatedOn;
            
            return JsonSerializer.Serialize(log);
        }

        #endregion
    }
}
