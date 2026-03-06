using Dto.Security.RolePermission.Logic;
using FluentValidation;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.RolePermission;

public class FilterRolePermissionLogicRequestValidator : AbstractValidator<FilterRolePermissionLogicRequest>
{
    private static readonly List<string> FilterFields =
    [
        nameof(FilterRolePermissionLogicRequest.RolePermissionIds),
        nameof(FilterRolePermissionLogicRequest.ApplicationId),
        nameof(FilterRolePermissionLogicRequest.RoleId),
        nameof(FilterRolePermissionLogicRequest.PermissionId),
        nameof(FilterRolePermissionLogicRequest.CreatedBy),
        nameof(FilterRolePermissionLogicRequest.CreatedOnDate),
        nameof(FilterRolePermissionLogicRequest.UpdatedBy),
        nameof(FilterRolePermissionLogicRequest.UpdatedOnDate)
    ];

    public FilterRolePermissionLogicRequestValidator()
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

    private static bool HasAtLeastOneFilter(FilterRolePermissionLogicRequest v)
    {
        return (v.RolePermissionIds is { Count: > 0 })
            || v.ApplicationId.HasValue
            || v.RoleId.HasValue
            || v.PermissionId.HasValue
            || !string.IsNullOrWhiteSpace(v.CreatedBy)
            || v.CreatedOnDate.HasValue
            || !string.IsNullOrWhiteSpace(v.UpdatedBy)
            || v.UpdatedOnDate.HasValue;
    }
}
