using Contract.Common;

namespace MssBase.Service.Shared.ConnectionStrings;

public class CommonConnectionStrings : ICommonConnectionStrings
{
    public string ReadWrite { get; set; }
    public string ReadOnly { get; set; }
}