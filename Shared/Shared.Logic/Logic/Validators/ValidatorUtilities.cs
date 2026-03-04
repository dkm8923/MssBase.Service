using Shared.Models;
using FluentValidation.Results;

namespace Shared.Logic.Validators
{
    public static class ValidatorUtilities
    {
        public static string CreateRequiredFieldErrorMessage(string fieldName)
        {
            return $"{fieldName} is a required field!";
        }

        public static string CreateMaxLengthErrorMessage(string fieldName, int maxLength)
        {
            return $"{fieldName} cannot exceed {maxLength} characters!";
        }

        public static string CreateMinMaxLengthErrorMessage(string fieldName, int minLength, int maxLength)
        {
            return $"{fieldName} cannot exceed {maxLength} characters!";
        }

        public static string CreateRequiredCharactersErrorMessage(string fieldName, int characterCount)
        {
            return $"{fieldName} requires {characterCount} characters!";
        }

        public static string CreateFilterParmRequiredErrorMessage(List<string> fieldNames)
        {
            return $"At least one filter parameter must be populated! (IE: {string.Join(" / ", fieldNames)}";
        }

        public static string CreateUniqueValidationErrorMessage(string fieldName)
        {
            return $"{fieldName} must be unique!";
        }

        public static string CreateRecordDoesNotExistValidationErrorMessage(string idName)
        {
            return $"Record does not exist for specified {idName}!";
        }

        public static string SetPropertyNameOnFilterRequestValidation() 
        {
            return "FilterRequest";
        }

        public static ErrorValidationResult<TResponse> CreateDefaultValidationResponse<TResponse>(ValidationResult result)
        {
            var validationResult = new ErrorValidationResult<TResponse>();

            foreach (var error in result.Errors)
            {
                if (validationResult.Errors.ContainsKey(error.PropertyName))
                {
                    validationResult.Errors[error.PropertyName].Add(error.ErrorMessage);
                }
                else
                {
                    validationResult.Errors.Add(error.PropertyName, new List<string> { error.ErrorMessage });
                }
            }

            return validationResult;
        }

        public static string CreateDependencyExistsValidationErrorMessage(string dependencyName)
        {
            return $"Record still contains child dependencies! IE: {dependencyName}";
        }
    }
}
