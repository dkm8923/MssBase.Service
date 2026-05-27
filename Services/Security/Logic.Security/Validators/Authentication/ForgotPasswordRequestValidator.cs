using Dto.Security.Authentication;
using FluentValidation;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.Authentication;

public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    private static class EntityFieldNames
    {
        public const string Email = "Email";
    }

    public ForgotPasswordRequestValidator()
    {
        // Set cascade mode per rule (stops after first failure within each RuleFor)
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Email).NotEmpty().WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.Email));
        
        RuleFor(x => x.CurrentUser).ValidateCurrentUser();
    }
}
