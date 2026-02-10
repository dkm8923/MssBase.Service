using Contract.Security;

namespace MssBase.Service.Shared
{
    public class SecurityConnectionStrings : ISecurityConnectionStrings
    {
        public string SecurityConnectionStringReadWrite { get; set; }
        public string SecurityConnectionStringReadOnly { get; set; }
    }
}
