using Dto.Security.ApplicationUserPermission;
using Shared.Contracts.Logic.Validators;
using FluentValidation;

namespace Logic.Security.Validators.ApplicationUserPermission;

public class InsertUpdateApplicationUserPermissionRequestValidator : AbstractValidator<InsertUpdateApplicationUserPermissionRequest>
{
    private readonly IValidatorUtilities _validatorUtilities;

    private static class EntityFieldNames
    {
        public const string ApplicationId = "ApplicationId";
        public const string ApplicationUserId = "ApplicationUserId";
        public const string PermissionId = "PermissionId";
        public const string CurrentUser = "CurrentUser";
    }

    public InsertUpdateApplicationUserPermissionRequestValidator(IValidatorUtilities validatorUtilities)
    {
        _validatorUtilities = validatorUtilities;

        // Set cascade mode per rule (stops after first failure within each RuleFor)
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(v => v.ApplicationId)
            .GreaterThan(0).WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.ApplicationId));

        RuleFor(v => v.ApplicationUserId)
            .GreaterThan(0).WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.ApplicationUserId));

        RuleFor(v => v.PermissionId)
            .GreaterThan(0).WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.PermissionId));

        RuleFor(v => v.CurrentUser)
            .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.CurrentUser))
            .Length(1, 64).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.CurrentUser, 64));
    }
}
