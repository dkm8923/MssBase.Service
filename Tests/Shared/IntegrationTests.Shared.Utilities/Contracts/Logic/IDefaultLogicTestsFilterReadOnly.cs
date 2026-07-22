namespace IntegrationTests.Shared.Utilities.Contracts.Logic;

public interface IDefaultLogicTestsFilterReadOnly
{
    Task Default_Filter_Should_Return_Active_ReadOnly_Data();
    Task Default_Filter_Should_Return_Inactive_ReadOnly_Data();
    Task Default_Filter_Should_Return_Zero_ReadOnly_Records();
}
