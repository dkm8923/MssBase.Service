using Contract.Security.Application;
using Contract.Security.ApplicationUser;
using Data.Security;
using Data.Security.Converters;
using Dto.Security.ApplicationUser;
using Dto.Security.ApplicationUser.Logic;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;
using Shared.Contracts.Logic.Validators;
using Shared.Models;

namespace Logic.Security.Logic
{
    public class ApplicationUserLogic : IApplicationUserLogic
    {
        private readonly IDatabaseConnectionStrings _connectionStrings;
        private readonly SecurityDBContextFactory _dbContextFactory;

        private IValidatorUtilities _validatorUtilities;

        private IValidator<FilterApplicationUserLogicRequest> _filterApplicationUserLogicRequestValidator;
        private IValidator<InsertUpdateApplicationUserRequest> _insertUpdateApplicationUserRequestValidator;

        public ApplicationUserLogic(
                            IDatabaseConnectionStrings connectionStrings,
                            IValidatorUtilities validatorUtilities,
                            IValidator<FilterApplicationUserLogicRequest> filterApplicationUserLogicRequestValidator,
                            IValidator<InsertUpdateApplicationUserRequest> insertUpdateApplicationUserRequestValidator
        )
        {
            _connectionStrings = connectionStrings;
            _dbContextFactory = new SecurityDBContextFactory(_connectionStrings);
            _validatorUtilities = validatorUtilities;
            _filterApplicationUserLogicRequestValidator = filterApplicationUserLogicRequestValidator;
            _insertUpdateApplicationUserRequestValidator = insertUpdateApplicationUserRequestValidator;
        }

        /// <summary>
        /// Retrieves a collection of application users based on the specified request parameters.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<ApplicationUserDto>>> GetAll(BaseLogicGet req)
        {
            var ret = await this.Filter(new FilterApplicationUserLogicRequest { IncludeInactive = req.IncludeInactive, CurrentUser = req.CurrentUser });
            return ret;
        }

        /// <summary>
        /// Retrieves an application user by its unique identifier.
        /// </summary>
        public async Task<ErrorValidationResult<ApplicationUserDto>> GetById(int applicationUserId, BaseLogicGet req)
        {
            var res = await this.Filter(new FilterApplicationUserLogicRequest { ApplicationUserIds = new List<int> { applicationUserId }, IncludeInactive = req.IncludeInactive, CurrentUser = req.CurrentUser });

            return new ErrorValidationResult<ApplicationUserDto> { Response = res.Response.FirstOrDefault() };
        }

        /// <summary>
        /// Filters application users based on the specified criteria.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<ApplicationUserDto>>> Filter(FilterApplicationUserLogicRequest req)
        {
            var errorValidationResult = await _validateApplicationUserFilter(req);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadOnly())
            {
                var records = dbContext.ApplicationUsers.AsQueryable().AsNoTracking();

                if (!req.IncludeInactive)
                {
                    records = records.Where(x => x.Active == true);
                }

                if (req.ApplicationUserIds != null && req.ApplicationUserIds.Count > 0)
                {
                    records = records.Where(x => req.ApplicationUserIds.Contains(x.ApplicationUserId));
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

                    if (req.Email != null)
                    {
                        records = records.Where(x => x.Email == req.Email);
                    }

                    if (req.FirstName != null)
                    {
                        records = records.Where(x => x.FirstName == req.FirstName);
                    }

                    if (req.LastName != null)
                    {
                        records = records.Where(x => x.LastName == req.LastName);
                    }

                    if (req.ApplicationId != null)
                    {
                        records = records.Where(x => x.ApplicationId == req.ApplicationId);
                    }
                }

                return new ErrorValidationResult<IEnumerable<ApplicationUserDto>> { Response = records.ToDtos() };
            }
        }

        /// <summary>
        /// Inserts a new application user into the data store.
        /// </summary>
        public async Task<ErrorValidationResult<ApplicationUserDto>> Insert(InsertUpdateApplicationUserRequest req, IApplicationLogic applicationLogic)
        {
            var errorValidationResult = await _validateApplicationUserOnInsertUpdate(applicationLogic, req, null);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = req.ToEntityOnInsert();

                await dbContext.ApplicationUsers.AddAsync(entity);
                await dbContext.SaveChangesAsync();

                return new ErrorValidationResult<ApplicationUserDto> { Response = entity.ToDto() };
            }
        }

        /// <summary>
        /// Updates the details of an existing application user.
        /// </summary>
        public async Task<ErrorValidationResult<ApplicationUserDto>> Update(int applicationUserId, InsertUpdateApplicationUserRequest req, IApplicationLogic applicationLogic)
        {
            var errorValidationResult = await _validateApplicationUserOnInsertUpdate(applicationLogic, req, applicationUserId);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.ApplicationUsers.FirstOrDefaultAsync(ent => ent.ApplicationUserId == applicationUserId);

                if (entity != null)
                {
                    entity = entity.UpdateEntityFromRequest(req);
                    await dbContext.SaveChangesAsync();
                    return new ErrorValidationResult<ApplicationUserDto> { Response = entity.ToDto() };
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
        public async Task<ErrorValidationResult> Delete(int applicationUserId)
        {
            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.ApplicationUsers.FirstOrDefaultAsync(ent => ent.ApplicationUserId == applicationUserId);
                var errorValidationResult = new ErrorValidationResult();

                if (entity != null)
                {
                    dbContext.ApplicationUsers.Remove(entity);

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

        private async Task<ErrorValidationResult<IEnumerable<ApplicationUserDto>>> _validateApplicationUserFilter(FilterApplicationUserLogicRequest req)
        {
            ValidationResult result = await _filterApplicationUserLogicRequestValidator.ValidateAsync(req);
            var errorValidationResult = _validatorUtilities.CreateDefaultValidationResponse<IEnumerable<ApplicationUserDto>>(result);
            return errorValidationResult;
        }

        private async Task<ErrorValidationResult<ApplicationUserDto>> _validateApplicationUserOnInsertUpdate(IApplicationLogic applicationLogic, InsertUpdateApplicationUserRequest req, int? applicationUserId = null)
        {
            ValidationResult result = await _insertUpdateApplicationUserRequestValidator.ValidateAsync(req);
            var errorValidationResult = _validatorUtilities.CreateDefaultValidationResponse<ApplicationUserDto>(result);

            if (errorValidationResult.Errors.Count == 0)
            {
                // Validate Application exists
                var applicationResponse = await applicationLogic.GetById(req.ApplicationId, new BaseLogicGet());
                
                if (applicationResponse.Response == null)
                {
                    errorValidationResult.Errors.Add("ApplicationId", new List<string> { _validatorUtilities.CreateRecordDoesNotExistValidationErrorMessage("ApplicationId") });
                    return errorValidationResult;
                }

                // Validate Application user email is unique
                var emailCheck = await this.Filter(new FilterApplicationUserLogicRequest { Email = req.Email });

                if (emailCheck.Errors.Count == 0 && emailCheck.Response.Count() > 0)
                {
                    if ((applicationUserId == null || applicationUserId == 0) || (emailCheck.Response.FirstOrDefault().ApplicationUserId != applicationUserId))
                    {
                        errorValidationResult.Errors.Add("Email", new List<string> { _validatorUtilities.CreateUniqueValidationErrorMessage("Email") });
                    }
                }
            }

            return errorValidationResult;
        }

        private Dictionary<string, List<string>> AddRecordNotFoundErrorToErrorValidationResult(Dictionary<string, List<string>> errors)
        {
            errors.Add("ApplicationUser", new List<string> { _validatorUtilities.CreateRecordDoesNotExistValidationErrorMessage("ApplicationUserId") });
            return errors;
        }

        #endregion
    }
}
