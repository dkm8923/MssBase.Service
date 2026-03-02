using Dto.Security.Permission.Logic;
using Shared.Contracts.Logic.Validators;
using FluentValidation;

namespace Logic.Security.Validators.Permission;

public class FilterPermissionLogicRequestValidator : AbstractValidator<FilterPermissionLogicRequest>
{
    private readonly IValidatorUtilities _validatorUtilities;

        public FilterPermissionLogicRequestValidator(IValidatorUtilities validatorUtilities)
        {
            _validatorUtilities = validatorUtilities;

            RuleFor(v => v).Custom((v, context) =>
            {
                //if (v.OriginSystemId is null && v.CostTypeId is null && v.Name is null)
                //{
                //    var message = _validatorUtilities.CreateFilterParmRequiredErrorMessage(new List<string> { "OriginSystemId", "CostTypeId", "Name" });

                //    //associate the error with a specific property
                //    context.AddFailure(_validatorUtilities.SetPropertyNameOnFilterRequestValidation(), message);
                //}
            });

        }

}
