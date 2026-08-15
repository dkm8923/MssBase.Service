namespace IntegrationTests.Shared.Utilities.Contracts.Service;

public interface IDefaultServiceTestsGetAllReadOnly
{
    public Task Default_GetAll_IncludeReadOnly_Should_Cache();
}
