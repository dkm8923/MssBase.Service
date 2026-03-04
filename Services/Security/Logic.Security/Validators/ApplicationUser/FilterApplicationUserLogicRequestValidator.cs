using Dto.Security.ApplicationUser.Logic;
using FluentValidation;

namespace Logic.Security.Validators.ApplicationUser;

public class FilterApplicationUserLogicRequestValidator : AbstractValidator<FilterApplicationUserLogicRequest>
{
    public FilterApplicationUserLogicRequestValidator()
    {
        RuleFor(v => v).Custom((v, context) =>
        {
            // Add custom validation rules as needed
            // Example: if no filter criteria provided, add warning
        });
    }
}
