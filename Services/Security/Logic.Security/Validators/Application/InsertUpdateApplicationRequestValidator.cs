using Dto.Security.Application;
using FluentValidation;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.Application;

public class InsertUpdateApplicationRequestValidator : AbstractValidator<InsertUpdateApplicationRequest>
{
    private static class EntityFieldNames
    {
        public const string Name = "Name";
        public const string Description = "Description";
        public const string CurrentUser = "CurrentUser";
    }

    public InsertUpdateApplicationRequestValidator()
    {
        // Set cascade mode per rule (stops after first failure within each RuleFor)
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.Name))
            .Length(1, 64).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.Name, 64));

        RuleFor(v => v.Description)
            .Length(0, 256).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.Description, 256));

        RuleFor(v => v.CurrentUser)
            .NotEmpty().WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.CurrentUser))
            .Length(1, 64).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.CurrentUser, 64));
    }
}
