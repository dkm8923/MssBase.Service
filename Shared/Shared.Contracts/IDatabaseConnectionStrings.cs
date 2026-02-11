using System;

namespace Shared.Contracts;

public interface IDatabaseConnectionStrings
{
    public string ReadWrite { get; set; }
    public string ReadOnly { get; set; }
}
