using Dto.Common.Rate;
using FluentValidation;
using Shared.Contracts.Logic.Validators;

namespace Logic.Common.Validators.Rate
{
    public class InsertUpdateRateRequestValidator : AbstractValidator<InsertUpdateRateRequest>
    {
        private readonly IValidatorUtilities _validatorUtilities;

        private static class EntityFieldNames
        {
            public const string UnitId = "UnitId";
            public const string RateAmt = "RateAmt";
            public const string MinLimit = "MinLimit";
            public const string MaxLimit = "MaxLimit";
            public const string StartDate = "StartDate";
            public const string EndDate = "EndDate";
            public const string UnitValue = "UnitValue";
            public const string Facility = "Facility";
            public const string CurrentUser = "CurrentUser";
        }

        //TODO: Validation on overflowing ints, decimals, etc
        //TODO: Validation on StartDate < EndDate

        public InsertUpdateRateRequestValidator(IValidatorUtilities validatorUtilities)
        {
            _validatorUtilities = validatorUtilities;

            // Set cascade mode per rule (stops after first failure within each RuleFor)
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(v => v.UnitId)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.UnitId));

            RuleFor(v => v.RateAmt)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.RateAmt));

            RuleFor(v => v.MinLimit)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.MinLimit));

            RuleFor(v => v.MaxLimit)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.MaxLimit));

            RuleFor(v => v.StartDate)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.StartDate));

            RuleFor(v => v.EndDate)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.EndDate));

            RuleFor(v => v.UnitValue)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.UnitValue))
                .Length(1, 64).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.UnitValue, 64));

            RuleFor(v => v.Facility)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.Facility))
                .Length(1, 32).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.Facility, 32));

            RuleFor(v => v.CurrentUser)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.CurrentUser))
                .Length(1, 32).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.CurrentUser, 32));
        }
    }
}
