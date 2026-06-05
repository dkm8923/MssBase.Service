using Dto.Security.Role;
using FluentValidation;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.Role;

public class InsertUpdateRoleRequestValidator : AbstractValidator<InsertUpdateRoleRequest>
{
    public InsertUpdateRoleRequestValidator()
    {
        // Set cascade mode per rule (stops after first failure within each RuleFor)
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(v => v.Name).ValidateNameIsRequired().ValidateNameMaxLength();

        RuleFor(v => v.Description).ValidateDescriptionMaxLength();

        RuleFor(v => v.ApplicationId).ValidateApplicationIdIsRequired();
            
        RuleFor(v => v.CurrentUser).ValidateCurrentUser();
    }
}
