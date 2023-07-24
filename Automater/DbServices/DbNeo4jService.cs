using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Serilog;
using Neo4j.Driver;
using System.Runtime.InteropServices;

namespace Automater.DbServices
{
    public class DbNeo4jService 
    {
        private readonly Serilog.ILogger _logger;

        public DbNeo4jService (Serilog.ILogger logger)
        {
            _logger = logger;
        }

        public async Task UpdatePointsAsync()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string? uri = configuration.GetSection("DbConnection:uri").Value;
            string? user = configuration.GetSection("DbConnection:user").Value;
            string? password = configuration.GetSection("DbConnection:password").Value;

            string date = DateTime.Now.ToString("dd-MM-yyyy");

            using (var driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password)))
            using (var session = driver.AsyncSession())
            {

                var checkExistingQuery = "MATCH (p:points { date: $date }) RETURN p";
                var parameters = new { date };
                var result = await session.RunAsync(checkExistingQuery, parameters);
                var recordExists = await result.SingleAsync(r => r["recordExists"].As<bool>());
            }
        }
    }
}
