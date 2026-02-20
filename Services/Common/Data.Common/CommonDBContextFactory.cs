//using Data.Common.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;

namespace Data.Common
{
    public class CommonDBContextFactory
    {
        private readonly IDatabaseConnectionStrings _connectionStrings;

        public CommonDBContextFactory(IDatabaseConnectionStrings connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        // public CommonDBContext CreateContextReadWrite()
        // {
        //     return this.CreateSqlServerContext(_connectionStrings.ReadWrite);
        // }

        // public CommonDBContext CreateContextReadOnly()
        // {
        //     return this.CreateSqlServerContext(_connectionStrings.ReadOnly);
        // }

        // public CommonDBContext CreateSqlServerContext(string connectionString)
        // {
        //     var decryptedConnectionString = connectionString;

        //     // if (CommonUtilities.IsBase64String(connectionString))
        //     // {
        //     //     decryptedConnectionString = Encryption.Decrypt(connectionString);
        //     // }

        //     var options = new DbContextOptionsBuilder<CommonDBContext>()
        //     .UseSqlServer(decryptedConnectionString)
        //     .Options;

        //     return new CommonDBContext(options);
        // }
    }
}
