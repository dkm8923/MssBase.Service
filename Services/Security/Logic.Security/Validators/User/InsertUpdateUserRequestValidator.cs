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

        RuleFor(v => v.FirstName)
            .Length(0, 64).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(Constants.EntityFieldNames.FirstName, 64));

        RuleFor(v => v.LastName)
            .Length(0, 64).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(Constants.EntityFieldNames.LastName, 64));

        RuleFor(v => v.CurrentUser).ValidateCurrentUser();
    }
}
