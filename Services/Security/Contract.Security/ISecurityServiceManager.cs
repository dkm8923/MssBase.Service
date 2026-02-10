using Contract.Security.Application;

namespace Contract.Security
{
    public interface ISecurityServiceManager
    {
        public IApplicationService Application { get; }
    }
}