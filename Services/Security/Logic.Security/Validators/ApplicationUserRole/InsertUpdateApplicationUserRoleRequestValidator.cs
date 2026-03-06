using Dto.Security.ApplicationUserRole;
using FluentValidation;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.ApplicationUserRole;

public class InsertUpdateApplicationUserRoleRequestValidator : AbstractValidator<InsertUpdateApplicationUserRoleRequest>
{
    private static class EntityFieldNames
    {
        public const string ApplicationId = "ApplicationId";
        public const string ApplicationUserId = "ApplicationUserId";
        public const string RoleId = "RoleId";
        public const string CurrentUser = "CurrentUser";
    }

    public InsertUpdateApplicationUserRoleRequestValidator()
    {
        // Set cascade mode per rule (stops after first failure within each RuleFor)
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(v => v.ApplicationId)
            .GreaterThan(0).WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.ApplicationId));

        RuleFor(v => v.ApplicationUserId)
            .GreaterThan(0).WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.ApplicationUserId));

        RuleFor(v => v.RoleId)
            .GreaterThan(0).WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.RoleId));

        RuleFor(v => v.CurrentUser)
            .NotEmpty().WithMessage(ValidatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.CurrentUser))
            .Length(1, 64).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.CurrentUser, 64));
    }
}
