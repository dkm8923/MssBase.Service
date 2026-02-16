using Dto.Security.ApplicationUser.Logic;
using Shared.Contracts.Logic.Validators;
using FluentValidation;

namespace Logic.Security.Validators.ApplicationUser;

public class FilterApplicationUserLogicRequestValidator : AbstractValidator<FilterApplicationUserLogicRequest>
{
    private readonly IValidatorUtilities _validatorUtilities;

    public FilterApplicationUserLogicRequestValidator(IValidatorUtilities validatorUtilities)
    {
        _validatorUtilities = validatorUtilities;

        RuleFor(v => v).Custom((v, context) =>
        {
            // Add custom validation rules as needed
            // Example: if no filter criteria provided, add warning
        });
    }
}
