using Automater;
using Serilog;

class Program
{
    static void Main(string[] args)
    {
        // LOGGING
        string date = DateTime.Now.ToString("ddMMyy");

        Log.Logger = new LoggerConfiguration()
          .WriteTo.Console()
          .WriteTo.File($"logs/{date}.log")
          .CreateLogger();

        Log.Information("=== STARTING AUTOMATER ===");

        var driver = Automation.CreateDriver(ClientType.Desktop);
        var automation = new BingFunctions(driver, Log.Logger);

        try
        {
            if (automation.RewardsLogin())
            {
                automation.AutomatedSearches(ClientType.Desktop);
                automation.ActivateRewardCards();
                automation.ActivateQuestAndPunchCards();
            }
            automation.CloseSelenium(1);
        }
        catch (Exception ex)
        {
            automation.CloseSelenium(1);
            Log.Error(ex.ToString());
        }

        // MOBILE
        driver = Automation.CreateDriver(ClientType.Mobile);
        automation = new BingFunctions(driver, Log.Logger);

        try
        {
            if (automation.RewardsLogin())
            {
                automation.AutomatedSearches(ClientType.Mobile);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex.ToString());
        }

        Log.Information($"POINTS EARNED TODAY: {BingElements.GetPointsEarnedToday(driver)}");
        automation.CloseSelenium(15);
    }
}

