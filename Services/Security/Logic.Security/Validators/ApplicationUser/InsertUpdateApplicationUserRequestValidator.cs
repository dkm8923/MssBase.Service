using Dto.Security.ApplicationUser;
using FluentValidation;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.ApplicationUser;

public class InsertUpdateApplicationUserRequestValidator : AbstractValidator<InsertUpdateApplicationUserRequest>
{
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

    public InsertUpdateApplicationUserRequestValidator()
    {
        // Set cascade mode per rule (stops after first failure within each RuleFor)
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(v => v.Email)
            .NotEmpty().WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.Email))
            .EmailAddress().WithMessage("Email must be in a valid format!")
            .Length(1, 128).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.Email, 128));

        RuleFor(v => v.FirstName)
            .Length(0, 64).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.FirstName, 64));

        RuleFor(v => v.LastName)
            .Length(0, 64).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.LastName, 64));

        RuleFor(v => v.Password)
            .Length(0, 64).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.Password, 64));

        RuleFor(v => v.ApplicationId)
            .GreaterThan(0).WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.ApplicationId));

        RuleFor(v => v.CurrentUser)
            .NotEmpty().WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.CurrentUser))
            .Length(1, 64).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.CurrentUser, 64));
    }
}
