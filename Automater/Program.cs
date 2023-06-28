using OpenQA.Selenium.Edge;
using Spectre.Console;

class Program
{
    static void Main(string[] args)
    {
        // come back to this later - Do I even want a menu??
        /*
        var bingSelection = AnsiConsole.Prompt(
          new MultiSelectionPrompt<string>()
            .Title("Choose what functions to run...")
            .InstructionsText(
              "Press <space> to select. <Enter> to accept"
            )
            .AddChoiceGroup("All", new[] {
              "Automated Searches",
              "Activate Reward Cards",
              "Activate Question And PunchCards"
            })
        );
        */

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

        // testing code
        /*
        string url = "https://rewards.bing.com/dashboard/ENAU_pcparent_FY23Gaming_punchcard";

        driver.Navigate().GoToUrl(url);
        var cardElements = driver.FindElements(By.CssSelector("div.btn-primary.btn.win-color-border-0.card-button-height.pull-left.margin-right-24.padding-left-24.padding-right-24"));
        foreach (var cardElement in cardElements)
        {
            Console.WriteLine(cardElement.Text);
        }
        */
    }
}

