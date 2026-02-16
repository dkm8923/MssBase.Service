using Dto.Security.ApplicationUser;
using Shared.Contracts.Logic.Validators;
using FluentValidation;

namespace Logic.Security.Validators.ApplicationUser;

public class InsertUpdateApplicationUserRequestValidator : AbstractValidator<InsertUpdateApplicationUserRequest>
{
    private readonly IValidatorUtilities _validatorUtilities;

    private static class EntityFieldNames
    {
        public const string Email = "Email";
        public const string FirstName = "FirstName";
        public const string LastName = "LastName";
        public const string DateOfBirth = "DateOfBirth";
        public const string Password = "Password";
        public const string ApplicationId = "ApplicationId";
        public const string CurrentUser = "CurrentUser";
    }

    public InsertUpdateApplicationUserRequestValidator(IValidatorUtilities validatorUtilities)
    {
        _validatorUtilities = validatorUtilities;

        // Set cascade mode per rule (stops after first failure within each RuleFor)
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(v => v.Email)
            .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.Email))
            .EmailAddress().WithMessage("Email must be in a valid format!")
            .Length(1, 256).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.Email, 256));

        RuleFor(v => v.FirstName)
            .Length(0, 64).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.FirstName, 64));

        RuleFor(v => v.LastName)
            .Length(0, 64).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.LastName, 64));

        RuleFor(v => v.Password)
            .Length(0, 256).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.Password, 256));

        RuleFor(v => v.ApplicationId)
            .GreaterThan(0).WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.ApplicationId));

        RuleFor(v => v.CurrentUser)
            .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.CurrentUser))
            .Length(1, 64).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.CurrentUser, 64));
    }
}
