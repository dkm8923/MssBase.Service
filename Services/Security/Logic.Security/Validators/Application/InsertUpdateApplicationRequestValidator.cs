using Dto.Security.Application;
using FluentValidation;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.Application;

public class InsertUpdateApplicationRequestValidator : AbstractValidator<InsertUpdateApplicationRequest>
{
    public InsertUpdateApplicationRequestValidator()
    {
        // Set cascade mode per rule (stops after first failure within each RuleFor)
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(v => v.Name).ValidateNameIsRequired().ValidateNameMaxLength();

        RuleFor(v => v.Description).ValidateDescriptionMaxLength();

        RuleFor(v => v.CurrentUser).ValidateCurrentUser();
    }
}
