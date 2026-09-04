using Dto.Common.CommonRelationalData;
using Shared.Logic.Validators;
using Shared.Models;

public static class CommonLogicUtilities
{
    public static ErrorValidationResult<T> ValidateCommonRelationalDataNameIsValid<T>(List<CommonRelationalDataDto>? commonRelationalData, string fieldValue, string fieldName, string referenceType, ErrorValidationResult<T> errorValidationResult)
    {
        if (!string.IsNullOrWhiteSpace(fieldValue) && (commonRelationalData == null || !commonRelationalData.Any(record => record.Name == fieldValue)))
        {
            errorValidationResult.Errors.Add(fieldName, new List<string> { ValidatorUtilities.CreateInvalidCommonRelationalDataValueValidationErrorMessage(fieldName, referenceType) });
        }

        return errorValidationResult;
    }

    public static ErrorValidationResult<T> ValidateCommonRelationalDataValueIsValid<T>(List<CommonRelationalDataDto>? commonRelationalData, string fieldValue, string fieldName, string referenceType, ErrorValidationResult<T> errorValidationResult)
    {
        if (!string.IsNullOrWhiteSpace(fieldValue) && (commonRelationalData == null || !commonRelationalData.Any(record => record.Value == fieldValue)))
        {
            errorValidationResult.Errors.Add(fieldName, new List<string> { ValidatorUtilities.CreateInvalidCommonRelationalDataValueValidationErrorMessage(fieldName, referenceType) });
        }

        return errorValidationResult;
    }   
}