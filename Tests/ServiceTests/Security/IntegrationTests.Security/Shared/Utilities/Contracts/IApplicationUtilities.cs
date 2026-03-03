using Dto.Security.Application;

namespace IntegrationTests.Security.Shared.Utilities.Contracts;

 public interface IApplicationUtilities
    {
        public Task DeleteAllRecords();
        public Task<List<ApplicationDto>> CreateTestRecords(short numberOfRecordsToCreate = 5, bool active = true);
        public Task<ApplicationDto> CreateSingleApplicationTestRecord(bool active = true);
        //public Task<ApplicationDto> CreateSingleApplicationTestRecordWithSpecificValues(bool active = true);
        public Task<ApplicationDto> CreateSingleApplicationTestRecordWithSpecificValues(InsertUpdateApplicationRequest req = null);
        public InsertUpdateApplicationRequest CreateInsertUpdateRequestWithMaxLengthErrors();
        public InsertUpdateApplicationRequest CreateInsertUpdateRequestWithRandomValues(bool active = true);
        public InsertUpdateApplicationRequest ConvertApplicationDtoToInsertUpdateRequest(ApplicationDto req);
        public void VerifyTestRecordValuesMatch(ApplicationDto recordA, ApplicationDto recordB);
        public Dictionary<string, List<string>> GetExpectedRecordDoesNotExistErrors();
        public Dictionary<string, List<string>> GetExpectedUniqueFieldErrors();
        public Dictionary<string, List<string>> GetExpectedRequiredFieldErrors();
        public Dictionary<string, List<string>> GetExpectedMaxLengthFieldErrors();
    }
