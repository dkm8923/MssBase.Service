namespace IntegrationTests.Shared.Utilities.Contracts.Service;

public interface IDefaultServiceTestsGetByIdReadOnly
{
    public Task Default_GetById_IncludeReadOnly_Should_Cache();
}
