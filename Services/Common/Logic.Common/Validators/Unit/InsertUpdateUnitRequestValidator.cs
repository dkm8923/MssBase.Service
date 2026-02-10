using Dto.Common.Unit;
using Shared.Contracts.Logic.Validators;
using FluentValidation;

namespace Logic.Common.Validators.Unit
{
    public class InsertUpdateUnitRequestValidator : AbstractValidator<InsertUpdateUnitRequest>
    {
        private readonly IValidatorUtilities _validatorUtilities;

        private static class EntityFieldNames
        {
            public const string UnitCode = "UnitCode";
            public const string UnitName = "UnitName";
            public const string UnitDescription = "UnitDescription";
            public const string OriginSystem = "OriginSystem";
            public const string UnitDefinitionIdUnitQty = "UnitDefinitionIdUnitQty";
            public const string UnitDefinitionIdUnitValue = "UnitDefinitionIdUnitValue";
            public const string ValueTypeName = "ValueTypeName";
            public const string UnitPrepQuery = "UnitPrepQuery";
            public const string UnitHeaderQuery = "UnitHeaderQuery";
            public const string UnitUpdateQuery = "UnitUpdateQuery";
            public const string ChargeCode = "ChargeCode";
            public const string CurrentUser = "CurrentUser";

        }

        public InsertUpdateUnitRequestValidator(IValidatorUtilities validatorUtilities)
        {
            _validatorUtilities = validatorUtilities;

            // Set cascade mode per rule (stops after first failure within each RuleFor)
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(v => v.UnitCode).Length(1, 64).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.UnitCode, 64));

            RuleFor(v => v.UnitName)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.UnitName))
                .Length(1, 64).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.UnitName, 64));

            RuleFor(v => v.UnitDescription)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.UnitDescription))
                .Length(1, 256).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.UnitDescription, 256));

            RuleFor(v => v.OriginSystem)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.OriginSystem))
                .Length(1, 32).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.OriginSystem, 32));

            RuleFor(v => v.UnitDefinitionIdUnitQty)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.UnitDefinitionIdUnitQty));

            RuleFor(v => v.UnitDefinitionIdUnitValue)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.UnitDefinitionIdUnitValue));

            RuleFor(v => v.ValueTypeName).Length(1, 32).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.ValueTypeName, 32));
            RuleFor(v => v.UnitPrepQuery).Length(1, 4096).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.UnitPrepQuery, 4096));
            RuleFor(v => v.UnitHeaderQuery).Length(1, 4096).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.UnitHeaderQuery, 4096));
            RuleFor(v => v.UnitUpdateQuery).Length(1, 1024).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.UnitUpdateQuery, 1024));
            RuleFor(v => v.ChargeCode).Length(1, 8).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.ChargeCode, 8));

            RuleFor(v => v.CurrentUser)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.CurrentUser))
                .Length(1, 32).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.CurrentUser, 32));
        }
    }
}
