using Dto.Common.UnitDefinition;
using Dto.Common.UnitDefinition.Logic;
using Shared.Models;

namespace Contract.Common.UnitDefinition
{
    public interface IUnitDefinitionLogic
    {
        public Task<ErrorValidationResult<IEnumerable<UnitDefinitionDto>>> GetAll(BaseLogicGet req);
        public Task<ErrorValidationResult<UnitDefinitionDto>> GetById(short unitDefinitionId, BaseLogicGet req);
        public Task<ErrorValidationResult<IEnumerable<UnitDefinitionDto>>> Filter(FilterUnitDefinitionLogicRequest req);
        public Task<ErrorValidationResult<UnitDefinitionDto>> Insert(InsertUpdateUnitDefinitionRequest req);
        public Task<ErrorValidationResult<UnitDefinitionDto>> Update(short unitDefinitionId, InsertUpdateUnitDefinitionRequest req);
        public Task<ErrorValidationResult> Delete(short unitDefinitionId);
    }
}
