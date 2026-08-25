namespace IntegrationTests.Shared.Utilities.Contracts.Controller;

public interface IDefaultControllerTestsGetAuditLogsById
{
    public Task Default_GetAuditLogById_Should_Return_Record();
    public Task Default_GetAuditLogById_Should_Return_Unauthorized();
    public Task Default_GetAuditLogById_Should_Return_Forbidden();
    public Task Default_GetAuditLogById_Should_Return_NotFound();
}
