using Contract.Security;
using Contract.Security.Application;

namespace Service.Security
{
    public class SecurityServiceManager : ISecurityServiceManager
    {
        private IApplicationService _applicationService;

        public SecurityServiceManager(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        public IApplicationService Application
        {
            get
            {
                return _applicationService;
            }
        }
    }
}
