using Dto.Common.Unit;
using Dto.Common.Unit.Logic;
using Shared.Models;

namespace Contract.Common.Unit
{
    public interface IUnitLogic
    {
        public Task<ErrorValidationResult<IEnumerable<UnitDto>>> GetAll(BaseLogicGet req, bool includeRelated = false);
        public Task<ErrorValidationResult<UnitDto>> GetById(int UnitId, BaseLogicGet req, bool includeRelated = false);
        public Task<ErrorValidationResult<IEnumerable<UnitDto>>> Filter(FilterUnitLogicRequest req);
        public Task<ErrorValidationResult<UnitDto>> Insert(InsertUpdateUnitRequest req);
        public Task<ErrorValidationResult<UnitDto>> Update(int UnitId, InsertUpdateUnitRequest req);
        public Task<ErrorValidationResult> Delete(int unitId);
    }
}
