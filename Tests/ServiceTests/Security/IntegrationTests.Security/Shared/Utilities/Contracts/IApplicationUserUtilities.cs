using Dto.Security.ApplicationUser;

namespace IntegrationTests.Security.Shared.Utilities.Contracts;

public interface IApplicationUserUtilities
{
    public Task DeleteAllRecords();
    public Task<List<ApplicationUserDto>> CreateActiveTestRecords(int applicationId, int userId, short numberOfRecordsToCreate = 5);
    public Task<List<ApplicationUserDto>> CreateInactiveTestRecords(int applicationId, int userId, short numberOfRecordsToCreate = 5);
    public Task<ApplicationUserDto> CreateActiveReadOnlyTestRecord(int applicationId, int userId);
    public Task<ApplicationUserDto> CreateInactiveReadOnlyTestRecord(int applicationId, int userId);
    public Task<ApplicationUserDto> CreateSingleApplicationUserTestRecord(int applicationId, int userId, bool active = true);
    public InsertUpdateApplicationUserRequest CreateInsertUpdateRequestWithMaxLengthErrors(int applicationId, int userId);
    public InsertUpdateApplicationUserRequest CreateInsertUpdateRequestWithSpecificValues(int applicationId, int userId, bool active = true);
    public InsertUpdateApplicationUserRequest ConvertApplicationUserDtoToInsertUpdateRequest(ApplicationUserDto req);
    public void VerifyTestRecordValuesMatch(ApplicationUserDto recordA, ApplicationUserDto recordB);
    public void VerifyIncludeRelatedDataOnApplicationUser(ApplicationUserDto applicationUser, bool includeInactive = false);
    public Dictionary<string, List<string>> GetExpectedRecordDoesNotExistErrors();
    public Dictionary<string, List<string>> GetExpectedUniqueFieldErrors();
    public Dictionary<string, List<string>> GetExpectedReadOnlyErrors();
    public Dictionary<string, List<string>> GetExpectedRequiredFieldErrors();
    public Dictionary<string, List<string>> GetExpectedMaxLengthFieldErrors();
}
