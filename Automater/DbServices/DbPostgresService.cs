using Automater.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using OpenQA.Selenium.DevTools.V111.DOM;
using Serilog;

namespace Automater.DbServices
{
    public class DbService
    {
        private readonly ILogger _logger;

        public DbService(ILogger logger)
        {
            _logger = logger;
        }

        public void UpdatePoints(BingPoints points)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string date = DateTime.Now.ToString("dd-MM-yyyy");

            string checkExistingQuery = "SELECT 1 FROM bing_points WHERE date=@date";
            string insertQuery = "INSERT INTO 'bing_points' VALUES (@date, @pointsEarnedToday, @totalPointsEarned)";
            string updateQuery = $@"
                UPDATE bing_points 
                SET points_today = @pointsEarnedToday, 
                    points_total = @totalpointsEarned
                WHERE date=@date";

            var connection = new NpgsqlConnection(configuration.GetSection("PostgresDbConnection").Value);
            connection.Open();

            using var checkExistingCommand = new NpgsqlCommand(checkExistingQuery, connection);
            checkExistingCommand.Parameters.AddWithValue("date", date);

            bool recordExists = checkExistingCommand.ExecuteScalar() != null;

            if (!recordExists)
            {
                using var insertCommand = new NpgsqlCommand(insertQuery, connection)
                {
                    Parameters =
                    {
                        new NpgsqlParameter("date", NpgsqlTypes.NpgsqlDbType.Date) { Value = date },
                        new NpgsqlParameter("pointsEarnedToday", NpgsqlTypes.NpgsqlDbType.Integer) { Value = points.Today },
                        new NpgsqlParameter("totalPointsEarned", NpgsqlTypes.NpgsqlDbType.Integer) { Value = points.Total }
                    }
                };
                SqlExecuteCommand(insertCommand);
            }
            else
            {
                using var updateCommand = new NpgsqlCommand(updateQuery, connection)
                {
                    Parameters =
                    {
                        new NpgsqlParameter("date", NpgsqlTypes.NpgsqlDbType.Date) { Value = date },
                        new NpgsqlParameter("pointsEarnedToday", NpgsqlTypes.NpgsqlDbType.Integer) { Value = points.Today },
                        new NpgsqlParameter("totalPointsEarned", NpgsqlTypes.NpgsqlDbType.Integer) { Value = points.Total }
                    }
                };
                SqlExecuteCommand(updateCommand);
            }
        }

        private void SqlExecuteCommand(NpgsqlCommand sqlCommand)
        {
            int rowsAffected = sqlCommand.ExecuteNonQuery();
            if (rowsAffected > 0)
            {
                _logger.Information("Record inserted successfully");
            }
            else
            {
                _logger.Error("Failed to insert the record");
            }
        }
    }
}
