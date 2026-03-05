using Dto.Security.Application.Logic;
using FluentValidation;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.Application;

public class FilterApplicationLogicRequestValidator : AbstractValidator<FilterApplicationLogicRequest>
{
    private static readonly List<string> FilterFields =
    [
        nameof(FilterApplicationLogicRequest.ApplicationIds),
        nameof(FilterApplicationLogicRequest.Name),
        nameof(FilterApplicationLogicRequest.CreatedBy),
        nameof(FilterApplicationLogicRequest.CreatedOnDate),
        nameof(FilterApplicationLogicRequest.UpdatedBy),
        nameof(FilterApplicationLogicRequest.UpdatedOnDate)
    ];

    public FilterApplicationLogicRequestValidator()
    {
        // RuleFor(v => v).Custom((v, context) =>
        // {
        //     if (!HasAtLeastOneFilter(v))
        //     {
        //         var message = ValidatorUtilities.CreateFilterParmRequiredErrorMessage(FilterFields);
        //         context.AddFailure(ValidatorUtilities.SetPropertyNameOnFilterRequestValidation(), message);
        //     }
        // });
    }

    private static bool HasAtLeastOneFilter(FilterApplicationLogicRequest v)
    {
        return (v.ApplicationIds is { Count: > 0 })
            || !string.IsNullOrWhiteSpace(v.Name)
            || !string.IsNullOrWhiteSpace(v.CreatedBy)
            || v.CreatedOnDate.HasValue
            || !string.IsNullOrWhiteSpace(v.UpdatedBy)
            || v.UpdatedOnDate.HasValue;
    }
}
