using Dto.Security.ApplicationUserPermission;

namespace IntegrationTests.Security.Shared.Utilities.Contracts;

public interface IApplicationUserPermissionUtilities
{
    // public Task<int> ClearTestTablesAndReturnApplicationId(IApplicationUtilities applicationUtilities);
    public Task DeleteAllRecords();
    // public Task<List<ApplicationUserPermissionDto>> CreateTestRecords(int applicationId, short numberOfRecordsToCreate = 5, bool active = true);
    // public Task<ApplicationUserPermissionDto> CreateSingleApplicationUserPermissionTestRecord(int applicationId,bool active = true);
    // //public Task<ApplicationUserPermissionDto> CreateSingleApplicationUserPermissionTestRecordWithSpecificValues(InsertUpdateApplicationUserPermissionRequest req = null);
    // public InsertUpdateApplicationUserPermissionRequest CreateInsertUpdateRequestWithMaxLengthErrors();
    // public InsertUpdateApplicationUserPermissionRequest CreateInsertUpdateRequestWithRandomValues(int applicationId, bool active = true);
    // public InsertUpdateApplicationUserPermissionRequest ConvertApplicationUserPermissionDtoToInsertUpdateRequest(ApplicationUserPermissionDto req);
    // public void VerifyTestRecordValuesMatch(ApplicationUserPermissionDto recordA, ApplicationUserPermissionDto recordB);
    // public Dictionary<string, List<string>> GetExpectedRecordDoesNotExistErrors();
    // public Dictionary<string, List<string>> GetExpectedUniqueFieldErrors();
    // public Dictionary<string, List<string>> GetExpectedRequiredFieldErrors();
    // public Dictionary<string, List<string>> GetExpectedMaxLengthFieldErrors();
}
