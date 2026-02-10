using Contract.Common;

namespace MssBase.Service.Shared
{
    public record CommonConnectionStrings : ICommonConnectionStrings
    {
        public string CommonConnectionStringReadWrite { get; set; }
        public string CommonConnectionStringReadOnly { get; set; }
    }
}
