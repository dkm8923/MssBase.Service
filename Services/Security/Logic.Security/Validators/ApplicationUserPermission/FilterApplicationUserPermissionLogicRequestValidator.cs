using Dto.Security.ApplicationUserPermission.Logic;
using Shared.Contracts.Logic.Validators;
using FluentValidation;

namespace Logic.Security.Validators.ApplicationUserPermission;

public class FilterApplicationUserPermissionLogicRequestValidator : AbstractValidator<FilterApplicationUserPermissionLogicRequest>
{
    private readonly IValidatorUtilities _validatorUtilities;

    public FilterApplicationUserPermissionLogicRequestValidator(IValidatorUtilities validatorUtilities)
    {
        _validatorUtilities = validatorUtilities;

        RuleFor(v => v).Custom((v, context) =>
        {
            // Add custom validation rules as needed
            // Example: if no filter criteria provided, add warning
        });
    }
}
