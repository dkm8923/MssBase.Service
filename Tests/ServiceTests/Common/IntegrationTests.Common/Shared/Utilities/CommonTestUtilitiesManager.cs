using IntegrationTests.Common.Shared.Utilities.Contracts;

namespace IntegrationTests.Common.Shared.Utilities
{
    public class CommonTestUtilitiesManager : ICommonTestUtilitiesManager
    {
        private IUnitUtilities _unitUtilities;
        
        public CommonTestUtilitiesManager(IUnitUtilities unitUtilities)
        {
            _unitUtilities = unitUtilities;
        }

        public IUnitUtilities Unit
        {
            get
            {
                return _unitUtilities;
            }
        }
    }
}
