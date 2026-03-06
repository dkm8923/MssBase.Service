namespace IntegrationTests.Shared.Utilities.Contracts.Logic;

public interface IDefaultLogicTestsGetById
{
    Task Default_GetById_Should_Return_Active_Record();
    Task Default_GetById_Should_Not_Return_Inactive_Record();
    Task Default_GetById_Should_Return_Inactive_Record();
}
