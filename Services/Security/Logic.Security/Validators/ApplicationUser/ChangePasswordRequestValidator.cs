using FluentValidation;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.ApplicationUser;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    private static class EntityFieldNames
    {
        public const string NewPassword = "NewPassword";
    }

    public ChangePasswordRequestValidator()
    {
        // Set cascade mode per rule (stops after first failure within each RuleFor)
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(v => v.ApplicationUserId).ValidateApplicationUserIdIsRequired();

        RuleFor(v => v.NewPassword)
            .NotEmpty().WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.NewPassword))
            .Length(12, 128).WithMessage(ValidatorUtilities.CreateMinMaxLengthErrorMessage(EntityFieldNames.NewPassword, 12, 128))
            .Matches("[a-z]").WithMessage($"{EntityFieldNames.NewPassword} must contain at least one lowercase letter!")
            .Matches("[A-Z]").WithMessage($"{EntityFieldNames.NewPassword} must contain at least one uppercase letter!")
            .Matches("[^a-zA-Z0-9]").WithMessage($"{EntityFieldNames.NewPassword} must contain at least one special character!")
            .Matches("[0-9]").WithMessage($"{EntityFieldNames.NewPassword} must contain at least one number!");

        RuleFor(v => v.CurrentUser).ValidateCurrentUser();
    }
}
            