using Dto.Security.Permission.Logic;
using FluentValidation;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.Permission;

public class FilterPermissionLogicRequestValidator : AbstractValidator<FilterPermissionLogicRequest>
{
    private static readonly List<string> FilterFields =
    [
        nameof(FilterPermissionLogicRequest.PermissionIds),
        nameof(FilterPermissionLogicRequest.Name),
        nameof(FilterPermissionLogicRequest.Description),
        nameof(FilterPermissionLogicRequest.ApplicationId),
        nameof(FilterPermissionLogicRequest.CreatedBy),
        nameof(FilterPermissionLogicRequest.CreatedOnDate),
        nameof(FilterPermissionLogicRequest.UpdatedBy),
        nameof(FilterPermissionLogicRequest.UpdatedOnDate)
    ];

    public FilterPermissionLogicRequestValidator()
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

    private static bool HasAtLeastOneFilter(FilterPermissionLogicRequest v)
    {
        return (v.PermissionIds is { Count: > 0 })
            ||!string.IsNullOrWhiteSpace(v.Name)
            || !string.IsNullOrWhiteSpace(v.Description)
            || v.ApplicationId.HasValue
            || !string.IsNullOrWhiteSpace(v.CreatedBy)
            || v.CreatedOnDate.HasValue
            || !string.IsNullOrWhiteSpace(v.UpdatedBy)
            || v.UpdatedOnDate.HasValue;
    }
}

