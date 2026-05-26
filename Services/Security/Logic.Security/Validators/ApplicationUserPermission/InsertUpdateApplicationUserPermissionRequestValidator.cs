using Dto.Security.ApplicationUserPermission;
using FluentValidation;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.ApplicationUserPermission;

public class InsertUpdateApplicationUserPermissionRequestValidator : AbstractValidator<InsertUpdateApplicationUserPermissionRequest>
{
    // private static class EntityFieldNames
    // {
    //     //add any additional field names here as needed for error messages
    // }

    public InsertUpdateApplicationUserPermissionRequestValidator()
    {
        // Set cascade mode per rule (stops after first failure within each RuleFor)
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(v => v.ApplicationId).ValidateApplicationIdIsRequired();

        RuleFor(v => v.ApplicationUserId).ValidateApplicationUserIdIsRequired();

        RuleFor(v => v.PermissionId).ValidatePermissionIdIsRequired();

        RuleFor(v => v.CurrentUser).ValidateCurrentUser();
    }
}
