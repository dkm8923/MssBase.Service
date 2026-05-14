namespace IntegrationTests.Shared.Utilities.Contracts.Logic;

public interface IDefaultLogicTestsGetByIdIncludeRelated
{
    public Task Default_GetById_Should_Return_Related_Active_Data();
    public Task Default_GetById_Should_Return_Related_Inactive_Data();
    public Task Default_GetById_Should_Not_Return_Related_Data();
}
