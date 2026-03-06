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

        /// <summary>
        /// Retrieves a collection of role permissions based on the specified request parameters.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<RolePermissionDto>>> GetAll(BaseLogicGet req)
        {
            var ret = await this.Filter(new FilterRolePermissionLogicRequest { IncludeInactive = req.IncludeInactive, CurrentUser = req.CurrentUser, IncludeRelated = req.IncludeRelated });
            return ret;
        }

        /// <summary>
        /// Retrieves a role permission by its unique identifier.
        /// </summary>
        public async Task<ErrorValidationResult<RolePermissionDto>> GetById(int rolePermissionId, BaseLogicGet req)
        {
            var res = await this.Filter(new FilterRolePermissionLogicRequest { RolePermissionIds = new List<int> { rolePermissionId }, IncludeInactive = req.IncludeInactive, CurrentUser = req.CurrentUser, IncludeRelated = req.IncludeRelated });

            return new ErrorValidationResult<RolePermissionDto> { Response = res.Response.FirstOrDefault() };
        }

        /// <summary>
        /// Filters role permissions based on the specified criteria.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<RolePermissionDto>>> Filter(FilterRolePermissionLogicRequest req)
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
                query = query.ApplyAuditableFilters(req);

                // if (req.IncludeRelated)
                // {
                //     query = query.Include(rolePermission => rolePermission.RolePermissionPermissions);
                // }

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

                return new ErrorValidationResult<IEnumerable<RolePermissionDto>> { Response = query.ToDtos() };
            }
        }

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
            var errorValidationResult = await _validateRolePermissionOnInsertUpdate(applicationLogic, roleLogic, permissionLogic, req);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.RolePermissions.FirstOrDefaultAsync(ent => ent.RolePermissionId == rolePermissionId);

                if (entity != null)
                {
                    entity = entity.UpdateEntityFromRequest(req);
                    await dbContext.SaveChangesAsync();
                    return new ErrorValidationResult<RolePermissionDto> { Response = entity.ToDto() };
                }
                else
                {
                    errorValidationResult.Errors = AddRecordNotFoundErrorToErrorValidationResult(errorValidationResult.Errors);
                    return errorValidationResult;
                }
            }
        }

        /// <summary>
        /// Deletes the role permission with the specified identifier.
        /// </summary>
        public async Task<ErrorValidationResult> Delete(int rolePermissionId)
        {
            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.RolePermissions.FirstOrDefaultAsync(ent => ent.RolePermissionId == rolePermissionId);
                var errorValidationResult = new ErrorValidationResult();

                if (entity != null)
                {
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
                                                                                                           InsertUpdateRolePermissionRequest req
                                                                                                        )
        {
            ValidationResult result = await _insertUpdateRolePermissionRequestValidator.ValidateAsync(req);
            var errorValidationResult = ValidatorUtilities.CreateDefaultValidationResponse<RolePermissionDto>(result);

            if (errorValidationResult.Errors.Count == 0)
            {
                // Validate Application exists
                var applicationResponse = await applicationLogic.GetById(req.ApplicationId, new BaseLogicGet());
                
                if (applicationResponse.Response == null)
                {
                    errorValidationResult.Errors.Add("ApplicationId", new List<string> { ValidatorUtilities.CreateRecordDoesNotExistValidationErrorMessage("ApplicationId") });
                    return errorValidationResult;
                }

                // Validate Role exists
                var roleResponse = await roleLogic.Filter(new FilterRoleLogicRequest { RoleIds = new List<int> { req.RoleId }, ApplicationId = req.ApplicationId });

                if (roleResponse.Response == null || roleResponse.Response.Count() == 0)
                {
                    errorValidationResult.Errors.Add("RoleId", new List<string> { ValidatorUtilities.CreateRecordDoesNotExistValidationErrorMessage("RoleId") });
                    return errorValidationResult;
                }

                // Validate Permission exists
                var permissionResponse = await permissionLogic.Filter(new FilterPermissionLogicRequest { PermissionIds = new List<int> { req.PermissionId }, ApplicationId = req.ApplicationId });

                if (permissionResponse.Response == null || permissionResponse.Response.Count() == 0)
                {
                    errorValidationResult.Errors.Add("PermissionId", new List<string> { ValidatorUtilities.CreateRecordDoesNotExistValidationErrorMessage("PermissionId") });
                    return errorValidationResult;
                }
            }

            return errorValidationResult;
        }

        private Dictionary<string, List<string>> AddRecordNotFoundErrorToErrorValidationResult(Dictionary<string, List<string>> errors)
        {
            errors.Add("RolePermission", new List<string> { ValidatorUtilities.CreateRecordDoesNotExistValidationErrorMessage("RolePermissionId") });
            return errors;
        }

        public Task<ErrorValidationResult<RolePermissionDto>> Insert(InsertUpdateRolePermissionRequest req)
        {
            throw new NotImplementedException();
        }

        public Task<ErrorValidationResult<RolePermissionDto>> Update(int rolePermissionId, InsertUpdateRolePermissionRequest req)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
