using Dto.Security.Authentication;
using FluentValidation;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.Authentication;

public class AuthenticationRequestValidator : AbstractValidator<AuthenticationRequest>
{
    private static class EntityFieldNames
    {
        public const string ApplicationId = "ApplicationId";
        public const string EmailAddress = "EmailAddress";
        public const string Password = "Password";
    }

    public AuthenticationRequestValidator()
    {
        // Set cascade mode per rule (stops after first failure within each RuleFor)
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.ApplicationId).GreaterThan(0).WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.ApplicationId));
        
        RuleFor(x => x.EmailAddress).NotEmpty().WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.EmailAddress))
            .EmailAddress().WithMessage(ValidatorUtilities.CreateInvalidEmailErrorMessage());
        
        RuleFor(x => x.Password).NotEmpty().WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.Password));
    }
}
