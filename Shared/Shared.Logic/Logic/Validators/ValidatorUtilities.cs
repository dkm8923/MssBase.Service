using Shared.Models;
using FluentValidation.Results;
using FluentValidation;
using Shared.Logic.Common;

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
            return $"{fieldName} must be between {minLength} and {maxLength} characters!";
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

        public static string CreateInvalidEmailErrorMessage()
        {
            return $"Invalid email address!";
        }

        /// <summary>
        /// Validate CurrentUser Max Length and NotEmpty. Default Values: FieldName = "CurrentUser", MaxLength = 64
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, string?> ValidateCurrentUser<T>(
            this IRuleBuilder<T, string?> ruleBuilder,
            int maxLength = 64
        )
        {
            return ruleBuilder
                .NotEmpty().WithMessage(CreateRequiredFieldErrorMessage(Constants.EntityFieldNames.CurrentUser))
                .Length(1, maxLength).WithMessage(CreateMaxLengthErrorMessage(Constants.EntityFieldNames.CurrentUser, maxLength));
        }

        /// <summary>
        /// Validate ApplicationId is required and greater than 0. Default Value: FieldName = "ApplicationId"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, int> ValidateApplicationIdIsRequired<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder.GreaterThan(0).WithMessage(CreateRequiredFieldErrorMessage(Constants.EntityFieldNames.ApplicationId));
        }

        /// <summary>
        /// Validate ApplicationUserId is required and greater than 0. Default Value: FieldName = "ApplicationUserId"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, int> ValidateApplicationUserIdIsRequired<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder.GreaterThan(0).WithMessage(CreateRequiredFieldErrorMessage(Constants.EntityFieldNames.ApplicationUserId));
        }

        /// <summary>
        /// Validate RoleId is required and greater than 0. Default Value: FieldName = "RoleId"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, int> ValidateRoleIdIsRequired<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder.GreaterThan(0).WithMessage(CreateRequiredFieldErrorMessage(Constants.EntityFieldNames.RoleId));
        }

        /// <summary>
        /// Validate PermissionId is required and greater than 0. Default Value: FieldName = "PermissionId"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, int> ValidatePermissionIdIsRequired<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder.GreaterThan(0).WithMessage(CreateRequiredFieldErrorMessage(Constants.EntityFieldNames.PermissionId));
        }

        /// <summary>
        /// Validate Description Max Length. Default Value: FieldName = "Description", MaxLength = 256
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, string?> ValidateDescriptionMaxLength<T>(this IRuleBuilder<T, string?> ruleBuilder, int maxLength = 256)
        {
            return ruleBuilder.Length(0, maxLength).WithMessage(CreateMaxLengthErrorMessage(Constants.EntityFieldNames.Description, maxLength));
        }

        /// <summary>
        /// Validate Name is populated
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, string?> ValidateNameIsRequired<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder.NotEmpty().WithMessage(CreateRequiredFieldErrorMessage(Constants.EntityFieldNames.Name));
        }

        /// <summary>
        /// Validate Name Max Length. Default Value: FieldName = "Name", MaxLength = 64
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, string?> ValidateNameMaxLength<T>(this IRuleBuilder<T, string?> ruleBuilder, int maxLength = 64)
        {
            return ruleBuilder.Length(0, maxLength).WithMessage(CreateMaxLengthErrorMessage(Constants.EntityFieldNames.Name, maxLength));
        }

        /// <summary>
        /// Validate Name Max Length. Default Value: FieldName = "Name", MaxLength = 64
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, string?> ValidateEmail<T>(this IRuleBuilder<T, string?> ruleBuilder, int maxLength = 128)
        {
            return ruleBuilder.NotEmpty().WithMessage(CreateRequiredFieldErrorMessage(Constants.EntityFieldNames.Email))
                .EmailAddress().WithMessage(CreateInvalidEmailErrorMessage())
                .Length(1, maxLength).WithMessage(CreateMaxLengthErrorMessage(Constants.EntityFieldNames.Email, maxLength));
        }
    }
}
