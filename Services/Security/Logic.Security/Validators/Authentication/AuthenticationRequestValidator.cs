using Dto.Security.Authentication;
using FluentValidation;
using Shared.Logic.Common;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.Authentication;

public class AuthenticationRequestValidator : AbstractValidator<AuthenticationRequest>
{
    public AuthenticationRequestValidator()
    {
        // Set cascade mode per rule (stops after first failure within each RuleFor)
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.ApplicationName).NotEmpty().WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(Constants.EntityFieldNames.ApplicationName))
            .Length(1, 64).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(Constants.EntityFieldNames.ApplicationName, 64));
        
        RuleFor(x => x.Email).ValidateEmail();
        
        RuleFor(x => x.Password).NotEmpty().WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(Constants.EntityFieldNames.Password))
            .Length(0, 64).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(Constants.EntityFieldNames.Password, 64));
    }
}
