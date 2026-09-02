using Dto.Security.User;
using FluentValidation;
using Shared.Logic.Common;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.User;

public class InsertUpdateUserRequestValidator : AbstractValidator<InsertUpdateUserRequest>
{
    public InsertUpdateUserRequestValidator()
    {
        // Set cascade mode per rule (stops after first failure within each RuleFor)
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(v => v.Email).ValidateEmail();

        RuleFor(v => v.Title)
            .Length(0, 8).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(Constants.EntityFieldNames.Title, 8));
        
        RuleFor(v => v.FirstName)
            .Length(0, 64).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(Constants.EntityFieldNames.FirstName, 64));

        RuleFor(v => v.MiddleName)
            .Length(0, 64).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(Constants.EntityFieldNames.MiddleName, 64));

        RuleFor(v => v.LastName)
            .Length(0, 64).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(Constants.EntityFieldNames.LastName, 64));

        RuleFor(v => v.PreferredName)
            .Length(0, 64).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(Constants.EntityFieldNames.PreferredName, 64));

        RuleFor(v => v.Suffix)
            .Length(0, 8).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(Constants.EntityFieldNames.Suffix, 8));

        RuleFor(v => v.TimeZone)
            .Length(0, 64).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(Constants.EntityFieldNames.TimeZone, 64));

        RuleFor(v => v.CurrentUser).ValidateCurrentUser();
    }
}
