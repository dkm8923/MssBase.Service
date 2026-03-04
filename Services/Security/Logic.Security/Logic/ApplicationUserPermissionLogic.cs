using Contract.Security;
using Contract.Security.Application;
using Contract.Security.ApplicationUser;
using Contract.Security.ApplicationUserPermission;
using Contract.Security.Permission;
using Data.Security;
using Data.Security.Converters;
using Dto.Security.ApplicationUser.Logic;
using Dto.Security.ApplicationUserPermission;
using Dto.Security.ApplicationUserPermission.Logic;
using Dto.Security.Permission.Logic;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;
using Shared.Contracts.Logic.Validators;
using Shared.Models;

namespace Logic.Security.Logic
{
    public class ApplicationUserPermissionLogic : IApplicationUserPermissionLogic
    {
        private readonly ISecurityConnectionStrings _connectionStrings;
        private readonly SecurityDBContextFactory _dbContextFactory;

        private IValidatorUtilities _validatorUtilities;

        private IValidator<FilterApplicationUserPermissionLogicRequest> _filterApplicationUserPermissionLogicRequestValidator;
        private IValidator<InsertUpdateApplicationUserPermissionRequest> _insertUpdateApplicationUserPermissionRequestValidator;

        public ApplicationUserPermissionLogic(
                            ISecurityConnectionStrings connectionStrings,
                            IValidatorUtilities validatorUtilities,
                            IValidator<FilterApplicationUserPermissionLogicRequest> filterApplicationUserPermissionLogicRequestValidator,
                            IValidator<InsertUpdateApplicationUserPermissionRequest> insertUpdateApplicationUserPermissionRequestValidator
        )
        {
            _connectionStrings = connectionStrings;
            _dbContextFactory = new SecurityDBContextFactory(_connectionStrings);
            _validatorUtilities = validatorUtilities;
            _filterApplicationUserPermissionLogicRequestValidator = filterApplicationUserPermissionLogicRequestValidator;
            _insertUpdateApplicationUserPermissionRequestValidator = insertUpdateApplicationUserPermissionRequestValidator;
        }

        /// <summary>
        /// Retrieves a collection of application users based on the specified request parameters.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<ApplicationUserPermissionDto>>> GetAll(BaseLogicGet req)
        {
            var ret = await this.Filter(new FilterApplicationUserPermissionLogicRequest { IncludeInactive = req.IncludeInactive, CurrentUser = req.CurrentUser, IncludeRelated = req.IncludeRelated });
            return ret;
        }

        /// <summary>
        /// Retrieves an application user by its unique identifier.
        /// </summary>
        public async Task<ErrorValidationResult<ApplicationUserPermissionDto>> GetById(int applicationUserPermissionId, BaseLogicGet req)
        {
            var res = await this.Filter(new FilterApplicationUserPermissionLogicRequest { ApplicationUserPermissionIds = new List<int> { applicationUserPermissionId }, IncludeInactive = req.IncludeInactive, CurrentUser = req.CurrentUser, IncludeRelated = req.IncludeRelated });

            return new ErrorValidationResult<ApplicationUserPermissionDto> { Response = res.Response.FirstOrDefault() };
        }

        /// <summary>
        /// Filters application users based on the specified criteria.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<ApplicationUserPermissionDto>>> Filter(FilterApplicationUserPermissionLogicRequest req)
        {
            var errorValidationResult = await _validateApplicationUserPermissionFilter(req);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadOnly())
            {
                var records = dbContext.ApplicationUserPermissions.AsQueryable().AsNoTracking();

                if (!req.IncludeInactive)
                {
                    records = records.Where(x => x.Active == true);
                }

                // if (req.IncludeRelated)
                // {
                //     records = records.Include(applicationUserPermission => applicationUserPermission.ApplicationUserPermissionPermissions);
                // }

                if (req.ApplicationUserPermissionIds != null && req.ApplicationUserPermissionIds.Count > 0)
                {
                    records = records.Where(x => req.ApplicationUserPermissionIds.Contains(x.ApplicationUserPermissionId));
                }
                else
                {
                    if (req.CreatedBy != null)
                    {
                        records = records.Where(x => x.CreatedBy == req.CreatedBy);
                    }

                    if (req.CreatedOnDate != null)
                    {
                        records = records.Where(x => DateOnly.FromDateTime((DateTime)x.CreatedOn) == req.CreatedOnDate);
                    }

                    if (req.UpdatedBy != null)
                    {
                        records = records.Where(x => x.UpdatedBy == req.UpdatedBy);
                    }

                    if (req.UpdatedOnDate != null)
                    {
                        records = records.Where(x => DateOnly.FromDateTime((DateTime)x.UpdatedOn) == req.UpdatedOnDate);
                    }

                    if (req.ApplicationId != null)
                    {
                        records = records.Where(x => x.ApplicationId == req.ApplicationId);
                    }

                    if (req.ApplicationUserId != null)
                    {
                        records = records.Where(x => x.ApplicationUserId == req.ApplicationUserId);
                    }

                    if (req.PermissionId != null)
                    {
                        records = records.Where(x => x.PermissionId == req.PermissionId);
                    }
                }

                return new ErrorValidationResult<IEnumerable<ApplicationUserPermissionDto>> { Response = records.ToDtos() };
            }
        }

        /// <summary>
        /// Inserts a new application user into the data store.
        /// </summary>
        public async Task<ErrorValidationResult<ApplicationUserPermissionDto>> Insert(InsertUpdateApplicationUserPermissionRequest req, 
                                                                                      IApplicationLogic applicationLogic,
                                                                                      IApplicationUserLogic applicationUserLogic,
                                                                                      IPermissionLogic permissionLogic
                                                                                     )
        {
            var errorValidationResult = await _validateApplicationUserPermissionOnInsertUpdate(applicationLogic, applicationUserLogic, permissionLogic, req);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = req.ToEntityOnInsert();

                await dbContext.ApplicationUserPermissions.AddAsync(entity);
                await dbContext.SaveChangesAsync();

                return new ErrorValidationResult<ApplicationUserPermissionDto> { Response = entity.ToDto() };
            }
        }

        /// <summary>
        /// Updates the details of an existing application user.
        /// </summary>
        public async Task<ErrorValidationResult<ApplicationUserPermissionDto>> Update(int applicationUserPermissionId, 
                                                                                      InsertUpdateApplicationUserPermissionRequest req, 
                                                                                      IApplicationLogic applicationLogic,
                                                                                      IApplicationUserLogic applicationUserPermissionLogic,
                                                                                      IPermissionLogic permissionLogic
                                                                                     )
        {
            var errorValidationResult = await _validateApplicationUserPermissionOnInsertUpdate(applicationLogic, applicationUserPermissionLogic, permissionLogic, req);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.ApplicationUserPermissions.FirstOrDefaultAsync(ent => ent.ApplicationUserPermissionId == applicationUserPermissionId);

                if (entity != null)
                {
                    entity = entity.UpdateEntityFromRequest(req);
                    await dbContext.SaveChangesAsync();
                    return new ErrorValidationResult<ApplicationUserPermissionDto> { Response = entity.ToDto() };
                }
                else
                {
                    errorValidationResult.Errors = AddRecordNotFoundErrorToErrorValidationResult(errorValidationResult.Errors);
                    return errorValidationResult;
                }
            }
        }

        /// <summary>
        /// Deletes the application user with the specified identifier.
        /// </summary>
        public async Task<ErrorValidationResult> Delete(int applicationUserPermissionId)
        {
            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.ApplicationUserPermissions.FirstOrDefaultAsync(ent => ent.ApplicationUserPermissionId == applicationUserPermissionId);
                var errorValidationResult = new ErrorValidationResult();

                if (entity != null)
                {
                    dbContext.ApplicationUserPermissions.Remove(entity);

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

        private async Task<ErrorValidationResult<IEnumerable<ApplicationUserPermissionDto>>> _validateApplicationUserPermissionFilter(FilterApplicationUserPermissionLogicRequest req)
        {
            ValidationResult result = await _filterApplicationUserPermissionLogicRequestValidator.ValidateAsync(req);
            var errorValidationResult = _validatorUtilities.CreateDefaultValidationResponse<IEnumerable<ApplicationUserPermissionDto>>(result);
            return errorValidationResult;
        }

        private async Task<ErrorValidationResult<ApplicationUserPermissionDto>> _validateApplicationUserPermissionOnInsertUpdate(IApplicationLogic applicationLogic,
                                                                                                                                 IApplicationUserLogic applicationUserLogic,
                                                                                                                                 IPermissionLogic permissionLogic,         
                                                                                                                                 InsertUpdateApplicationUserPermissionRequest req
                                                                                                                                )
        {
            ValidationResult result = await _insertUpdateApplicationUserPermissionRequestValidator.ValidateAsync(req);
            var errorValidationResult = _validatorUtilities.CreateDefaultValidationResponse<ApplicationUserPermissionDto>(result);

            if (errorValidationResult.Errors.Count == 0)
            {
                // Validate Application exists
                var applicationResponse = await applicationLogic.GetById(req.ApplicationId, new BaseLogicGet());
                
                if (applicationResponse.Response == null)
                {
                    errorValidationResult.Errors.Add("ApplicationId", new List<string> { _validatorUtilities.CreateRecordDoesNotExistValidationErrorMessage("ApplicationId") });
                    return errorValidationResult;
                }

                // Validate ApplicationUser exists
                var applicationUserResponse = await applicationUserLogic.Filter(new FilterApplicationUserLogicRequest { ApplicationUserIds = new List<int> { req.ApplicationUserId }, ApplicationId = req.ApplicationId });

                if (applicationUserResponse.Response == null || applicationUserResponse.Response.Count() == 0)
                {
                    errorValidationResult.Errors.Add("ApplicationUserId", new List<string> { _validatorUtilities.CreateRecordDoesNotExistValidationErrorMessage("ApplicationUserId") });
                    return errorValidationResult;
                }

                // Validate Permission exists
                var permissionResponse = await permissionLogic.Filter(new FilterPermissionLogicRequest { PermissionIds = new List<int> { req.PermissionId }, ApplicationId = req.ApplicationId });

                if (permissionResponse.Response == null || permissionResponse.Response.Count() == 0)
                {
                    errorValidationResult.Errors.Add("PermissionId", new List<string> { _validatorUtilities.CreateRecordDoesNotExistValidationErrorMessage("PermissionId") });
                    return errorValidationResult;
                }
            }

            return errorValidationResult;
        }

        private Dictionary<string, List<string>> AddRecordNotFoundErrorToErrorValidationResult(Dictionary<string, List<string>> errors)
        {
            errors.Add("ApplicationUserPermission", new List<string> { _validatorUtilities.CreateRecordDoesNotExistValidationErrorMessage("ApplicationUserPermissionId") });
            return errors;
        }

        #endregion
    }
}
