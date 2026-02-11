using Contract.Common;
using Contract.Common.Unit;
using Data.Common;
using Data.Common.Converters;
using Dto.Common.Unit;
using Dto.Common.Unit.Logic;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Logic.Validators;
using Shared.Models;
using Shared.Contracts; 

namespace Logic.Common.Logic
{
    public class UnitLogic : IUnitLogic
    {
        private readonly IDatabaseConnectionStrings _connectionStrings;
        private readonly CommonDBContextFactory _dbContextFactory;

        private IValidatorUtilities _validatorUtilities;

        private IValidator<FilterUnitLogicRequest> _filterUnitLogicRequestValidator;
        private IValidator<InsertUpdateUnitRequest> _insertUpdateUnitRequestValidator;

        public UnitLogic(
                            IDatabaseConnectionStrings connectionStrings,
                            IValidatorUtilities validatorUtilities,
                            IValidator<FilterUnitLogicRequest> filterUnitLogicRequestValidator,
                            IValidator<InsertUpdateUnitRequest> insertUpdateUnitRequestValidator
        )
        {
            _connectionStrings = connectionStrings;
            _dbContextFactory = new CommonDBContextFactory(_connectionStrings);
            _validatorUtilities = validatorUtilities;
            _filterUnitLogicRequestValidator = filterUnitLogicRequestValidator;
            _insertUpdateUnitRequestValidator = insertUpdateUnitRequestValidator;
        }

        /// <summary>
        /// Retrieves a collection of units based on the specified request parameters.
        /// </summary>
        /// <param name="req">The request parameters that determine which units to retrieve. If <see cref="BaseLogicGet.IncludeInactive"/>
        /// is <see langword="false"/>, only active units are included.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see
        /// cref="ErrorValidationResult{T}"/> with a collection of <see cref="UnitDto"/> objects matching the request
        /// criteria.</returns>
        public async Task<ErrorValidationResult<IEnumerable<UnitDto>>> GetAll(BaseLogicGet req, bool includeRelated = false)
        {
            var ret = await this.Filter(new FilterUnitLogicRequest { IncludeInactive = req.IncludeInactive, IncludeRelated = includeRelated, CurrentUser = req.CurrentUser });
            return ret;
        }

        /// <summary>
        /// Retrieves a unit by its unique identifier and returns the result as a UnitDto wrapped in an
        /// ErrorValidationResult.
        /// </summary>
        /// <remarks>If the specified unit is not found or does not meet the criteria defined in the
        /// request, the Response property of the result will be null. The method does not modify the
        /// database.</remarks>
        /// <param name="UnitId">The unique identifier of the unit to retrieve.</param>
        /// <param name="req">An object specifying additional retrieval options, such as whether to include inactive units.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an ErrorValidationResult with
        /// the requested UnitDto if found; otherwise, the Response property is null.</returns>
        public async Task<ErrorValidationResult<UnitDto>> GetById(int unitId, BaseLogicGet req, bool includeRelated = false)
        {
            var res = await this.Filter(new FilterUnitLogicRequest { UnitIds = new List<int> { unitId }, IncludeInactive = req.IncludeInactive, IncludeRelated = includeRelated, CurrentUser = req.CurrentUser });

            return new ErrorValidationResult<UnitDto> { Response = res.Response.FirstOrDefault() };
        }

        /// <summary>
        /// Filters units based on the specified criteria and returns the matching results along with any validation
        /// errors.
        /// </summary>
        /// <remarks>If the filter criteria are invalid, the result will contain validation errors and no
        /// data will be returned. Only active units are included by default unless the IncludeInactive flag is set to
        /// true in the request.</remarks>
        /// <param name="req">The filter criteria to apply when searching for units. Contains optional fields for various unit attributes
        /// and filter options.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an ErrorValidationResult object
        /// with a collection of UnitDto objects that match the filter criteria. If validation errors occur, the Errors
        /// property will contain details and the Response may be null or empty.</returns>
        public async Task<ErrorValidationResult<IEnumerable<UnitDto>>> Filter(FilterUnitLogicRequest req)
        {
            //TODO: Add filter validation to notify user to not use other filter parms if unitids is populated...
            var errorValidationResult = await _validateUnitFilter(req);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadOnly())
            {
                var records = dbContext.Units.AsQueryable().AsNoTracking();

                if (!req.IncludeInactive)
                {
                    records = records.Where(x => x.Active == true);
                }

                if (req.IncludeRelated)
                {
                    if (!req.IncludeInactive)
                    {
                        records = records.Include(q => q.UnitGroupColumns.Where(ugc => ugc.Active == true));
                    }
                    else
                    {
                        records = records.Include(q => q.UnitGroupColumns);
                    }
                }

                if (req.UnitIds != null && req.UnitIds.Count > 0)
                {
                    records = records.Where(x => req.UnitIds.Contains(x.UnitId));
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

                    if (req.UnitCode != null)
                    {
                        records = records.Where(x => x.UnitCode == req.UnitCode);
                    }

                    if (req.UnitName != null)
                    {
                        records = records.Where(x => x.UnitName == req.UnitName);
                    }

                    if (req.OriginSystem != null)
                    {
                        records = records.Where(x => x.OriginSystem == req.OriginSystem);
                    }

                    if (req.UnitDefinitionIdUnitQty != null)
                    {
                        records = records.Where(x => x.UnitDefinitionIdUnitQty == req.UnitDefinitionIdUnitQty);
                    }

                    if (req.UnitDefinitionIdUnitValue != null)
                    {
                        records = records.Where(x => x.UnitDefinitionIdUnitValue == req.UnitDefinitionIdUnitValue);
                    }

                    if (req.ValueTypeName != null)
                    {
                        records = records.Where(x => x.ValueTypeName == req.ValueTypeName);
                    }

                    if (req.UnitPrepQuery != null)
                    {
                        records = records.Where(x => x.UnitPrepQuery == req.UnitPrepQuery);
                    }

                    if (req.UnitHeaderQuery != null)
                    {
                        records = records.Where(x => x.UnitHeaderQuery == req.UnitHeaderQuery);
                    }

                    if (req.UnitUpdateQuery != null)
                    {
                        records = records.Where(x => x.UnitUpdateQuery == req.UnitUpdateQuery);
                    }

                    if (req.ChargeCode != null)
                    {
                        records = records.Where(x => x.ChargeCode == req.ChargeCode);
                    }
                }

                return new ErrorValidationResult<IEnumerable<UnitDto>> { Response = records.ToDtos() };
            }
        }

        /// <summary>
        /// Inserts a new unit into the data store using the specified request data.
        /// </summary>
        /// <remarks>If the request fails validation, the operation is not performed and the returned
        /// result contains the validation errors. Otherwise, the new unit is persisted and its data is returned in the
        /// response.</remarks>
        /// <param name="req">The request containing the details of the unit to insert. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an ErrorValidationResult with
        /// the inserted unit data if successful, or validation errors if the request is invalid.</returns>
        public async Task<ErrorValidationResult<UnitDto>> Insert(InsertUpdateUnitRequest req)
        {
            var errorValidationResult = await _validateUnitOnInsertUpdate(req);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = req.ToEntityOnInsert();

                await dbContext.Units.AddAsync(entity);
                await dbContext.SaveChangesAsync();

                return new ErrorValidationResult<UnitDto> { Response = entity.ToDto() };
            }
        }

        /// <summary>
        /// Updates the details of an existing unit with the specified values.
        /// </summary>
        /// <remarks>If validation fails, the unit is not updated and the returned result contains the
        /// validation errors. If the specified unit does not exist, the response will contain a null value for the
        /// updated unit.</remarks>
        /// <param name="UnitId">The unique identifier of the unit to update.</param>
        /// <param name="req">An object containing the new values for the unit's properties. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an ErrorValidationResult with
        /// the updated unit data if the update is successful, or validation errors if the input is invalid.</returns>
        public async Task<ErrorValidationResult<UnitDto>> Update(int unitId, InsertUpdateUnitRequest req)
        {
            //TODO: Not Found Error?
            var errorValidationResult = await _validateUnitOnInsertUpdate(req, unitId);
            if (errorValidationResult.Errors.Count > 0)
            {
                return errorValidationResult;
            }

            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.Units.FirstOrDefaultAsync(ent => ent.UnitId == unitId);

                if (entity != null)
                {
                    entity = entity.UpdateEntityFromRequest(req);
                    await dbContext.SaveChangesAsync();
                    return new ErrorValidationResult<UnitDto> { Response = entity.ToDto() };
                }
                else
                {
                    errorValidationResult.Errors = AddRecordNotFoundErrorToErrorValidationResult(errorValidationResult.Errors);
                    return errorValidationResult;
                }
            }
        }

        /// <summary>
        /// Deletes the unit with the specified identifier from the data store.
        /// </summary>
        /// <param name="UnitId">The unique identifier of the unit to delete.</param>
        /// <returns>An ErrorValidationResult that indicates whether the deletion was successful. If the unit does not exist, the
        /// result contains validation errors.</returns>
        public async Task<ErrorValidationResult> Delete(int unitId)
        {
            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.Units.FirstOrDefaultAsync(ent => ent.UnitId == unitId);
                var errorValidationResult = new ErrorValidationResult();

                if (entity != null)
                {
                    dbContext.Units.Remove(entity);

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

        /// <summary>
        /// Validates the specified unit filter request and returns the validation result.
        /// </summary>
        /// <param name="req">The filter request to validate. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an error validation result for
        /// the unit filter request, including any validation errors and the associated collection of unit data transfer
        /// objects.</returns>
        private async Task<ErrorValidationResult<IEnumerable<UnitDto>>> _validateUnitFilter(FilterUnitLogicRequest req)
        {
            ValidationResult result = await _filterUnitLogicRequestValidator.ValidateAsync(req);
            var errorValidationResult = _validatorUtilities.CreateDefaultValidationResponse<IEnumerable<UnitDto>>(result);
            return errorValidationResult;
        }

        /// <summary>
        /// Validates the specified unit insert or update request, ensuring that the request data is valid and that the
        /// unit name is unique.
        /// </summary>
        /// <remarks>This method performs both data validation and a uniqueness check for the unit name.
        /// It should be called before inserting or updating a unit to ensure data integrity.</remarks>
        /// <param name="req">The request containing the unit data to be inserted or updated. Cannot be null.</param>
        /// <param name="unitId">The identifier of the unit being updated, or null if inserting a new unit. Used to exclude the current unit
        /// from uniqueness checks.</param>
        /// <returns>A validation result containing any errors found during validation. If validation succeeds, the result
        /// contains no errors.</returns>
        private async Task<ErrorValidationResult<UnitDto>> _validateUnitOnInsertUpdate(InsertUpdateUnitRequest req, int? unitId = null)
        {
            ValidationResult result = await _insertUpdateUnitRequestValidator.ValidateAsync(req);
            var errorValidationResult = _validatorUtilities.CreateDefaultValidationResponse<UnitDto>(result);

            if (errorValidationResult.Errors.Count == 0)
            {
                //validate Unit insert / update req is unique
                var unitNameCheck = await this.Filter(new FilterUnitLogicRequest { UnitName = req.UnitName });

                if (unitNameCheck.Errors.Count == 0 & unitNameCheck.Response.Count() > 0)
                {
                    if ((unitId == null || unitId == 0) || (unitNameCheck.Response.FirstOrDefault().UnitId != unitId))
                    {
                        errorValidationResult.Errors.Add("UnitName", new List<string> { _validatorUtilities.CreateUniqueValidationErrorMessage("UnitName") });
                    }
                }
            }

            return errorValidationResult;
        }

        /// <summary>
        /// Adds a 'record not found' validation error for the 'Unit' entity to the specified error dictionary.
        /// </summary>
        /// <param name="errors">A dictionary containing validation errors, where the key is the field name and the value is a list of error
        /// messages. Must not be null.</param>
        /// <returns>The updated dictionary including the 'Unit' record not found error.</returns>
        private Dictionary<string, List<string>> AddRecordNotFoundErrorToErrorValidationResult(Dictionary<string, List<string>> errors)
        {
            errors.Add("Unit", new List<string> { _validatorUtilities.CreateRecordDoesNotExistValidationErrorMessage("UnitId") });
            return errors;
        }

        #endregion
    }
}
