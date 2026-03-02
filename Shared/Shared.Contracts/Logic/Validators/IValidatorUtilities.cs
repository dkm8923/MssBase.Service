using Shared.Models;
using FluentValidation.Results;

namespace Shared.Contracts.Logic.Validators
{
    public interface IValidatorUtilities
    {
        public string CreateRequiredFieldErrorMessage(string fieldName);
        public string CreateMaxLengthErrorMessage(string fieldName, int maxLength);
        public string CreateMinMaxLengthErrorMessage(string fieldName, int minLength, int maxLength);
        public string CreateRequiredCharactersErrorMessage(string fieldName, int characterCount);
        public string CreateFilterParmRequiredErrorMessage(List<string> fieldNames);
        public string CreateUniqueValidationErrorMessage(string fieldName);
        public string CreateRecordDoesNotExistValidationErrorMessage(string idName);
        public string CreateDependencyExistsValidationErrorMessage(string dependencyName);
        public string SetPropertyNameOnFilterRequestValidation();
        public ErrorValidationResult<TResponse> CreateDefaultValidationResponse<TResponse>(ValidationResult result);
    }
}
