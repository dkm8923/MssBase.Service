namespace IntegrationTests.Shared.Utilities.Contracts.Logic;

public interface IDefaultLogicTestsGetAll
{
    Task Default_GetAll_Should_Return_Active_Data();
    Task Default_GetAll_Should_Return_Inactive_Data();
    Task Default_GetAll_Should_Return_Zero_Records();
}
