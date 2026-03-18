using Dto.Security.ApplicationUserRole;

namespace IntegrationTests.Security.Shared.Utilities.Contracts;

public interface IApplicationUserRoleUtilities
{
    public Task DeleteAllRecords();
    public Task<List<ApplicationUserRoleDto>> CreateActiveTestRecords(int applicationId, int applicationUserId, int roleId, short numberOfRecordsToCreate = 5);
    public Task<List<ApplicationUserRoleDto>> CreateInactiveTestRecords(int applicationId, int applicationUserId, int roleId, short numberOfRecordsToCreate = 5);
    public Task<ApplicationUserRoleDto> CreateSingleApplicationUserRoleTestRecord(int applicationId, int applicationUserId, int roleId, bool active = true);
    public InsertUpdateApplicationUserRoleRequest CreateInsertUpdateRequestWithMaxLengthErrors(int applicationId, int applicationUserId, int roleId);
    public InsertUpdateApplicationUserRoleRequest CreateInsertUpdateRequestWithSpecificValues(int applicationId, int applicationUserId, int roleId, bool active = true);
    public InsertUpdateApplicationUserRoleRequest ConvertApplicationUserRoleDtoToInsertUpdateRequest(ApplicationUserRoleDto req);
    public void VerifyTestRecordValuesMatch(ApplicationUserRoleDto recordA, ApplicationUserRoleDto recordB);
    public Dictionary<string, List<string>> GetExpectedRecordDoesNotExistErrors();
    public Dictionary<string, List<string>> GetExpectedUniqueFieldErrors();
    public Dictionary<string, List<string>> GetExpectedRequiredFieldErrors();
    public Dictionary<string, List<string>> GetExpectedMaxLengthFieldErrors();
}

