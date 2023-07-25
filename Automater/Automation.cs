using OpenQA.Selenium.Edge;
using Serilog;
using Automater;

public static class Automation
{
    public static EdgeDriver CreateDriver(ClientType type)
    {
        var options = new EdgeOptions();
        options.AddExcludedArgument("enable-logging");
        if (type == ClientType.Mobile) options.AddArgument("--user-agent=Mozilla/5.0 (Linux; Android 11; SM-G998B Build/RP1A.200720.012; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/91.0.4472.120 Mobile Safari/537.36");

        return new EdgeDriver(options);
    }

    public static void Automate(ClientType type, ILogger logger, BingFunctions bingFunctions)
    {
        try
        {
            if(bingFunctions.RewardsLogin())
            {
                bingFunctions.AutomatedSearches(type);
                if (type == ClientType.Desktop) bingFunctions.ActivateRewardCards();
            }
        }               
        catch (Exception ex)
        {
            logger.Error(ex.ToString());
        }
        finally
        {
            if (type == ClientType.Desktop) bingFunctions.CloseSelenium(1);
        }
    }
 }