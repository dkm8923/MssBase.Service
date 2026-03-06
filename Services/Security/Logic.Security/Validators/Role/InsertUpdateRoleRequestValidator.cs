using Dto.Security.Role;
using FluentValidation;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.Role;

public class InsertUpdateRoleRequestValidator : AbstractValidator<InsertUpdateRoleRequest>
{
    private static class EntityFieldNames
    {
        public const string Name = "Name";
        public const string Description = "Description";
        public const string CurrentUser = "CurrentUser";
        public const string ApplicationId = "ApplicationId";

    }

    public InsertUpdateRoleRequestValidator()
    {
        // Set cascade mode per rule (stops after first failure within each RuleFor)
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.Name))
            .Length(1, 64).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.Name, 64));

        RuleFor(v => v.Description)
            .Length(0, 256).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.Description, 256));

        RuleFor(v => v.ApplicationId)
            .GreaterThan(0).WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.ApplicationId));
            
        RuleFor(v => v.CurrentUser)
            .NotEmpty().WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.CurrentUser))
            .Length(1, 64).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.CurrentUser, 64));
    }
}
