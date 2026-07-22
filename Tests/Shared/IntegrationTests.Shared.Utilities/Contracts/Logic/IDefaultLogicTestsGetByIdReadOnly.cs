namespace IntegrationTests.Shared.Utilities.Contracts.Logic;

public interface IDefaultLogicTestsGetByIdReadOnly
{
    public Task Default_GetById_Should_Return_Active_ReadOnly_Record();
    public Task Default_GetById_Should_Return_Inactive_ReadOnly_Record();
    public Task Default_GetById_Should_Not_Return_ReadOnly_Record();
}
