using Contract.Security;
using Contract.Security.Application;
using Data.Security;
using Data.Security.Converters;
using Dto.Security.Application;
using Dto.Security.Application.Logic;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using Shared.Logic;
using Shared.Logic.Validators;
using Shared.Logic.Common;
using Data.Security.Models;
using Shared.Data.Models;
using Shared.Data.Converters;
using System.Text.Json;
using static Shared.Logic.Common.Constants;
using Shared.Models.Dtos;

namespace Logic.Security.Logic
{
    public class ApplicationLogic : IApplicationLogic
    {
        private readonly ISecurityConnectionStrings _connectionStrings;
        private readonly SecurityDBContextFactory _dbContextFactory;

        private IValidator<FilterApplicationLogicRequest> _filterApplicationLogicRequestValidator;
        private IValidator<InsertUpdateApplicationRequest> _insertUpdateApplicationRequestValidator;

        public ApplicationLogic(
                            ISecurityConnectionStrings connectionStrings,
                            IValidator<FilterApplicationLogicRequest> filterApplicationLogicRequestValidator,
                            IValidator<InsertUpdateApplicationRequest> insertUpdateApplicationRequestValidator
        )
        {
            _connectionStrings = connectionStrings;
            _dbContextFactory = new SecurityDBContextFactory(_connectionStrings);
            _filterApplicationLogicRequestValidator = filterApplicationLogicRequestValidator;
            _insertUpdateApplicationRequestValidator = insertUpdateApplicationRequestValidator;
        }

        /// <summary>
        /// Retrieves a collection of applications based on the specified request parameters.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<ApplicationDto>>> GetAll(BaseLogicGet req, CancellationToken cancellationToken = default)
        {
            var ret = await this.Filter(new FilterApplicationLogicRequest { IncludeInactive = req.IncludeInactive, CurrentUser = req.CurrentUser, IncludeRelated = req.IncludeRelated, IncludeReadOnly = req.IncludeReadOnly }, cancellationToken);
            return ret;
        }

        /// <summary>
        /// Retrieves an application by its unique identifier.
        /// </summary>
        public async Task<ErrorValidationResult<ApplicationDto>> GetById(int applicationId, BaseLogicGet req, CancellationToken cancellationToken = default)
        {
            var res = await this.Filter(new FilterApplicationLogicRequest { ApplicationIds = new List<int> { applicationId }, IncludeInactive = req.IncludeInactive, CurrentUser = req.CurrentUser, IncludeRelated = req.IncludeRelated, IncludeReadOnly = req.IncludeReadOnly }, cancellationToken);

            return new ErrorValidationResult<ApplicationDto> { Response = res.Response.FirstOrDefault() };
        }

        /// <summary>
        /// Retrieves the audit logs for an application by its unique identifier.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByApplicationId(int applicationId, CancellationToken cancellationToken = default)
        {
            using (var dbContext = _dbContextFactory.CreateContextReadOnly())
            {
                var query = dbContext.AuditLogs.AsQueryable().AsNoTracking().Where(al => al.ReferenceType == EntityFieldNames.Application && al.ReferenceId == applicationId);

                return new ErrorValidationResult<IEnumerable<AuditLogDto>> { Response = await query.ToDtos(cancellationToken) };
            }
        }

        /// <summary>
        /// Filters applications based on the specified criteria.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<ApplicationDto>>> Filter(FilterApplicationLogicRequest req, CancellationToken cancellationToken = default)
        {
            var errorValidationResult = await _validateApplicationFilter(req, cancellationToken);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadOnly())
            {
                var query = dbContext.Applications.AsQueryable().AsNoTracking();

                query = query.ApplyIncludeInactiveFilter(req);
                query = query.ApplyIncludeReadOnlyFilter(req);
                query = query.ApplyAuditableFilters(req);

                if (req.IncludeRelated)
                {
                    query = query.Include(application => application.ApplicationUsers).Where(au => (req.IncludeInactive || au.Active) && (req.IncludeReadOnly || !au.ReadOnly))
                                 .Include(application => application.Permissions).Where(p => (req.IncludeInactive || p.Active) && (req.IncludeReadOnly || !p.ReadOnly))
                                 .Include(application => application.Roles).Where(r => (req.IncludeInactive || r.Active) && (req.IncludeReadOnly || !r.ReadOnly))
                                 .Include(application => application.RolePermissions).Where(rp => (req.IncludeInactive || rp.Active) && (req.IncludeReadOnly || !rp.ReadOnly))
                                 .Include(application => application.ApplicationUserPermissions).Where(aup => (req.IncludeInactive || aup.Active) && (req.IncludeReadOnly || !aup.ReadOnly));
                }

                if (req.ApplicationIds != null && req.ApplicationIds.Count > 0)
                {
                    query = query.Where(x => req.ApplicationIds.Contains(x.ApplicationId));
                }
                
                if (req.Name != null)
                {
                    query = query.Where(x => x.Name == req.Name);
                }

                return new ErrorValidationResult<IEnumerable<ApplicationDto>> { Response = await query.ToDtos(cancellationToken) };
            }
        }

        /// <summary>
        /// Inserts a new application into the data store.
        /// </summary>
        public async Task<ErrorValidationResult<ApplicationDto>> Insert(InsertUpdateApplicationRequest req)
        {
            var errorValidationResult = await _validateApplicationOnInsertUpdate(req);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = req.ToEntityOnInsert();

                await dbContext.Applications.AddAsync(entity);
                await dbContext.SaveChangesAsync();

                return new ErrorValidationResult<ApplicationDto> { Response = entity.ToDto() };
            }
        }

        /// <summary>
        /// Updates the details of an existing application.
        /// </summary>
        public async Task<ErrorValidationResult<ApplicationDto>> Update(int applicationId, InsertUpdateApplicationRequest req)
        {
            var errorValidationResult = await _validateApplicationOnInsertUpdate(req, applicationId);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.Applications.FirstOrDefaultAsync(ent => ent.ApplicationId == applicationId  && !ent.ReadOnly);

                if (entity != null)
                {
                    await LogApplicationChange(dbContext, entity, req);
                    
                    entity = entity.UpdateEntityFromRequest(req);
                    await dbContext.SaveChangesAsync();
                    
                    return new ErrorValidationResult<ApplicationDto> { Response = entity.ToDto() };
                }
                else
                {
                    errorValidationResult.Errors = AddRecordNotFoundErrorToErrorValidationResult(errorValidationResult.Errors);
                    return errorValidationResult;
                }
            }
        }

        /// <summary>
        /// Deletes the application with the specified identifier.
        /// </summary>
        public async Task<ErrorValidationResult<ApplicationDto>> Delete(int applicationId, string currentUser)
        {
            var errorValidationResult = await _validateApplicationOnDelete(applicationId);

            if (errorValidationResult.Errors.Count == 0)
            {
                using (var dbContext = _dbContextFactory.CreateContextReadWrite())
                {
                    var entity = await dbContext.Applications.FirstOrDefaultAsync(ent => ent.ApplicationId == applicationId && !ent.ReadOnly);
                    await LogApplicationDelete(dbContext, entity, currentUser);
                    dbContext.Applications.Remove(entity);
                    await dbContext.SaveChangesAsync();
                    errorValidationResult.Response = null;
                }
            }

            return errorValidationResult;
        }

        #region Validation

        private async Task<ErrorValidationResult<IEnumerable<ApplicationDto>>> _validateApplicationFilter(FilterApplicationLogicRequest req, CancellationToken cancellationToken = default)
        {
            ValidationResult result = await _filterApplicationLogicRequestValidator.ValidateAsync(req, cancellationToken);
            var errorValidationResult = ValidatorUtilities.CreateDefaultValidationResponse<IEnumerable<ApplicationDto>>(result);
            return errorValidationResult;
        }

        private async Task<ErrorValidationResult<ApplicationDto>> _validateApplicationOnInsertUpdate(InsertUpdateApplicationRequest req, int? applicationId = null)
        {
            ValidationResult result = await _insertUpdateApplicationRequestValidator.ValidateAsync(req);
            var errorValidationResult = ValidatorUtilities.CreateDefaultValidationResponse<ApplicationDto>(result);

            if (errorValidationResult.Errors.Count == 0)
            {
                if (applicationId != null && applicationId != 0)
                {
                    var applicationToUpdate = await this.GetById(applicationId.Value, new BaseLogicGet { IncludeInactive = true, IncludeRelated = false, IncludeReadOnly = true });
                    
                    // Validate Application is not read only on update
                    if (applicationToUpdate.Response != null && applicationToUpdate.Response.ReadOnly)
                    {
                        return await _returnReadOnlyRecordErrorValidationResult();
                    }
                }

                // Validate Application name is unique
                var nameCheck = await this.Filter(new FilterApplicationLogicRequest { Name = req.Name });

                if (nameCheck.Errors.Count == 0 && nameCheck.Response.Count() > 0)
                {
                    if ((applicationId == null || applicationId == 0) || (nameCheck.Response.FirstOrDefault().ApplicationId != applicationId))
                    {
                        errorValidationResult.Errors.Add("Name", new List<string> { ValidatorUtilities.CreateUniqueValidationErrorMessage("Name") });
                    }
                }
            }

            return errorValidationResult;
        }

        private async Task<ErrorValidationResult<ApplicationDto>> _validateApplicationOnDelete(int applicationId)
        {
            var applicationErrorValidationResult = await GetById(applicationId, new BaseLogicGet { IncludeInactive = true, IncludeRelated = true, IncludeReadOnly = true });

            if (applicationErrorValidationResult.Response == null)
            {
                //application for given id does not exist
                applicationErrorValidationResult.Errors = AddRecordNotFoundErrorToErrorValidationResult(applicationErrorValidationResult.Errors);
                return applicationErrorValidationResult;
            }

            if (applicationErrorValidationResult.Response != null && applicationErrorValidationResult.Response.ReadOnly)
            {
                return await _returnReadOnlyRecordErrorValidationResult();
            }

            //verify no dependencies exist on application record
            if (applicationErrorValidationResult.Response.ApplicationUsers.NotNullAndHasRecords())
            {
                applicationErrorValidationResult.Errors.Add("ApplicationUsers", new List<string> { ValidatorUtilities.CreateDependencyExistsValidationErrorMessage("ApplicationUsers") });
            }

            if (applicationErrorValidationResult.Response.Permissions.NotNullAndHasRecords())
            {
                applicationErrorValidationResult.Errors.Add("Permissions", new List<string> { ValidatorUtilities.CreateDependencyExistsValidationErrorMessage("Permissions") });
            }

            if (applicationErrorValidationResult.Response.Roles.NotNullAndHasRecords())
            {
                applicationErrorValidationResult.Errors.Add("Roles", new List<string> { ValidatorUtilities.CreateDependencyExistsValidationErrorMessage("Roles") });
            }

            if (applicationErrorValidationResult.Response.RolePermissions.NotNullAndHasRecords())
            {
                applicationErrorValidationResult.Errors.Add("RolePermissions", new List<string> { ValidatorUtilities.CreateDependencyExistsValidationErrorMessage("RolePermissions") });
            }

            if (applicationErrorValidationResult.Response.ApplicationUserPermissions.NotNullAndHasRecords())
            {
                applicationErrorValidationResult.Errors.Add("ApplicationUserPermissions", new List<string> { ValidatorUtilities.CreateDependencyExistsValidationErrorMessage("ApplicationUserPermissions") });
            }
            
            return applicationErrorValidationResult;
        }

        private Dictionary<string, List<string>> AddRecordNotFoundErrorToErrorValidationResult(Dictionary<string, List<string>> errors)
        {
            return LogicUtilities.AddRecordNotFoundErrorToErrorValidationResult(errors, Constants.EntityFieldNames.Application, Constants.EntityFieldNames.ApplicationId);
        }

        private async Task<ErrorValidationResult<ApplicationDto>> _returnReadOnlyRecordErrorValidationResult()
        {
            var errorValidationResult = new ErrorValidationResult<ApplicationDto>();
            errorValidationResult.Errors.Add(Constants.EntityFieldNames.Application, new List<string> { ValidatorUtilities.CreateRecordIsReadOnlyValidationErrorMessage() });
            return errorValidationResult;
        }

        #endregion

        #region Private

        private async Task LogApplicationChange(SecurityDBContext dbContext, Application oldRecord, InsertUpdateApplicationRequest req) 
        {
            var newRecord = req.ToEntityOnInsert();
            
            // Only capture fields that actually changed, not the full entity graph
            var changeLog = new Dictionary<string, object?>();

            if (oldRecord.Name != newRecord.Name)
            {
                changeLog[nameof(Application.Name)] = newRecord.Name;
            }

            if (oldRecord.Description != newRecord.Description)
            {
                changeLog[nameof(Application.Description)] = newRecord.Description;
            }

            if (oldRecord.Active != newRecord.Active)
            {
                changeLog[nameof(Application.Active)] = newRecord.Active;
            }

            if (oldRecord.UpdatedBy != req.CurrentUser)
            {
                changeLog[nameof(Application.UpdatedBy)] = req.CurrentUser;
            }
            
            changeLog[nameof(Application.UpdatedOn)] = oldRecord.UpdatedOn;
            
            await dbContext.AuditLogs.AddAsync(new AuditLog {
                LogType = AuditLogLogTypes.Update,
                ReferenceType = EntityFieldNames.Application,
                ReferenceId = oldRecord.ApplicationId,
                Json = JsonSerializer.Serialize(changeLog),
                CreatedBy = req.CurrentUser,
                CreatedOn = CommonUtilities.GetDateTimeUtcNow()
            });
        }

        private async Task LogApplicationDelete(SecurityDBContext dbContext, Application record, string currentUser) 
        {
            var deleteLog = new Dictionary<string, object?>();
            deleteLog[nameof(Application.Name)] = record.Name;
            deleteLog[nameof(Application.Description)] = record.Description;
            deleteLog[nameof(Application.Active)] = record.Active;
            deleteLog[nameof(Application.ReadOnly)] = record.ReadOnly;
            deleteLog[nameof(Application.CreatedBy)] = record.CreatedBy;
            deleteLog[nameof(Application.CreatedOn)] = record.CreatedOn;
            deleteLog[nameof(Application.UpdatedBy)] = record.UpdatedBy;
            deleteLog[nameof(Application.UpdatedOn)] = record.UpdatedOn;

            await dbContext.AuditLogs.AddAsync(new AuditLog {
                LogType = AuditLogLogTypes.Delete,
                ReferenceType = EntityFieldNames.Application,
                ReferenceId = record.ApplicationId,
                Json = JsonSerializer.Serialize(deleteLog),
                CreatedBy = currentUser,
                CreatedOn = CommonUtilities.GetDateTimeUtcNow()
            });
        }

        #endregion
    }
}
