using Dto.Security.ApplicationUser;

namespace IntegrationTests.Security.Shared.Utilities.Contracts;

public interface IApplicationUserUtilities
{
    public Task<int> ClearTestTablesAndReturnApplicationId(IApplicationUtilities applicationUtilities);
    public Task DeleteAllRecords();
    public Task<List<ApplicationUserDto>> CreateTestRecords(int applicationId, short numberOfRecordsToCreate = 5, bool active = true);
    public Task<ApplicationUserDto> CreateSingleApplicationUserTestRecord(int applicationId,bool active = true);
    //public Task<ApplicationUserDto> CreateSingleApplicationUserTestRecordWithSpecificValues(InsertUpdateApplicationUserRequest req = null);
    public InsertUpdateApplicationUserRequest CreateInsertUpdateRequestWithMaxLengthErrors();
    public InsertUpdateApplicationUserRequest CreateInsertUpdateRequestWithRandomValues(int applicationId, bool active = true);
    public InsertUpdateApplicationUserRequest ConvertApplicationUserDtoToInsertUpdateRequest(ApplicationUserDto req);
    public void VerifyTestRecordValuesMatch(ApplicationUserDto recordA, ApplicationUserDto recordB);
    public Dictionary<string, List<string>> GetExpectedRecordDoesNotExistErrors();
    public Dictionary<string, List<string>> GetExpectedUniqueFieldErrors();
    public Dictionary<string, List<string>> GetExpectedRequiredFieldErrors();
    public Dictionary<string, List<string>> GetExpectedMaxLengthFieldErrors();
}
