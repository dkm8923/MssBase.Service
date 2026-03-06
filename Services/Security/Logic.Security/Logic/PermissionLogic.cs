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
        public async Task<ErrorValidationResult<IEnumerable<PermissionDto>>> GetAll(BaseLogicGet req)
        {
            var ret = await this.Filter(new FilterPermissionLogicRequest { IncludeInactive = req.IncludeInactive, CurrentUser = req.CurrentUser });
            return ret;
        }

        /// <summary>
        /// Retrieves an Permission by its unique identifier.
        /// </summary>
        public async Task<ErrorValidationResult<PermissionDto>> GetById(int PermissionId, BaseLogicGet req)
        {
            var res = await this.Filter(new FilterPermissionLogicRequest { PermissionIds = new List<int> { PermissionId }, IncludeInactive = req.IncludeInactive, CurrentUser = req.CurrentUser });

            return new ErrorValidationResult<PermissionDto> { Response = res.Response.FirstOrDefault() };
        }

        /// <summary>
        /// Filters Permissions based on the specified criteria.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<PermissionDto>>> Filter(FilterPermissionLogicRequest req)
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

                return new ErrorValidationResult<IEnumerable<PermissionDto>> { Response = query.ToDtos() };
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

                if (entity != null)
                {
                    entity = entity.UpdateEntityFromRequest(req);
                    await dbContext.SaveChangesAsync();
                    return new ErrorValidationResult<PermissionDto> { Response = entity.ToDto() };
                }
                else
                {
                    errorValidationResult.Errors = AddRecordNotFoundErrorToErrorValidationResult(errorValidationResult.Errors);
                    return errorValidationResult;
                }
            }
        }

        /// <summary>
        /// Deletes the Permission with the specified identifier.
        /// </summary>
        public async Task<ErrorValidationResult> Delete(int PermissionId)
        {
            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.Permissions.FirstOrDefaultAsync(ent => ent.PermissionId == PermissionId);
                var errorValidationResult = new ErrorValidationResult();

                if (entity != null)
                {
                    dbContext.Permissions.Remove(entity);

                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    errorValidationResult.Errors = AddRecordNotFoundErrorToErrorValidationResult(errorValidationResult.Errors);
                }

                return errorValidationResult;
            }
        }

        #region Validation

        private async Task<ErrorValidationResult<IEnumerable<PermissionDto>>> _validatePermissionFilter(FilterPermissionLogicRequest req)
        {
            ValidationResult result = await _filterPermissionLogicRequestValidator.ValidateAsync(req);
            var errorValidationResult = ValidatorUtilities.CreateDefaultValidationResponse<IEnumerable<PermissionDto>>(result);
            return errorValidationResult;
        }

        private async Task<ErrorValidationResult<PermissionDto>> _validatePermissionOnInsertUpdate(IApplicationLogic applicationLogic, InsertUpdatePermissionRequest req, int? PermissionId = null)
        {
            ValidationResult result = await _insertUpdatePermissionRequestValidator.ValidateAsync(req);
            var errorValidationResult = ValidatorUtilities.CreateDefaultValidationResponse<PermissionDto>(result);

            if (errorValidationResult.Errors.Count == 0)
            {
                // Validate Application exists
                var applicationResponse = await applicationLogic.GetById(req.ApplicationId, new BaseLogicGet());
                
                if (applicationResponse.Response == null)
                {
                    errorValidationResult.Errors.Add("ApplicationId", new List<string> { ValidatorUtilities.CreateRecordDoesNotExistValidationErrorMessage("ApplicationId") });
                    return errorValidationResult;
                }

                // Validate Permission name is unique
                var nameCheck = await this.Filter(new FilterPermissionLogicRequest { Name = req.Name });

                if (nameCheck.Errors.Count == 0 && nameCheck.Response.Count() > 0)
                {
                    if ((PermissionId == null || PermissionId == 0) || (nameCheck.Response.FirstOrDefault().PermissionId != PermissionId))
                    {
                        errorValidationResult.Errors.Add("Name", new List<string> { ValidatorUtilities.CreateUniqueValidationErrorMessage("Name") });
                    }
                }
            }

            return errorValidationResult;
        }

        private Dictionary<string, List<string>> AddRecordNotFoundErrorToErrorValidationResult(Dictionary<string, List<string>> errors)
        {
            errors.Add("Permission", new List<string> { ValidatorUtilities.CreateRecordDoesNotExistValidationErrorMessage("PermissionId") });
            return errors;
        }

        #endregion
    }
}
