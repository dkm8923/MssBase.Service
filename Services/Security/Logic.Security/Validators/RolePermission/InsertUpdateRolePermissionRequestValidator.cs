using Dto.Security.RolePermission;
using FluentValidation;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.RolePermission;

public class InsertUpdateRolePermissionRequestValidator : AbstractValidator<InsertUpdateRolePermissionRequest>
{
    public InsertUpdateRolePermissionRequestValidator()
    {
        // Set cascade mode per rule (stops after first failure within each RuleFor)
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(v => v.ApplicationId).ValidateApplicationIdIsRequired();
            
        RuleFor(v => v.RoleId).ValidateRoleIdIsRequired();

        RuleFor(v => v.PermissionId).ValidatePermissionIdIsRequired();

        RuleFor(v => v.CurrentUser).ValidateCurrentUser();
    }
}
