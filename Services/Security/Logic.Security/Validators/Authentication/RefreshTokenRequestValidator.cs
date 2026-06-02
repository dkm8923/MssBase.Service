using Dto.Security.Authentication;
using FluentValidation;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.Authentication;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    private static class EntityFieldNames
    {
        public const string Token = "Token";
        public const string RefreshToken = "RefreshToken";
    }

    public RefreshTokenRequestValidator()
    {
        // Set cascade mode per rule (stops after first failure within each RuleFor)
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Token).NotEmpty().WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.Token));
        
        RuleFor(x => x.RefreshToken).NotEmpty().WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.RefreshToken))
            .Length(1, 2048).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.RefreshToken, 2048));
    }
}
