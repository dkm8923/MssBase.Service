using Dto.Security.RolePermission;

namespace IntegrationTests.Security.Shared.Utilities.Contracts;

public interface IRolePermissionUtilities
{
    public Task DeleteAllRecords();
    public Task<List<RolePermissionDto>> CreateActiveTestRecords(int applicationId, int applicationUserId, int permissionId, short numberOfRecordsToCreate = 5);
    public Task<List<RolePermissionDto>> CreateInactiveTestRecords(int applicationId, int applicationUserId, int permissionId, short numberOfRecordsToCreate = 5);
    public InsertUpdateRolePermissionRequest CreateInsertUpdateRequestWithMaxLengthErrors(int applicationId, int applicationUserId, int permissionId);
    public InsertUpdateRolePermissionRequest CreateInsertUpdateRequestWithSpecificValues(int applicationId, int applicationUserId, int permissionId, bool active = true);
    public InsertUpdateRolePermissionRequest ConvertRolePermissionDtoToInsertUpdateRequest(RolePermissionDto req);
    public void VerifyTestRecordValuesMatch(RolePermissionDto recordA, RolePermissionDto recordB);
    public Dictionary<string, List<string>> GetExpectedRecordDoesNotExistErrors();
    public Dictionary<string, List<string>> GetExpectedUniqueFieldErrors();
    public Dictionary<string, List<string>> GetExpectedRequiredFieldErrors();
    public Dictionary<string, List<string>> GetExpectedMaxLengthFieldErrors();
}
