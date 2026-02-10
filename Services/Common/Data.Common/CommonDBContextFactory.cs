using Contract.Common;
using Data.Common.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Logic.Common;

namespace Data.Common
{
    public class CommonDBContextFactory
    {
        private readonly ICommonConnectionStrings _connectionStrings;

        public CommonDBContextFactory(ICommonConnectionStrings connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public CommonDBContext CreateContextReadWrite()
        {
            return this.CreateSqlServerContext(_connectionStrings.CommonConnectionStringReadWrite);
        }

        public CommonDBContext CreateContextReadOnly()
        {
            return this.CreateSqlServerContext(_connectionStrings.CommonConnectionStringReadOnly);
        }

        public CommonDBContext CreateSqlServerContext(string connectionString)
        {
            var decryptedConnectionString = connectionString;

            // if (CommonUtilities.IsBase64String(connectionString))
            // {
            //     decryptedConnectionString = Encryption.Decrypt(connectionString);
            // }

            var options = new DbContextOptionsBuilder<CommonDBContext>()
            .UseSqlServer(decryptedConnectionString)
            .Options;

            return new CommonDBContext(options);
        }
    }
}
