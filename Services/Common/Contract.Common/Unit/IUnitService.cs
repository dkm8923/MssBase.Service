using Dto.Common.Unit;
using Dto.Common.Unit.Service;
using Shared.Models;

namespace Contract.Common.Unit
{
    public interface IUnitService
    {
        public Task<ErrorValidationResult<IEnumerable<UnitDto>>> GetAll(BaseServiceGet req, bool includeRelated = false);
        public Task<ErrorValidationResult<UnitDto>> GetById(int UnitId, BaseServiceGet req, bool includeRelated = false);
        public Task<ErrorValidationResult<IEnumerable<UnitDto>>> Filter(FilterUnitServiceRequest req);
        public Task<ErrorValidationResult<UnitDto>> Insert(InsertUpdateUnitRequest req);
        public Task<ErrorValidationResult<UnitDto>> Update(int UnitId, InsertUpdateUnitRequest req);
        public Task<ErrorValidationResult> Delete(int UnitId);
    }
}
