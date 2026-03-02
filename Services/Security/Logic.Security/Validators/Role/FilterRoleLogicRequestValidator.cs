using Dto.Security.Role.Logic;
using Shared.Contracts.Logic.Validators;
using FluentValidation;

namespace Logic.Security.Validators.Role;

public class FilterRoleLogicRequestValidator : AbstractValidator<FilterRoleLogicRequest>
{
    private readonly IValidatorUtilities _validatorUtilities;

        public FilterRoleLogicRequestValidator(IValidatorUtilities validatorUtilities)
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
