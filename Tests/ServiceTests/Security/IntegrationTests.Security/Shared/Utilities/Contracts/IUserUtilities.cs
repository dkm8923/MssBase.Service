using Dto.Common.CommonRelationalData;
using Dto.Security.User;

namespace IntegrationTests.Security.Shared.Utilities.Contracts;

public interface IUserUtilities
{
    public Task DeleteAllRecords();
    public Task<List<UserDto>> CreateActiveTestRecords(short numberOfRecordsToCreate = 5);
    public Task<List<UserDto>> CreateInactiveTestRecords(short numberOfRecordsToCreate = 5);
    public Task<List<UserDto>> CreateActiveReadOnlyTestRecords(short numberOfRecordsToCreate = 5);
    public Task<List<UserDto>> CreateInactiveReadOnlyTestRecords(short numberOfRecordsToCreate = 5);
    public Task<UserDto> CreateSingleUserTestRecord(bool active = true);
    public InsertUpdateUserRequest CreateInsertUpdateRequestWithMaxLengthErrors();
    public InsertUpdateUserRequest CreateInsertUpdateRequestWithRandomValues(bool active = true);
    public InsertUpdateUserRequest ConvertUserDtoToInsertUpdateRequest(UserDto req);
    public void VerifyTestRecordValuesMatch(UserDto recordA, UserDto recordB);
    public void VerifyIncludeRelatedDataOnUser(UserDto user, bool includeInactive = false);
    public Task<FilterCommonRelationalDataDto> GetCommonRelationalDataForUserInsertUpdateValidation();
    public Dictionary<string, List<string>> GetExpectedRecordDoesNotExistErrors();
    public Dictionary<string, List<string>> GetExpectedUniqueFieldErrors();
    public Dictionary<string, List<string>> GetExpectedReadOnlyErrors();
    public Dictionary<string, List<string>> GetExpectedApplicationUserPermissionForeignKeyErrors();
    public Dictionary<string, List<string>> GetExpectedApplicationUserRoleForeignKeyErrors();
    public Dictionary<string, List<string>> GetExpectedRequiredFieldErrors();
    public Dictionary<string, List<string>> GetExpectedMaxLengthFieldErrors();
    public Dictionary<string, List<string>> GetExpectedInvalidEmailFieldErrors();
    public Dictionary<string, List<string>> GetExpectedChangePasswordRequiredFieldErrors();
    public Dictionary<string, List<string>> GetExpectedChangePasswordMinMaxLengthErrors();
    public Dictionary<string, List<string>> GetExpectedChangePasswordInvalidPasswordErrors();
    public Dictionary<string, List<string>> GetExpectedChangePasswordUpperCaseRequiredErrors();
    public Dictionary<string, List<string>> GetExpectedChangePasswordLowerCaseRequiredErrors();
    public Dictionary<string, List<string>> GetExpectedChangePasswordSpecialCharacterRequiredErrors();
    public Dictionary<string, List<string>> GetExpectedChangePasswordNumberRequiredErrors();
}