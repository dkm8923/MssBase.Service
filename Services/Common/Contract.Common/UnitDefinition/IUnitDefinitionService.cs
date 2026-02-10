using Dto.Common.UnitDefinition;
using Dto.Common.UnitDefinition.Service;
using Shared.Models;

namespace Contract.Common.UnitDefinition
{
    public interface IUnitDefinitionService
    {
        public Task<ErrorValidationResult<IEnumerable<UnitDefinitionDto>>> GetAll(BaseServiceGet req);
        public Task<ErrorValidationResult<UnitDefinitionDto>> GetById(short unitDefinitionId, BaseServiceGet req);
        public Task<ErrorValidationResult<IEnumerable<UnitDefinitionDto>>> Filter(FilterUnitDefinitionServiceRequest req);
        public Task<ErrorValidationResult<UnitDefinitionDto>> Insert(InsertUpdateUnitDefinitionRequest req);
        public Task<ErrorValidationResult<UnitDefinitionDto>> Update(short unitDefinitionId, InsertUpdateUnitDefinitionRequest req);
        public Task<ErrorValidationResult> Delete(short unitDefinitionId);
    }
}
