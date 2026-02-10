using Dto.Security.Application.Logic;
using Shared.Contracts.Logic.Validators;
using FluentValidation;

namespace Logic.Security.Validators.Application;

public class FilterApplicationLogicRequestValidator : AbstractValidator<FilterApplicationLogicRequest>
{
    private readonly IValidatorUtilities _validatorUtilities;

        public FilterApplicationLogicRequestValidator(IValidatorUtilities validatorUtilities)
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
