using Dto.Security.ApplicationUserPermission;
using FluentValidation;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.ApplicationUserPermission;

public class InsertUpdateApplicationUserPermissionRequestValidator : AbstractValidator<InsertUpdateApplicationUserPermissionRequest>
{
    private static class EntityFieldNames
    {
        public const string ApplicationId = "ApplicationId";
        public const string ApplicationUserId = "ApplicationUserId";
        public const string PermissionId = "PermissionId";
        public const string CurrentUser = "CurrentUser";
    }

    public InsertUpdateApplicationUserPermissionRequestValidator()
    {
        // Set cascade mode per rule (stops after first failure within each RuleFor)
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(v => v.ApplicationId)
            .GreaterThan(0).WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.ApplicationId));

        RuleFor(v => v.ApplicationUserId)
            .GreaterThan(0).WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.ApplicationUserId));

        RuleFor(v => v.PermissionId)
            .GreaterThan(0).WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.PermissionId));

        RuleFor(v => v.CurrentUser)
            .NotEmpty().WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.CurrentUser))
            .Length(1, 64).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.CurrentUser, 64));
    }
}
