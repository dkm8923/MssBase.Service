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

        /// <summary>
        /// Retrieves a collection of application users based on the specified request parameters.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<ApplicationUserRoleDto>>> GetAll(BaseLogicGet req)
        {
            var ret = await this.Filter(new FilterApplicationUserRoleLogicRequest { IncludeInactive = req.IncludeInactive, CurrentUser = req.CurrentUser, IncludeRelated = req.IncludeRelated });
            return ret;
        }

        /// <summary>
        /// Retrieves an application user by its unique identifier.
        /// </summary>
        public async Task<ErrorValidationResult<ApplicationUserRoleDto>> GetById(int applicationUserRoleId, BaseLogicGet req)
        {
            var res = await this.Filter(new FilterApplicationUserRoleLogicRequest { ApplicationUserRoleIds = new List<int> { applicationUserRoleId }, IncludeInactive = req.IncludeInactive, CurrentUser = req.CurrentUser, IncludeRelated = req.IncludeRelated });

            return new ErrorValidationResult<ApplicationUserRoleDto> { Response = res.Response.FirstOrDefault() };
        }

        /// <summary>
        /// Filters application users based on the specified criteria.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<ApplicationUserRoleDto>>> Filter(FilterApplicationUserRoleLogicRequest req)
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
                query = query.ApplyAuditableFilters(req);

                // if (req.IncludeRelated)
                // {
                //     query = query.Include(applicationUserRole => applicationUserRole.ApplicationUserRoleRoles);
                // }

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

                return new ErrorValidationResult<IEnumerable<ApplicationUserRoleDto>> { Response = query.ToDtos() };
            }
        }

        /// <summary>
        /// Inserts a new application user into the data store.
        /// </summary>
        public async Task<ErrorValidationResult<ApplicationUserRoleDto>> Insert(InsertUpdateApplicationUserRoleRequest req, 
                                                                                      IApplicationLogic applicationLogic,
                                                                                      IApplicationUserLogic applicationUserLogic,
                                                                                      IRoleLogic permissionLogic
                                                                                     )
        {
            var errorValidationResult = await _validateApplicationUserRoleOnInsertUpdate(applicationLogic, applicationUserLogic, permissionLogic, req);
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

        /// <summary>
        /// Updates the details of an existing application user.
        /// </summary>
        public async Task<ErrorValidationResult<ApplicationUserRoleDto>> Update(int applicationUserRoleId, 
                                                                                      InsertUpdateApplicationUserRoleRequest req, 
                                                                                      IApplicationLogic applicationLogic,
                                                                                      IApplicationUserLogic applicationUserRoleLogic,
                                                                                      IRoleLogic permissionLogic
                                                                                     )
        {
            var errorValidationResult = await _validateApplicationUserRoleOnInsertUpdate(applicationLogic, applicationUserRoleLogic, permissionLogic, req);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.ApplicationUserRoles.FirstOrDefaultAsync(ent => ent.ApplicationUserRoleId == applicationUserRoleId);

                if (entity != null)
                {
                    entity = entity.UpdateEntityFromRequest(req);
                    await dbContext.SaveChangesAsync();
                    return new ErrorValidationResult<ApplicationUserRoleDto> { Response = entity.ToDto() };
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
        public async Task<ErrorValidationResult> Delete(int applicationUserRoleId)
        {
            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.ApplicationUserRoles.FirstOrDefaultAsync(ent => ent.ApplicationUserRoleId == applicationUserRoleId);
                var errorValidationResult = new ErrorValidationResult();

                if (entity != null)
                {
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
                                                                                                                                 InsertUpdateApplicationUserRoleRequest req
                                                                                                                                )
        {
            ValidationResult result = await _insertUpdateApplicationUserRoleRequestValidator.ValidateAsync(req);
            var errorValidationResult = ValidatorUtilities.CreateDefaultValidationResponse<ApplicationUserRoleDto>(result);

            if (errorValidationResult.Errors.Count == 0)
            {
                // Validate Application exists
                var applicationResponse = await applicationLogic.GetById(req.ApplicationId, new BaseLogicGet { IncludeInactive = true });
                
                if (applicationResponse.Response == null)
                {
                    errorValidationResult.Errors.Add("ApplicationId", new List<string> { ValidatorUtilities.CreateRecordDoesNotExistValidationErrorMessage("ApplicationId") });
                    return errorValidationResult;
                }

                // Validate ApplicationUser exists
                var applicationUserResponse = await applicationUserLogic.Filter(new FilterApplicationUserLogicRequest { ApplicationUserIds = new List<int> { req.ApplicationUserId }, ApplicationId = req.ApplicationId, IncludeInactive = true });

                if (applicationUserResponse.Response == null || applicationUserResponse.Response.Count() == 0)
                {
                    errorValidationResult.Errors.Add("ApplicationUserId", new List<string> { ValidatorUtilities.CreateRecordDoesNotExistValidationErrorMessage("ApplicationUserId") });
                    return errorValidationResult;
                }

                // Validate Role exists
                var roleResponse = await roleLogic.Filter(new FilterRoleLogicRequest { RoleIds = new List<int> { req.RoleId }, ApplicationId = req.ApplicationId, IncludeInactive = true });

                if (roleResponse.Response == null || roleResponse.Response.Count() == 0)
                {
                    errorValidationResult.Errors.Add("RoleId", new List<string> { ValidatorUtilities.CreateRecordDoesNotExistValidationErrorMessage("RoleId") });
                    return errorValidationResult;
                }
            }

            return errorValidationResult;
        }

        private Dictionary<string, List<string>> AddRecordNotFoundErrorToErrorValidationResult(Dictionary<string, List<string>> errors)
        {
            errors.Add("ApplicationUserRole", new List<string> { ValidatorUtilities.CreateRecordDoesNotExistValidationErrorMessage("ApplicationUserRoleId") });
            return errors;
        }

        #endregion
    }
}
