using Dto.Common.UnitGroupColumn;
using Dto.Common.UnitGroupColumn.Logic;
using Shared.Models;

namespace Contract.Common.UnitGroupColumn
{
    public interface IUnitGroupColumnLogic
    {
        public Task<ErrorValidationResult<IEnumerable<UnitGroupColumnDto>>> GetAll(BaseLogicGet req);
        public Task<ErrorValidationResult<UnitGroupColumnDto>> GetById(int unitGroupColumnId, BaseLogicGet req);
        public Task<ErrorValidationResult<IEnumerable<UnitGroupColumnDto>>> Filter(FilterUnitGroupColumnLogicRequest req);
        public Task<ErrorValidationResult<UnitGroupColumnDto>> Insert(InsertUpdateUnitGroupColumnRequest req);
        public Task<ErrorValidationResult<UnitGroupColumnDto>> Update(int unitGroupColumnId, InsertUpdateUnitGroupColumnRequest req);
        public Task<ErrorValidationResult> Delete(int unitGroupColumnId);
    }
}
