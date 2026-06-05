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

        /// <summary>
        /// Retrieves a collection of Roles based on the specified request parameters.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<RoleDto>>> GetAll(BaseLogicGet req, CancellationToken cancellationToken = default)
        {
            var ret = await this.Filter(new FilterRoleLogicRequest { IncludeInactive = req.IncludeInactive, IncludeRelated = req.IncludeRelated, CurrentUser = req.CurrentUser }, cancellationToken);
            return ret;
        }

        /// <summary>
        /// Retrieves an Role by its unique identifier.
        /// </summary>
        public async Task<ErrorValidationResult<RoleDto>> GetById(int RoleId, BaseLogicGet req, CancellationToken cancellationToken = default)
        {
            var res = await this.Filter(new FilterRoleLogicRequest { RoleIds = new List<int> { RoleId }, IncludeInactive = req.IncludeInactive, IncludeRelated = req.IncludeRelated, CurrentUser = req.CurrentUser }, cancellationToken);

            return new ErrorValidationResult<RoleDto> { Response = res.Response.FirstOrDefault() };
        }

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

        /// <summary>
        /// Updates the details of an existing Role.
        /// </summary>
        public async Task<ErrorValidationResult<RoleDto>> Update(int RoleId, InsertUpdateRoleRequest req, IApplicationLogic applicationLogic)
        {
            var errorValidationResult = await _validateRoleOnInsertUpdate(applicationLogic, req, RoleId);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.Roles.FirstOrDefaultAsync(ent => ent.RoleId == RoleId);

                if (entity != null)
                {
                    entity = entity.UpdateEntityFromRequest(req);
                    await dbContext.SaveChangesAsync();
                    return new ErrorValidationResult<RoleDto> { Response = entity.ToDto() };
                }
                else
                {
                    errorValidationResult.Errors = AddRecordNotFoundErrorToErrorValidationResult(errorValidationResult.Errors);
                    return errorValidationResult;
                }
            }
        }

        /// <summary>
        /// Deletes the Role with the specified identifier.
        /// </summary>
        public async Task<ErrorValidationResult> Delete(int RoleId)
        {
            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.Roles.FirstOrDefaultAsync(ent => ent.RoleId == RoleId);
                var errorValidationResult = new ErrorValidationResult();

                if (entity != null)
                {
                    dbContext.Roles.Remove(entity);

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

        private async Task<ErrorValidationResult<IEnumerable<RoleDto>>> _validateRoleFilter(FilterRoleLogicRequest req)
        {
            ValidationResult result = await _filterRoleLogicRequestValidator.ValidateAsync(req);
            var errorValidationResult = ValidatorUtilities.CreateDefaultValidationResponse<IEnumerable<RoleDto>>(result);
            return errorValidationResult;
        }

        private async Task<ErrorValidationResult<RoleDto>> _validateRoleOnInsertUpdate(IApplicationLogic applicationLogic, InsertUpdateRoleRequest req, int? RoleId = null)
        {
            ValidationResult result = await _insertUpdateRoleRequestValidator.ValidateAsync(req);
            var errorValidationResult = ValidatorUtilities.CreateDefaultValidationResponse<RoleDto>(result);

            if (errorValidationResult.Errors.Count == 0)
            {
                // Validate Application exists
                var applicationResponse = await applicationLogic.GetById(req.ApplicationId, new BaseLogicGet { IncludeInactive = true });
                
                if (applicationResponse.Response == null)
                {
                    errorValidationResult.Errors.Add(Constants.EntityFieldNames.ApplicationId, new List<string> { ValidatorUtilities.CreateRecordDoesNotExistValidationErrorMessage(Constants.EntityFieldNames.ApplicationId) });
                    return errorValidationResult;
                }

                // Validate Role name is unique
                var nameCheck = await this.Filter(new FilterRoleLogicRequest { Name = req.Name });

                if (nameCheck.Errors.Count == 0 && nameCheck.Response.Count() > 0)
                {
                    if ((RoleId == null || RoleId == 0) || (nameCheck.Response.FirstOrDefault().RoleId != RoleId))
                    {
                        errorValidationResult.Errors.Add(Constants.EntityFieldNames.Name, new List<string> { ValidatorUtilities.CreateUniqueValidationErrorMessage(Constants.EntityFieldNames.Name) });
                    }
                }
            }

            return errorValidationResult;
        }

        private Dictionary<string, List<string>> AddRecordNotFoundErrorToErrorValidationResult(Dictionary<string, List<string>> errors)
        {
            errors.Add(Constants.EntityFieldNames.Role, new List<string> { ValidatorUtilities.CreateRecordDoesNotExistValidationErrorMessage(Constants.EntityFieldNames.RoleId) });
            return errors;
        }

        #endregion
    }
}
