using Dto.Security.Authentication;
using FluentValidation;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.Authentication;

public class AuthenticationRequestValidator : AbstractValidator<AuthenticationRequest>
{
    private static class EntityFieldNames
    {
        public const string ApplicationName = "ApplicationName";
        public const string Email = "Email";
        public const string Password = "Password";
    }

    public AuthenticationRequestValidator()
    {
        // Set cascade mode per rule (stops after first failure within each RuleFor)
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.ApplicationName).NotEmpty().WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.ApplicationName));
        
        RuleFor(x => x.Email).NotEmpty().WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.Email))
            .EmailAddress().WithMessage(ValidatorUtilities.CreateInvalidEmailErrorMessage())
            .Length(1, 128).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.Email, 128));
        
        RuleFor(x => x.Password).NotEmpty().WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.Password))
            .Length(0, 64).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.Password, 64));
    }
}
