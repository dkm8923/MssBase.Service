namespace IntegrationTests.Shared.Utilities.Contracts.Logic;

public interface IDefaultLogicTestsGetAllReadOnly
{
    Task Default_GetAll_Should_Return_Active_ReadOnly_Data();
    Task Default_GetAll_Should_Return_Inactive_ReadOnly_Data();
    Task Default_GetAll_Should_Return_Zero_ReadOnly_Records();
}
