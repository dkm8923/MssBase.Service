using Dto.Security.Permission.Logic;
using FluentValidation;
using Shared.Logic.Validators;

namespace Logic.Security.Validators.Permission;

public class FilterPermissionLogicRequestValidator : AbstractValidator<FilterPermissionLogicRequest>
{
    public FilterPermissionLogicRequestValidator()
    {
        RuleFor(v => v).Custom((v, context) =>
        {
            //if (v.OriginSystemId is null && v.CostTypeId is null && v.Name is null)
            //{
            //    var message = ValidatorUtilities.CreateFilterParmRequiredErrorMessage(new List<string> { "OriginSystemId", "CostTypeId", "Name" });

            //    //associate the error with a specific property
            //    context.AddFailure(ValidatorUtilities.SetPropertyNameOnFilterRequestValidation(), message);
            //}
        });

    }

}
