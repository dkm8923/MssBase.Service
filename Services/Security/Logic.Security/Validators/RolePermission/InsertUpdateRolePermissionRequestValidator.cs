using Dto.Security.RolePermission;
using FluentValidation;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.RolePermission;

public class InsertUpdateRolePermissionRequestValidator : AbstractValidator<InsertUpdateRolePermissionRequest>
{
    private static class EntityFieldNames
    {
        
        public const string CurrentUser = "CurrentUser";
        public const string ApplicationId = "ApplicationId";
        public const string RoleId = "RoleId";
        public const string PermissionId = "PermissionId";

    }

    public InsertUpdateRolePermissionRequestValidator()
    {
        // Set cascade mode per rule (stops after first failure within each RuleFor)
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(v => v.ApplicationId)
            .GreaterThan(0).WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.ApplicationId));
            
        RuleFor(v => v.RoleId)
            .GreaterThan(0).WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.RoleId));

        RuleFor(v => v.PermissionId)
            .GreaterThan(0).WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.PermissionId));

        RuleFor(v => v.CurrentUser)
            .NotEmpty().WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.CurrentUser))
            .Length(1, 64).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.CurrentUser, 64));
    }
}
