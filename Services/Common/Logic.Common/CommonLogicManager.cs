using Contract.Common.Unit;
using Contract.Common;

namespace Logic.Common  
{
    public class CommonLogicManager : ICommonLogicManager
    {
        private IUnitLogic _UnitLogic;
        
        public CommonLogicManager(IUnitLogic UnitLogic)
        {
            _UnitLogic = UnitLogic;
        }

        public IUnitLogic Unit
        {
            get
            {
                return _UnitLogic;
            }
        }
    }
}
