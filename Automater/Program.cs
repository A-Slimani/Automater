using Automater;
using Automater.DbServices;
using Serilog;

class Program
{
    static void Main(string[] args)
    {
        // LOGGING
        Log.Logger = LoggerConfig.ConfigureLogger();
        Log.Information("=== STARTING AUTOMATER ===");

        // DESKTOP
        var driver = Automation.CreateDriver(ClientType.Desktop);
        var bingFunctions = new BingFunctions(driver, Log.Logger);
        Automation.Automate(ClientType.Desktop, Log.Logger, bingFunctions);
    
        // MOBILE
        driver = Automation.CreateDriver(ClientType.Mobile);
        bingFunctions = new BingFunctions(driver, Log.Logger);
        Automation.Automate(ClientType.Mobile, Log.Logger, bingFunctions);

        var bingPoints = BingElements.GetPointsEarnedToday(driver);
        Log.Information($"POINTS EARNED TODAY: {bingPoints.Today}");

        // UPDATE DB
        DbService.UpdatePoints(bingPoints, Log.Logger);

        bingFunctions.CloseSelenium(10);
    }
}

