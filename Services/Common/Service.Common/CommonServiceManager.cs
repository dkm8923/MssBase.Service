using Contract.Common;
using Contract.Common.CommonType;
using Contract.Common.Rate;
using Contract.Common.Unit;
using Contract.Common.UnitDefinition;
using Contract.Common.UnitGroupColumn;

namespace Service.Common
{
    public class CommonServiceManager : ICommonServiceManager
    {
        private IUnitService _UnitService;

        public CommonServiceManager(IUnitService UnitService)
        {
            _UnitService = UnitService;
        }

        public IUnitService Unit
        {
            get
            {
                return _UnitService;
            }
        }
    }
}
