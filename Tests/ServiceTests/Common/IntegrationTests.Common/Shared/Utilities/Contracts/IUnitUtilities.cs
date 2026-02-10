using Dto.Common.Unit;
using Dto.Common.UnitDefinition;

namespace IntegrationTests.Common.Shared.Utilities.Contracts
{
    public interface IUnitUtilities
    {
        public Task DeleteAllRecords();
        public Task<List<UnitDto>> CreateTestRecords(short numberOfRecordsToCreate = 5, bool active = true);
        public Task<UnitDto> CreateSingleUnitTestRecord(bool active = true);
        public Task<UnitDto> CreateSingleUnitTestRecordWithSpecificValues(bool active = true);
        public InsertUpdateUnitRequest CreateInsertUpdateRequestWithMaxLengthErrors();
        public InsertUpdateUnitRequest CreateInsertUpdateRequestWithRandomValues(bool active = true);
        public InsertUpdateUnitRequest ConvertUnitDtoToInsertUpdateRequest(UnitDto req);
        public void VerifyTestRecordValuesMatch(UnitDto recordA, UnitDto recordB);
        public Dictionary<string, List<string>> GetExpectedRecordDoesNotExistErrors();
        public Dictionary<string, List<string>> GetExpectedUniqueFieldErrors();
        public Dictionary<string, List<string>> GetExpectedRequiredFieldErrors();
        public Dictionary<string, List<string>> GetExpectedMaxLengthFieldErrors();
    }
}
