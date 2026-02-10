using Dto.Common.UnitGroupColumn;
using FluentValidation;
using Shared.Contracts.Logic.Validators;

namespace Logic.Common.Validators.UnitGroupColumn
{
    public class InsertUpdateUnitGroupColumnRequestValidator : AbstractValidator<InsertUpdateUnitGroupColumnRequest>
    {
        private readonly IValidatorUtilities _validatorUtilities;

        private static class EntityFieldNames
        {
            public const string UnitId = "UnitId";
            public const string UnitDefinitionId = "UnitDefinitionId";
            public const string SortOrder = "SortOrder";
            public const string CurrentUser = "CurrentUser";
        }

        public InsertUpdateUnitGroupColumnRequestValidator(IValidatorUtilities validatorUtilities)
        {
            _validatorUtilities = validatorUtilities;

            // Set cascade mode per rule (stops after first failure within each RuleFor)
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(v => v.UnitId)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.UnitId));

            RuleFor(v => v.UnitDefinitionId)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.UnitDefinitionId));

            RuleFor(v => v.SortOrder)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.SortOrder));

            RuleFor(v => v.CurrentUser)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.CurrentUser))
                .Length(1, 32).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.CurrentUser, 32));
        }
    }
}
