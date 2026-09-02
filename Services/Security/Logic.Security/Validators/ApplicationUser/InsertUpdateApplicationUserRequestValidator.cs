using Dto.Security.ApplicationUser;
using FluentValidation;
using Shared.Logic.Common;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.ApplicationUser;

public class InsertUpdateApplicationUserRequestValidator : AbstractValidator<InsertUpdateApplicationUserRequest>
{
    public InsertUpdateApplicationUserRequestValidator()
    {
        // Set cascade mode per rule (stops after first failure within each RuleFor)
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(v => v.ApplicationId).ValidateApplicationIdIsRequired();

        RuleFor(v => v.UserId).ValidateUserIdIsRequired();

        RuleFor(v => v.CurrentUser).ValidateCurrentUser();
    }
}
