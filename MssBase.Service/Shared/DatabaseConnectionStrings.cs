using System;
using Shared.Contracts;

namespace MssBase.Service.Shared;

public class DatabaseConnectionStrings : IDatabaseConnectionStrings
{
    public required string ReadWrite { get; set; }
    public required string ReadOnly { get; set; }
}
