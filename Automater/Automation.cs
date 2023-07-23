using OpenQA.Selenium.Edge;
using Serilog;
using Automater;

public static class Automation
{
    private static BingFunctions CreateDriver(ClientType type, ILogger logger)
    {
        var options = new EdgeOptions();
        options.AddExcludedArgument("enable-logging");
        if (type == ClientType.Mobile) options.AddArgument("--user-agent=Mozilla/5.0 (Linux; Android 11; SM-G998B Build/RP1A.200720.012; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/91.0.4472.120 Mobile Safari/537.36");
        var driver = new EdgeDriver(options);
        
        return new BingFunctions(driver, logger);
    }

    public static void Automate(ClientType type, ILogger logger, List<Action> actions)
    {
        CreateDriver(type, logger);

        Action LoginAction = actions.Find(action => action.Method.Name == "RewardsLogin");

        try
        {
            actions.
        }
        catch (Exception ex)
        {

        }
    }
}