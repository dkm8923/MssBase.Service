using Dto.Security.Permission;

namespace IntegrationTests.Security.Shared.Utilities.Contracts;

public interface IPermissionUtilities
{
    public Task<int> ClearTestTablesAndReturnApplicationId(IApplicationUtilities applicationUtilities);
    public Task DeleteAllRecords();
    public Task<List<PermissionDto>> CreateActiveTestRecords(int applicationId, short numberOfRecordsToCreate = 5);
    public Task<List<PermissionDto>> CreateInactiveTestRecords(int applicationId, short numberOfRecordsToCreate = 5);
    public Task<PermissionDto> CreateSinglePermissionTestRecord(int applicationId,bool active = true);
    public InsertUpdatePermissionRequest CreateInsertUpdateRequestWithMaxLengthErrors();
    public InsertUpdatePermissionRequest CreateInsertUpdateRequestWithRandomValues(int applicationId, bool active = true);
    public InsertUpdatePermissionRequest ConvertPermissionDtoToInsertUpdateRequest(PermissionDto req);
    public void VerifyTestRecordValuesMatch(PermissionDto recordA, PermissionDto recordB);
    public Dictionary<string, List<string>> GetExpectedRecordDoesNotExistErrors();
    public Dictionary<string, List<string>> GetExpectedUniqueFieldErrors();
    public Dictionary<string, List<string>> GetExpectedRequiredFieldErrors();
    public Dictionary<string, List<string>> GetExpectedMaxLengthFieldErrors();
    public Dictionary<string, List<string>> GetExpectedInvalidApplicationIdFieldErrors();
}
