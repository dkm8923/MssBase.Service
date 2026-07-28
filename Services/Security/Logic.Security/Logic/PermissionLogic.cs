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

        /// <summary>
        /// Retrieves a collection of Permissions based on the specified request parameters.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<PermissionDto>>> GetAll(BaseLogicGet req, CancellationToken cancellationToken = default)
        {
            var ret = await this.Filter(new FilterPermissionLogicRequest { IncludeInactive = req.IncludeInactive, IncludeReadOnly = req.IncludeReadOnly, CurrentUser = req.CurrentUser }, cancellationToken);
            return ret;
        }

        /// <summary>
        /// Retrieves an Permission by its unique identifier.
        /// </summary>
        public async Task<ErrorValidationResult<PermissionDto>> GetById(int PermissionId, BaseLogicGet req, CancellationToken cancellationToken = default)
        {
            var res = await this.Filter(new FilterPermissionLogicRequest { PermissionIds = new List<int> { PermissionId }, IncludeInactive = req.IncludeInactive, IncludeReadOnly = req.IncludeReadOnly, CurrentUser = req.CurrentUser }, cancellationToken);

            return new ErrorValidationResult<PermissionDto> { Response = res.Response.FirstOrDefault() };
        }

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

                entity = entity.UpdateEntityFromRequest(req);
                await dbContext.SaveChangesAsync();
                return new ErrorValidationResult<PermissionDto> { Response = entity.ToDto() };
            }
        }

        /// <summary>
        /// Deletes the Permission with the specified identifier.
        /// </summary>
        public async Task<ErrorValidationResult> Delete(int permissionId)
        {
            var errorValidationResult = await _validatePermissionOnDelete(permissionId);

            if (errorValidationResult.Errors.Count == 0)
            {
                using (var dbContext = _dbContextFactory.CreateContextReadWrite())
                {
                    var entity = await dbContext.Permissions.FirstOrDefaultAsync(ent => ent.PermissionId == permissionId && !ent.ReadOnly);
                    dbContext.Permissions.Remove(entity);
                    await dbContext.SaveChangesAsync();
                    errorValidationResult.Response = null;
                }
            }

            return errorValidationResult;
        }

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
    }
}
