namespace IntegrationTests.Shared.Utilities.Contracts.Logic;

public interface IDefaultLogicTestsGetAuditLogsById
{
    Task Default_GetAuditLogsById_Should_Return_Update_Data();
    Task Default_GetAuditLogsById_Should_Return_Delete_Data();
    Task Default_GetAuditLogsById_Should_Return_Update_And_Delete_Data();
}
