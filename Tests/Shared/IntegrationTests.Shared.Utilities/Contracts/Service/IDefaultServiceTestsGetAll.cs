namespace IntegrationTests.Shared.Utilities.Contracts.Service;

public interface IDefaultServiceTestsGetAll
{
    Task Default_GetAll_Active_Should_Cache();
    Task Default_GetAll_IncludeInactive_Should_Cache();
    Task Default_GetAll_Should_Not_Cache_And_Return_Zero_Records();
}
