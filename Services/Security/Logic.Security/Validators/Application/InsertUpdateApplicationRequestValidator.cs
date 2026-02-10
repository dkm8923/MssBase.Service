using Dto.Security.Application;
using Shared.Contracts.Logic.Validators;
using FluentValidation;

namespace Logic.Security.Validators.Application;

public class InsertUpdateApplicationRequestValidator : AbstractValidator<InsertUpdateApplicationRequest>
{
    private readonly IValidatorUtilities _validatorUtilities;

        private static class EntityFieldNames
        {
            public const string Name = "Name";
            public const string Description = "Description";
            public const string CurrentUser = "CurrentUser";

        }

        public InsertUpdateApplicationRequestValidator(IValidatorUtilities validatorUtilities)
        {
            _validatorUtilities = validatorUtilities;

            // Set cascade mode per rule (stops after first failure within each RuleFor)
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(v => v.Name)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.Name))
                .Length(1, 64).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.Name, 64));

            RuleFor(v => v.Description)
                .Length(0, 256).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.Description, 256));

            RuleFor(v => v.CurrentUser)
                .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.CurrentUser))
                .Length(1, 64).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.CurrentUser, 64));
        }
}
