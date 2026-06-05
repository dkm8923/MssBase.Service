using FluentValidation;
using Shared.Logic.Common;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.ApplicationUser;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        // Set cascade mode per rule (stops after first failure within each RuleFor)
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(v => v.ApplicationUserId).ValidateApplicationUserIdIsRequired();

        RuleFor(v => v.NewPassword)
            .NotEmpty().WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(Constants.EntityFieldNames.NewPassword))
            .Length(12, 128).WithMessage(ValidatorUtilities.CreateMinMaxLengthErrorMessage(Constants.EntityFieldNames.NewPassword, 12, 128))
            .Matches("[a-z]").WithMessage($"{Constants.EntityFieldNames.NewPassword} must contain at least one lowercase letter!")
            .Matches("[A-Z]").WithMessage($"{Constants.EntityFieldNames.NewPassword} must contain at least one uppercase letter!")
            .Matches("[^a-zA-Z0-9]").WithMessage($"{Constants.EntityFieldNames.NewPassword} must contain at least one special character!")
            .Matches("[0-9]").WithMessage($"{Constants.EntityFieldNames.NewPassword} must contain at least one number!");

        RuleFor(v => v.CurrentUser).ValidateCurrentUser();
    }
}
            