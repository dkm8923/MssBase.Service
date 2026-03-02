using Shared.Contracts.Logic.Validators;
using Shared.Models;
using FluentValidation.Results;

namespace Shared.Logic.Validators
{
    public class ValidatorUtilities : IValidatorUtilities
    {
        public string CreateRequiredFieldErrorMessage(string fieldName)
        {
            return $"{fieldName} is a required field!";
        }

        public string CreateMaxLengthErrorMessage(string fieldName, int maxLength)
        {
            return $"{fieldName} cannot exceed {maxLength} characters!";
        }

        public string CreateMinMaxLengthErrorMessage(string fieldName, int minLength, int maxLength)
        {
            return $"{fieldName} cannot exceed {maxLength} characters!";
        }

        public string CreateRequiredCharactersErrorMessage(string fieldName, int characterCount)
        {
            return $"{fieldName} requires {characterCount} characters!";
        }

        public string CreateFilterParmRequiredErrorMessage(List<string> fieldNames)
        {
            return $"At least one filter parameter must be populated! (IE: {string.Join(" / ", fieldNames)}";
        }

        public string CreateUniqueValidationErrorMessage(string fieldName)
        {
            return $"{fieldName} must be unique!";
        }

        public string CreateRecordDoesNotExistValidationErrorMessage(string idName)
        {
            return $"Record does not exist for specified {idName}!";
        }

        public string SetPropertyNameOnFilterRequestValidation() 
        {
            return "FilterRequest";
        }

        public ErrorValidationResult<TResponse> CreateDefaultValidationResponse<TResponse>(ValidationResult result)
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

        public string CreateDependencyExistsValidationErrorMessage(string dependencyName)
        {
            return $"Record still contains child dependencies! IE: : {dependencyName}";
        }
    }
}
