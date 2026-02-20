using Shared.Models;

namespace Shared.Contracts;

public interface ICacheService
{
	#region Public

	public Task<ErrorValidationResult<TResponse>> GetByKeyAsync<TResponse>(bool deleteCache, string keyName, Func<Task<ErrorValidationResult<TResponse>>> asyncFunction, int expiresIn = 0);
	public Task RemoveKeysByPatternAsync(string pattern);

    //Common Service Names
    

    //Security Service Names
    public const string ApplicationService = "ApplicationService";
    public const string ApplicationUserService = "ApplicationUserService";

    #endregion
}