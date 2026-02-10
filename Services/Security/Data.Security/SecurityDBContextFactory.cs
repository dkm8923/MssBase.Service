using Contract.Security;
using Data.Security.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Logic.Common;

namespace Data.Security
{
    public class SecurityDBContextFactory
    {
        private readonly ISecurityConnectionStrings _connectionStrings;

        public SecurityDBContextFactory(ISecurityConnectionStrings connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public SecurityDBContext CreateContextReadWrite()
        {
            return this.CreateSqlServerContext(_connectionStrings.SecurityConnectionStringReadWrite);
        }

        public SecurityDBContext CreateContextReadOnly()
        {
            return this.CreateSqlServerContext(_connectionStrings.SecurityConnectionStringReadOnly);
        }

        public SecurityDBContext CreateSqlServerContext(string connectionString)
        {
            var decryptedConnectionString = connectionString;

            // if (CommonUtilities.IsBase64String(connectionString))
            // {
            //     decryptedConnectionString = Encryption.Decrypt(connectionString);
            // }

            var options = new DbContextOptionsBuilder<SecurityDBContext>()
            .UseSqlServer(decryptedConnectionString)
            .LogTo(Console.WriteLine)
            .Options;

            return new SecurityDBContext(options);
        }
    }
}
