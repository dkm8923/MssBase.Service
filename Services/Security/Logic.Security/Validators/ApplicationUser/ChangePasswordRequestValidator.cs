using FluentValidation;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.ApplicationUser;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    private static class EntityFieldNames
    {
        public const string ApplicationUserId = "ApplicationUserId";
        public const string NewPassword = "NewPassword";
        public const string CurrentUser = "CurrentUser";
    }

    public ChangePasswordRequestValidator()
    {
        // Set cascade mode per rule (stops after first failure within each RuleFor)
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(v => v.ApplicationUserId)
            .GreaterThan(0).WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.ApplicationUserId));

        RuleFor(v => v.NewPassword)
            .NotEmpty().WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.NewPassword))
            .Length(1, 128).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.NewPassword, 128));

        RuleFor(v => v.CurrentUser)
            .NotEmpty().WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.CurrentUser))
            .Length(1, 64).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.CurrentUser, 64));
    }
}
            