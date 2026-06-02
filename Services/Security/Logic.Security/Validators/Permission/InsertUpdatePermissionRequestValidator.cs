using Dto.Security.Permission;
using FluentValidation;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.Permission;

public class InsertUpdatePermissionRequestValidator : AbstractValidator<InsertUpdatePermissionRequest>
{
    // private static class EntityFieldNames
    // {
    //     //add any additional field names here as needed for error messages
    // }

    public InsertUpdatePermissionRequestValidator()
    {
        // Set cascade mode per rule (stops after first failure within each RuleFor)
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(v => v.Name).ValidateNameIsRequired().ValidateNameMaxLength();

        RuleFor(v => v.Description).ValidateDescriptionMaxLength();
            
        RuleFor(v => v.ApplicationId).ValidateApplicationIdIsRequired();
        
        RuleFor(v => v.CurrentUser).ValidateCurrentUser();
    }
}
