using Automater.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Serilog;

namespace Automater.DbServices
{
    public static class DbService
    {
        public static void UpdatePoints(BingPoints points, ILogger logger)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string date = DateTime.Now.ToString("dd-MM-yyyy");

            string checkExistingQuery = "SELECT 1 FROM bing_points WHERE date = @date";
            string insertQuery = "INSERT INTO bing_points VALUES (@date, @pointsEarnedToday, @totalPointsEarned)";
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
                        new NpgsqlParameter("date", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = date },
                        new NpgsqlParameter("pointsEarnedToday", NpgsqlTypes.NpgsqlDbType.Integer) { Value = points.Today },
                        new NpgsqlParameter("totalPointsEarned", NpgsqlTypes.NpgsqlDbType.Integer) { Value = points.Total }
                    }
                };
                SqlExecuteCommand(insertCommand, logger);
            }
            else
            {
                using var updateCommand = new NpgsqlCommand(updateQuery, connection)
                {
                    Parameters =
                    {
                        new NpgsqlParameter("date", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = date },
                        new NpgsqlParameter("pointsEarnedToday", NpgsqlTypes.NpgsqlDbType.Integer) { Value = points.Today },
                        new NpgsqlParameter("totalPointsEarned", NpgsqlTypes.NpgsqlDbType.Integer) { Value = points.Total }
                    }
                };
                SqlExecuteCommand(updateCommand, logger);
            }
        }

        private static void SqlExecuteCommand(NpgsqlCommand sqlCommand, ILogger logger)
        {
            int rowsAffected = sqlCommand.ExecuteNonQuery();
            if (rowsAffected > 0)
            {
                logger.Information("Record inserted successfully");
            }
            else
            {
                logger.Error("Failed to insert the record");
            }
        }
    }
}
