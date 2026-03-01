using System;
using Contract.Security;

namespace MssBase.Service.Shared.ConnectionStrings;

public class SecurityConnectionStrings : ISecurityConnectionStrings
{
    public string ReadWrite { get; set; }
    public string ReadOnly { get; set; }
}
