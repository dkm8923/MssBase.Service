using Dto.Common.Rate.Logic;
using FluentValidation;
using Shared.Contracts.Logic.Validators;

namespace Logic.Common.Validators.Rate
{
    public class FilterRateLogicRequestValidator : AbstractValidator<FilterRateLogicRequest>
    {
        private readonly IValidatorUtilities _validatorUtilities;

        public FilterRateLogicRequestValidator(IValidatorUtilities validatorUtilities)
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
}
