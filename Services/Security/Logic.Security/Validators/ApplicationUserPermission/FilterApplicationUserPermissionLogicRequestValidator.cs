using Dto.Security.ApplicationUserPermission.Logic;
using FluentValidation;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.ApplicationUserPermission;

public class FilterApplicationUserPermissionLogicRequestValidator : AbstractValidator<FilterApplicationUserPermissionLogicRequest>
{
    private static readonly List<string> FilterFields =
    [
        nameof(FilterApplicationUserPermissionLogicRequest.ApplicationUserPermissionIds),
        nameof(FilterApplicationUserPermissionLogicRequest.ApplicationId),
        nameof(FilterApplicationUserPermissionLogicRequest.ApplicationUserId),
        nameof(FilterApplicationUserPermissionLogicRequest.PermissionId),
        nameof(FilterApplicationUserPermissionLogicRequest.CreatedBy),
        nameof(FilterApplicationUserPermissionLogicRequest.CreatedOnDate),
        nameof(FilterApplicationUserPermissionLogicRequest.UpdatedBy),
        nameof(FilterApplicationUserPermissionLogicRequest.UpdatedOnDate)
    ];

    public FilterApplicationUserPermissionLogicRequestValidator()
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

    private static bool HasAtLeastOneFilter(FilterApplicationUserPermissionLogicRequest v)
    {
        return (v.ApplicationUserPermissionIds is { Count: > 0 })
            || v.ApplicationId.HasValue
            || v.ApplicationUserId.HasValue
            || v.PermissionId.HasValue
            || !string.IsNullOrWhiteSpace(v.CreatedBy)
            || v.CreatedOnDate.HasValue
            || !string.IsNullOrWhiteSpace(v.UpdatedBy)
            || v.UpdatedOnDate.HasValue;
    }
}

