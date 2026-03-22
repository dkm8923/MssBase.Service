using Dto.Security.Role;

namespace IntegrationTests.Security.Shared.Utilities.Contracts;

public interface IRoleUtilities
{
    public Task DeleteAllRecords();
    public Task<List<RoleDto>> CreateActiveTestRecords(int applicationId, short numberOfRecordsToCreate = 5);
    public Task<List<RoleDto>> CreateInactiveTestRecords(int applicationId, short numberOfRecordsToCreate = 5);
    public Task<RoleDto> CreateSingleRoleTestRecord(int applicationId,bool active = true);
    public InsertUpdateRoleRequest CreateInsertUpdateRequestWithMaxLengthErrors();
    public InsertUpdateRoleRequest CreateInsertUpdateRequestWithRandomValues(int applicationId, bool active = true);
    public InsertUpdateRoleRequest ConvertRoleDtoToInsertUpdateRequest(RoleDto req);
    public void VerifyTestRecordValuesMatch(RoleDto recordA, RoleDto recordB);
    public Dictionary<string, List<string>> GetExpectedRecordDoesNotExistErrors();
    public Dictionary<string, List<string>> GetExpectedUniqueFieldErrors();
    public Dictionary<string, List<string>> GetExpectedRequiredFieldErrors();
    public Dictionary<string, List<string>> GetExpectedMaxLengthFieldErrors();
    public Dictionary<string, List<string>> GetExpectedInvalidApplicationIdFieldErrors();
}
