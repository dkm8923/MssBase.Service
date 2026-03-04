using Dto.Security.ApplicationUserPermission.Logic;
using FluentValidation;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.ApplicationUserPermission;

public class FilterApplicationUserPermissionLogicRequestValidator : AbstractValidator<FilterApplicationUserPermissionLogicRequest>
{
    public FilterApplicationUserPermissionLogicRequestValidator()
    {
        RuleFor(v => v).Custom((v, context) =>
        {
            // Add custom validation rules as needed
            // Example: if no filter criteria provided, add warning
        });
    }
}
