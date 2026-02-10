using Dto.Common.Unit;
using Dto.Common.UnitDefinition;
using FluentValidation;
using Shared.Contracts.Logic.Validators;


namespace Logic.Common.Validators.UnitDefinition
{
    public class InsertUpdateUnitDefinitionRequestValidator : AbstractValidator<InsertUpdateUnitDefinitionRequest>
    {
        private readonly IValidatorUtilities _validatorUtilities;

        private static class EntityFieldNames
        {
            public const string OriginSystem = "OriginSystem";
            public const string SourceColumn = "SourceColumn";
            public const string DestinationColumn = "DestinationColumn";
            public const string UserFriendlyDescription = "UserFriendlyDescription";
            public const string UnitValue = "UnitValue";
            public const string UnitValueColumn = "UnitValueColumn";
            public const string UnitQty = "UnitQty";
            public const string UnitQtyColumn = "UnitQtyColumn";
            public const string UnitQuery = "UnitQuery";
            public const string UnitQueryColumn = "UnitQueryColumn";
            public const string UnitQueryPosition = "UnitQueryPosition";
            public const string GroupBy = "GroupBy";
            public const string GroupByColumn = "GroupByColumn";
            public const string SupplementalGroupBy = "SupplementalGroupBy";
            public const string SupplementalGroupByColumn = "SupplementalGroupByColumn";
            public const string PkgQty = "PkgQty";
            public const string PkgQtyColumn = "PkgQtyColumn";
            public const string ConditionalAdjustment = "ConditionalAdjustment";
            public const string ConditionalAdjustmentColumn = "ConditionalAdjustmentColumn";
            public const string SqlDataType = "SqlDataType";
            public const string ListObjectName = "ListObjectName";
            public const string UseList = "UseList";
            public const string UsePrimaryKey = "UsePrimaryKey";
            public const string EvaluateAsString = "EvaluateAsString";
            public const string ExtraCriteria = "ExtraCriteria";
            public const string CurrentUser = "CurrentUser";
        }

        public InsertUpdateUnitDefinitionRequestValidator(IValidatorUtilities validatorUtilities)
        {
            _validatorUtilities = validatorUtilities;

            // Set cascade mode per rule (stops after first failure within each RuleFor)
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(v => v.OriginSystem)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.OriginSystem))
                .Length(1, 32).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.OriginSystem, 32));

            RuleFor(v => v.SourceColumn)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.SourceColumn))
                .Length(1, 64).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.SourceColumn, 64));

            RuleFor(v => v.DestinationColumn)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.DestinationColumn))
                .Length(1, 128).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.DestinationColumn, 128));

            RuleFor(v => v.UserFriendlyDescription)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.UserFriendlyDescription))
                .Length(1, 64).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.UserFriendlyDescription, 64));

            //RuleFor(v => v.UnitValue)
            //    .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.UnitValue));

            RuleFor(v => v.UnitValueColumn)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.UnitValueColumn))
                .Length(1, 128).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.UnitValueColumn, 128));

            //RuleFor(v => v.UnitQty)
            //    .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.UnitQty));

            RuleFor(v => v.UnitQtyColumn)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.UnitQtyColumn))
                .Length(1, 128).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.UnitQtyColumn, 128));

            //RuleFor(v => v.UnitQuery)
            //    .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.UnitQuery));

            RuleFor(v => v.UnitQueryColumn)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.UnitQueryColumn))
                .Length(1, 128).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.UnitQueryColumn, 128));

            RuleFor(v => v.UnitQueryPosition)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.UnitQueryPosition));

            //RuleFor(v => v.GroupBy)
            //    .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.GroupBy));

            RuleFor(v => v.GroupByColumn)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.GroupByColumn))
                .Length(1, 64).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.GroupByColumn, 64));

            //RuleFor(v => v.SupplementalGroupBy)
            //    .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.SupplementalGroupBy));

            RuleFor(v => v.SupplementalGroupByColumn)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.SupplementalGroupByColumn))
                .Length(1, 64).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.SupplementalGroupByColumn, 64));

            //RuleFor(v => v.PkgQty)
            //    .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.PkgQty));

            RuleFor(v => v.PkgQtyColumn)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.PkgQtyColumn))
                .Length(1, 64).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.PkgQtyColumn, 64));

            //RuleFor(v => v.ConditionalAdjustment)
            //    .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.ConditionalAdjustment));

            RuleFor(v => v.ConditionalAdjustmentColumn)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.ConditionalAdjustmentColumn))
                .Length(1, 64).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.ConditionalAdjustmentColumn, 64));

            RuleFor(v => v.SqlDataType)
                .Length(1, 32).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.SqlDataType, 32));

            RuleFor(v => v.ListObjectName)
                .Length(1, 64).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.ListObjectName, 64));

            //RuleFor(v => v.UseList)
            //    .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.UseList));

            //RuleFor(v => v.UsePrimaryKey)
            //    .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.UsePrimaryKey));

            //RuleFor(v => v.EvaluateAsString)
            //    .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.EvaluateAsString));

            //RuleFor(v => v.ExtraCriteria)
            //    .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.ExtraCriteria));

            RuleFor(v => v.CurrentUser)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.CurrentUser))
                .Length(1, 32).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.CurrentUser, 32));
        }
    }
}
