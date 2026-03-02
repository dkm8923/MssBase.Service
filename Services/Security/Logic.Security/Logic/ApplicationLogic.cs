using Contract.Security;
using Contract.Security.Application;
using Data.Security;
using Data.Security.Converters;
using Dto.Security.Application;
using Dto.Security.Application.Logic;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Logic.Validators;
using Shared.Models;
using Shared.Contracts;

namespace Logic.Security.Logic
{
    public class ApplicationLogic : IApplicationLogic
    {
        private readonly ISecurityConnectionStrings _connectionStrings;
        private readonly SecurityDBContextFactory _dbContextFactory;

        private IValidatorUtilities _validatorUtilities;

        private IValidator<FilterApplicationLogicRequest> _filterApplicationLogicRequestValidator;
        private IValidator<InsertUpdateApplicationRequest> _insertUpdateApplicationRequestValidator;

        public ApplicationLogic(
                            ISecurityConnectionStrings connectionStrings,
                            IValidatorUtilities validatorUtilities,
                            IValidator<FilterApplicationLogicRequest> filterApplicationLogicRequestValidator,
                            IValidator<InsertUpdateApplicationRequest> insertUpdateApplicationRequestValidator
        )
        {
            _connectionStrings = connectionStrings;
            _dbContextFactory = new SecurityDBContextFactory(_connectionStrings);
            _validatorUtilities = validatorUtilities;
            _filterApplicationLogicRequestValidator = filterApplicationLogicRequestValidator;
            _insertUpdateApplicationRequestValidator = insertUpdateApplicationRequestValidator;
        }

        /// <summary>
        /// Retrieves a collection of applications based on the specified request parameters.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<ApplicationDto>>> GetAll(BaseLogicGet req)
        {
            var ret = await this.Filter(new FilterApplicationLogicRequest { IncludeInactive = req.IncludeInactive, CurrentUser = req.CurrentUser, IncludeRelated = req.IncludeRelated });
            return ret;
        }

        /// <summary>
        /// Retrieves an application by its unique identifier.
        /// </summary>
        public async Task<ErrorValidationResult<ApplicationDto>> GetById(int applicationId, BaseLogicGet req)
        {
            var res = await this.Filter(new FilterApplicationLogicRequest { ApplicationIds = new List<int> { applicationId }, IncludeInactive = req.IncludeInactive, CurrentUser = req.CurrentUser, IncludeRelated = req.IncludeRelated });

            return new ErrorValidationResult<ApplicationDto> { Response = res.Response.FirstOrDefault() };
        }

        /// <summary>
        /// Filters applications based on the specified criteria.
        /// </summary>
        public async Task<ErrorValidationResult<IEnumerable<ApplicationDto>>> Filter(FilterApplicationLogicRequest req)
        {
            var errorValidationResult = await _validateApplicationFilter(req);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadOnly())
            {
                var records = dbContext.Applications.AsQueryable().AsNoTracking();

                if (!req.IncludeInactive)
                {
                    records = records.Where(x => x.Active == true);
                }

                // var query = _repositoryContext.Contacts.AsNoTracking().AsQueryable()
                //     .Include(contact => contact.ContactToAddressLinks).ThenInclude(row => row.Address)
                //     .Include(contact => contact.ContactToPhoneNumberLinks).ThenInclude(row => row.PhoneNumber)
                //     .Include(contact => contact.ContactToCompanyLinks).ThenInclude(row => row.Company)
                //     .Include(contact => contact.ContactToCommonNoteLinks).ThenInclude(row => row.CommonNote)
                //     .Where(q => q.Id == id)
                //     .SingleOrDefaultAsync();

                if (req.IncludeRelated)
                {
                    records = records.Include(application => application.ApplicationUsers)
                                     .Include(application => application.Permissions)
                                     .Include(application => application.Roles)
                                     .Include(application => application.RolePermissions)
                                     .Include(application => application.ApplicationUserPermissions);
                }

                if (req.ApplicationIds != null && req.ApplicationIds.Count > 0)
                {
                    records = records.Where(x => req.ApplicationIds.Contains(x.ApplicationId));
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

                    if (req.Name != null)
                    {
                        records = records.Where(x => x.Name == req.Name);
                    }
                }

                return new ErrorValidationResult<IEnumerable<ApplicationDto>> { Response = records.ToDtos() };
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
                var entity = await dbContext.Applications.FirstOrDefaultAsync(ent => ent.ApplicationId == applicationId);

                if (entity != null)
                {
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
        public async Task<ErrorValidationResult<ApplicationDto>> Delete(int applicationId)
        {
            var errorValidationResult = await _validateApplicationOnDelete(applicationId);

            if (errorValidationResult.Errors.Count == 0)
            {
                using (var dbContext = _dbContextFactory.CreateContextReadWrite())
                {
                    var entity = await dbContext.Applications.FirstOrDefaultAsync(ent => ent.ApplicationId == applicationId);
                    dbContext.Applications.Remove(entity);
                    await dbContext.SaveChangesAsync();
                    errorValidationResult.Response = null;
                }
            }

            return errorValidationResult;
        }

        #region Validation

        private async Task<ErrorValidationResult<IEnumerable<ApplicationDto>>> _validateApplicationFilter(FilterApplicationLogicRequest req)
        {
            ValidationResult result = await _filterApplicationLogicRequestValidator.ValidateAsync(req);
            var errorValidationResult = _validatorUtilities.CreateDefaultValidationResponse<IEnumerable<ApplicationDto>>(result);
            return errorValidationResult;
        }

        private async Task<ErrorValidationResult<ApplicationDto>> _validateApplicationOnInsertUpdate(InsertUpdateApplicationRequest req, int? applicationId = null)
        {
            ValidationResult result = await _insertUpdateApplicationRequestValidator.ValidateAsync(req);
            var errorValidationResult = _validatorUtilities.CreateDefaultValidationResponse<ApplicationDto>(result);

            if (errorValidationResult.Errors.Count == 0)
            {
                // Validate Application name is unique
                var nameCheck = await this.Filter(new FilterApplicationLogicRequest { Name = req.Name });

                if (nameCheck.Errors.Count == 0 && nameCheck.Response.Count() > 0)
                {
                    if ((applicationId == null || applicationId == 0) || (nameCheck.Response.FirstOrDefault().ApplicationId != applicationId))
                    {
                        errorValidationResult.Errors.Add("Name", new List<string> { _validatorUtilities.CreateUniqueValidationErrorMessage("Name") });
                    }
                }
            }

            return errorValidationResult;
        }

        private async Task<ErrorValidationResult<ApplicationDto>> _validateApplicationOnDelete(int applicationId)
        {
            var applicationErrorValidationResult = await GetById(applicationId, new BaseLogicGet { IncludeInactive = true, IncludeRelated = true });

            if (applicationErrorValidationResult.Response == null)
            {
                //application for given id does not exist
                applicationErrorValidationResult.Errors = AddRecordNotFoundErrorToErrorValidationResult(applicationErrorValidationResult.Errors);
                return applicationErrorValidationResult;
            }

            //verify no dependencies exist on application record
            if (applicationErrorValidationResult.Response.ApplicationUsers.Count() > 0)
            {
                applicationErrorValidationResult.Errors.Add("ApplicationUsers", new List<string> { _validatorUtilities.CreateDependencyExistsValidationErrorMessage("ApplicationUsers") });
            }

            if (applicationErrorValidationResult.Response.Permissions.Count() > 0)
            {
                applicationErrorValidationResult.Errors.Add("Permissions", new List<string> { _validatorUtilities.CreateDependencyExistsValidationErrorMessage("Permissions") });
            }

            if (applicationErrorValidationResult.Response.Roles.Count() > 0)
            {
                applicationErrorValidationResult.Errors.Add("Roles", new List<string> { _validatorUtilities.CreateDependencyExistsValidationErrorMessage("Roles") });
            }

            if (applicationErrorValidationResult.Response.RolePermissions.Count() > 0)
            {
                applicationErrorValidationResult.Errors.Add("RolePermissions", new List<string> { _validatorUtilities.CreateDependencyExistsValidationErrorMessage("RolePermissions") });
            }

            if (applicationErrorValidationResult.Response.ApplicationUserPermissions.Count() > 0)
            {
                applicationErrorValidationResult.Errors.Add("ApplicationUserPermissions", new List<string> { _validatorUtilities.CreateDependencyExistsValidationErrorMessage("ApplicationUserPermissions") });
            }

            return applicationErrorValidationResult;
        }

        private Dictionary<string, List<string>> AddRecordNotFoundErrorToErrorValidationResult(Dictionary<string, List<string>> errors)
        {
            errors.Add("Application", new List<string> { _validatorUtilities.CreateRecordDoesNotExistValidationErrorMessage("ApplicationId") });
            return errors;
        }

        #endregion
    }
}
