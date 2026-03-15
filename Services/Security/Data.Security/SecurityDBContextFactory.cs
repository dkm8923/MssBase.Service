using Contract.Security;
using Data.Security.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Logic.Common;
using Shared.Contracts;

namespace Data.Security
{
    public class SecurityDBContextFactory
    {
        private readonly IDatabaseConnectionStrings _connectionStrings;

        public SecurityDBContextFactory(IDatabaseConnectionStrings connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public SecurityDBContext CreateContextReadWrite()
        {
            return this.CreateSqlServerContext(_connectionStrings.ReadWrite);
        }

        public SecurityDBContext CreateContextReadOnly()
        {
            return this.CreateSqlServerContext(_connectionStrings.ReadOnly);
        }

        public SecurityDBContext CreateSqlServerContext(string connectionString)
        {
            var decryptedConnectionString = connectionString;

            // if (CommonUtilities.IsBase64String(connectionString))
            // {
            //     decryptedConnectionString = Encryption.Decrypt(connectionString);
            // }

            var isDevelopment = string.Equals(
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                "Development",
                StringComparison.OrdinalIgnoreCase);

            var optionsBuilder = new DbContextOptionsBuilder<SecurityDBContext>()
                .UseSqlServer(decryptedConnectionString)
                .LogTo(Console.WriteLine);

            if (isDevelopment)
            {
                optionsBuilder.EnableSensitiveDataLogging();
            }

            var options = optionsBuilder.Options;

            return new SecurityDBContext(options);
        }
    }
}
