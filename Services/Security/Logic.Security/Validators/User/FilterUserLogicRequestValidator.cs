using Dto.Security.User.Logic;
using FluentValidation;

namespace Logic.Security.Validators.User;

public class FilterUserLogicRequestValidator : AbstractValidator<FilterUserLogicRequest>
{
    public FilterUserLogicRequestValidator()
    {
        RuleFor(v => v).Custom((v, context) =>
        {
            // Add custom validation rules as needed
            // Example: if no filter criteria provided, add warning
        });
    }
}
