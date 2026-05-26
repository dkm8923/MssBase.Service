using Dto.Security.ApplicationUserRole;
using FluentValidation;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.ApplicationUserRole;

public class InsertUpdateApplicationUserRoleRequestValidator : AbstractValidator<InsertUpdateApplicationUserRoleRequest>
{
    // private static class EntityFieldNames
    // {
    //     //add any additional field names here as needed for error messages
    // }

    public InsertUpdateApplicationUserRoleRequestValidator()
    {
        // Set cascade mode per rule (stops after first failure within each RuleFor)
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(v => v.ApplicationId).ValidateApplicationIdIsRequired();

        RuleFor(v => v.ApplicationUserId).ValidateApplicationUserIdIsRequired();

        RuleFor(v => v.RoleId).ValidateRoleIdIsRequired();

        RuleFor(v => v.CurrentUser).ValidateCurrentUser();
    }
}
