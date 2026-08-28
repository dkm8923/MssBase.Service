using Dto.Security.Authentication;
using FluentValidation;
using Microsoft.Extensions.Options;
using Shared.Logic.Common;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.User;

// public class ChangePasswordRequestValidator : AbstractValidator<Dto.Security.User.ChangePasswordRequest>
// {
//     private readonly PasswordValidationConfig _passwordValidationConfig;

//     public ChangePasswordRequestValidator(IOptions<PasswordValidationConfig> passwordValidationConfig)
//     {
//         _passwordValidationConfig = passwordValidationConfig.Value;

//         // Set cascade mode per rule (stops after first failure within each RuleFor)
//         RuleLevelCascadeMode = CascadeMode.Stop;

//         RuleFor(v => v.UserId).ValidateUserIdIsRequired();

//         RuleFor(v => v.NewPassword)
//             .NotEmpty().WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(Constants.EntityFieldNames.NewPassword))
//             .Length(_passwordValidationConfig.RequiredLength, _passwordValidationConfig.MaxLength).WithMessage(ValidatorUtilities.CreateMinMaxLengthErrorMessage(Constants.EntityFieldNames.NewPassword, _passwordValidationConfig.RequiredLength, _passwordValidationConfig.MaxLength));
            
//         if (_passwordValidationConfig.RequireUppercase)
//         {
//             RuleFor(v => v.NewPassword).Matches("[A-Z]").WithMessage($"{Constants.EntityFieldNames.NewPassword} must contain at least one uppercase letter!");
//         }

//         if (_passwordValidationConfig.RequireLowercase)
//         {
//             RuleFor(v => v.NewPassword).Matches("[a-z]").WithMessage($"{Constants.EntityFieldNames.NewPassword} must contain at least one lowercase letter!");
//         }

//         if (_passwordValidationConfig.RequireDigit)
//         {
//             RuleFor(v => v.NewPassword).Matches("[0-9]").WithMessage($"{Constants.EntityFieldNames.NewPassword} must contain at least one number!");
//         }   
        
//         if (_passwordValidationConfig.RequireNonAlphanumeric)
//         {
//             RuleFor(v => v.NewPassword).Matches("[^a-zA-Z0-9]").WithMessage($"{Constants.EntityFieldNames.NewPassword} must contain at least one special character!");
//         }

//         RuleFor(v => v.CurrentUser).ValidateCurrentUser();
//     }
// }
            