namespace IntegrationTests.Shared.Utilities.Contracts.Logic;

public interface IDefaultLogicTestsFilterIncludeRelated
{
    public Task Default_Filter_Should_Return_Related_Active_Data();
    public Task Default_Filter_Should_Return_Related_Inactive_Data();
    public Task Default_Filter_Should_Not_Return_Related_Data();
}
