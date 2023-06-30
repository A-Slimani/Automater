using OpenQA.Selenium.Edge;
using Spectre.Console;

class Program
{
    static void Main(string[] args)
    {
        var edgeOptions = new EdgeOptions();
        edgeOptions.AddExcludedArgument("enable-logging");
        var driver = new EdgeDriver(edgeOptions);
        var bingFunctions = new BingFunctions(driver);

        try
        {
            bingFunctions.AutomatedSearches();
            bingFunctions.ActivateRewardCards();
            bingFunctions.ActivateQuestAndPunchCards();
            bingFunctions.CloseSelenium(15);
        }
        catch (Exception ex)
        {
            bingFunctions.CloseSelenium(15);
            AnsiConsole.WriteException(ex);
        }

        /*
        driver.Navigate().GoToUrl("https://www.bing.com/search?q=Famous%20Asian%20cities&rnoreward=1&mkt=EN-AU&FORM=ML12JG&skipopalnative=true&rqpiodemo=1&filters=BTEPOKey:%22REWARDSQUIZ_ENAU_ThursdayBonus_20230629%22%20BTROID:%22Gamification_DailySet_ENAU_20230629_Child2%22%20BTROEC:%2210%22%20BTROMC:%2230%22");
        var earnedPoints = int.Parse(driver.FindElement(By.ClassName("rqECredits")).Text);
        var totalPoints = int.Parse(driver.FindElement(By.ClassName("rqMCredits")).Text);

        Console.WriteLine(earnedPoints);
        Console.WriteLine(totalPoints);
        */
    }
}

