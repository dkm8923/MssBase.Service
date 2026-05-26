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
            .Length(1, 128).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.NewPassword, 128));

        RuleFor(v => v.CurrentUser).ValidateCurrentUser();
    }
}
            