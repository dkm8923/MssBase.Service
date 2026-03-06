namespace IntegrationTests.Shared.Utilities.Contracts.Service;

public interface IDefaultServiceTestsGetById
{
    Task Default_GetById_Should_Cache();
    Task Default_GetById_IncludeInactive_Should_Cache();
    Task Default_GetById_Unused_Id_Should_Not_Cache();
}
    