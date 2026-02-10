using Shared.Models;

namespace Shared.Contracts;

public interface ICacheService
{
	#region Public

	public Task<ErrorValidationResult<TResponse>> GetByKeyAsync<TResponse>(bool deleteCache, string keyName, Func<Task<ErrorValidationResult<TResponse>>> asyncFunction, int expiresIn = 0);
	public Task RemoveKeysByPatternAsync(string pattern);

    //Common Service Names
    public const string CommonTypeService = "CommonTypeService";
    public const string UnitService = "UnitService";
    public const string UnitGroupColumnService = "UnitGroupColumnService";
    public const string UnitDefinitionService = "UnitDefinitionService";
    public const string RateService = "RateService";

    //Commission Servivce Names
    public const string CommissionService = "CommissionService";

    //Security Service Names
    public const string ApplicationService = "ApplicationService";

    #endregion
}