using AngleSharp;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automater.DbServices
{
    public class DbService
    {
        private readonly IConfiguration configuration;
        private readonly ILogger _logger;

        public DbService (IConfiguration configuration, ILogger logger)
        {
            this.configuration = configuration ?? throw new ArgumentNullException (nameof (configuration));
            _logger = logger;
        }

        public void UpdatePoints(int pointsEarnedToday, int totalPointsEarned)
        {
            string? connectionString = configuration.GetConnectionString("DbConnectionString");
        }
    }
}
