using Dto.Security.ApplicationUserPermission;

namespace IntegrationTests.Security.Shared.Utilities.Contracts;

public interface IApplicationUserPermissionUtilities
{
    public Task DeleteAllRecords();
    public Task<List<ApplicationUserPermissionDto>> CreateActiveTestRecords(int applicationId, int applicationUserId, int permissionId, short numberOfRecordsToCreate = 5);
    public Task<List<ApplicationUserPermissionDto>> CreateInactiveTestRecords(int applicationId, int applicationUserId, int permissionId, short numberOfRecordsToCreate = 5);
    public InsertUpdateApplicationUserPermissionRequest CreateInsertUpdateRequestWithMaxLengthErrors(int applicationId, int applicationUserId, int permissionId);
    public InsertUpdateApplicationUserPermissionRequest CreateInsertUpdateRequestWithSpecificValues(int applicationId, int applicationUserId, int permissionId, bool active = true);
    public InsertUpdateApplicationUserPermissionRequest ConvertApplicationUserPermissionDtoToInsertUpdateRequest(ApplicationUserPermissionDto req);
    public void VerifyTestRecordValuesMatch(ApplicationUserPermissionDto recordA, ApplicationUserPermissionDto recordB);
    public Dictionary<string, List<string>> GetExpectedRecordDoesNotExistErrors();
    public Dictionary<string, List<string>> GetExpectedUniqueFieldErrors();
    public Dictionary<string, List<string>> GetExpectedRequiredFieldErrors();
    public Dictionary<string, List<string>> GetExpectedMaxLengthFieldErrors();
}
