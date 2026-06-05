using Dto.Security.Authentication;
using FluentValidation;
using Shared.Logic.Common;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.Authentication;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        // Set cascade mode per rule (stops after first failure within each RuleFor)
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Token).NotEmpty().WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(Constants.EntityFieldNames.Token));
        
        RuleFor(x => x.RefreshToken).NotEmpty().WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(Constants.EntityFieldNames.RefreshToken))
            .Length(1, 2048).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(Constants.EntityFieldNames.RefreshToken, 2048));
    }
}
