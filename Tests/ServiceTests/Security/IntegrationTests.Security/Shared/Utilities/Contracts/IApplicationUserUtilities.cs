using Dto.Security.ApplicationUser;

namespace IntegrationTests.Security.Shared.Utilities.Contracts;

public interface IApplicationUserUtilities
{
    public Task DeleteAllRecords();
    public Task<List<ApplicationUserDto>> CreateActiveTestRecords(int applicationId, short numberOfRecordsToCreate = 5);
    public Task<List<ApplicationUserDto>> CreateInactiveTestRecords(int applicationId, short numberOfRecordsToCreate = 5);
    public Task<ApplicationUserDto> CreateSingleApplicationUserTestRecord(int applicationId,bool active = true);
    public InsertUpdateApplicationUserRequest CreateInsertUpdateRequestWithMaxLengthErrors();
    public InsertUpdateApplicationUserRequest CreateInsertUpdateRequestWithRandomValues(int applicationId, bool active = true);
    public InsertUpdateApplicationUserRequest ConvertApplicationUserDtoToInsertUpdateRequest(ApplicationUserDto req);
    public void VerifyTestRecordValuesMatch(ApplicationUserDto recordA, ApplicationUserDto recordB);
    public Dictionary<string, List<string>> GetExpectedRecordDoesNotExistErrors();
    public Dictionary<string, List<string>> GetExpectedUniqueFieldErrors();
    public Dictionary<string, List<string>> GetExpectedRequiredFieldErrors();
    public Dictionary<string, List<string>> GetExpectedMaxLengthFieldErrors();
    public Dictionary<string, List<string>> GetExpectedInvalidEmailFieldErrors();
}
