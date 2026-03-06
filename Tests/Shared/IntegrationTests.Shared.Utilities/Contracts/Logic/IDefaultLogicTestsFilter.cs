namespace IntegrationTests.Shared.Utilities.Contracts.Logic;

public interface IDefaultLogicTestsFilter
{
    Task Default_Filter_Should_Return_Active_Data();
    Task Default_Filter_Should_Return_Inactive_Data();
    Task Filter_Should_Return_Zero_Records();
}
