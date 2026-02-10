using Dto.Common.UnitGroupColumn;
using Dto.Common.UnitGroupColumn.Service;
using Shared.Models;

namespace Contract.Common.UnitGroupColumn
{
    public interface IUnitGroupColumnService
    {
        public Task<ErrorValidationResult<IEnumerable<UnitGroupColumnDto>>> GetAll(BaseServiceGet req);
        public Task<ErrorValidationResult<UnitGroupColumnDto>> GetById(int UnitId, BaseServiceGet req);
        public Task<ErrorValidationResult<IEnumerable<UnitGroupColumnDto>>> Filter(FilterUnitGroupColumnServiceRequest req);
        public Task<ErrorValidationResult<UnitGroupColumnDto>> Insert(InsertUpdateUnitGroupColumnRequest req);
        public Task<ErrorValidationResult<UnitGroupColumnDto>> Update(int UnitId, InsertUpdateUnitGroupColumnRequest req);
        public Task<ErrorValidationResult> Delete(int UnitId);
    }
}
