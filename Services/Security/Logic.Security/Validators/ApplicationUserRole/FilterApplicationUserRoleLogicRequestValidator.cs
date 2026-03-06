using Dto.Security.ApplicationUserRole.Logic;
using FluentValidation;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.ApplicationUserRole;

public class FilterApplicationUserRoleLogicRequestValidator : AbstractValidator<FilterApplicationUserRoleLogicRequest>
{
    private static readonly List<string> FilterFields =
    [
        nameof(FilterApplicationUserRoleLogicRequest.ApplicationUserRoleIds),
        nameof(FilterApplicationUserRoleLogicRequest.ApplicationId),
        nameof(FilterApplicationUserRoleLogicRequest.ApplicationUserId),
        nameof(FilterApplicationUserRoleLogicRequest.RoleId),
        nameof(FilterApplicationUserRoleLogicRequest.CreatedBy),
        nameof(FilterApplicationUserRoleLogicRequest.CreatedOnDate),
        nameof(FilterApplicationUserRoleLogicRequest.UpdatedBy),
        nameof(FilterApplicationUserRoleLogicRequest.UpdatedOnDate)
    ];

    public FilterApplicationUserRoleLogicRequestValidator()
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

    private static bool HasAtLeastOneFilter(FilterApplicationUserRoleLogicRequest v)
    {
        return (v.ApplicationUserRoleIds is { Count: > 0 })
            || v.ApplicationId.HasValue
            || v.ApplicationUserId.HasValue
            || v.RoleId.HasValue
            || !string.IsNullOrWhiteSpace(v.CreatedBy)
            || v.CreatedOnDate.HasValue
            || !string.IsNullOrWhiteSpace(v.UpdatedBy)
            || v.UpdatedOnDate.HasValue;
    }
}

