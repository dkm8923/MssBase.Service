using Dto.Security.Role.Logic;
using FluentValidation;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.Role;

public class FilterRoleLogicRequestValidator : AbstractValidator<FilterRoleLogicRequest>
{
    private static readonly List<string> FilterFields =
    [
        nameof(FilterRoleLogicRequest.RoleIds),
        nameof(FilterRoleLogicRequest.Name),
        nameof(FilterRoleLogicRequest.Description),
        nameof(FilterRoleLogicRequest.ApplicationId),
        nameof(FilterRoleLogicRequest.CreatedBy),
        nameof(FilterRoleLogicRequest.CreatedOnDate),
        nameof(FilterRoleLogicRequest.UpdatedBy),
        nameof(FilterRoleLogicRequest.UpdatedOnDate)
    ];

    public FilterRoleLogicRequestValidator()
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

    private static bool HasAtLeastOneFilter(FilterRoleLogicRequest v)
    {
        return (v.RoleIds is { Count: > 0 })
            || !string.IsNullOrWhiteSpace(v.Name)
            || !string.IsNullOrWhiteSpace(v.Description)
            || v.ApplicationId.HasValue
            || !string.IsNullOrWhiteSpace(v.CreatedBy)
            || v.CreatedOnDate.HasValue
            || !string.IsNullOrWhiteSpace(v.UpdatedBy)
            || v.UpdatedOnDate.HasValue;
    }
}
