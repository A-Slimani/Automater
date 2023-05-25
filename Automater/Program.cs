using OpenQA.Selenium.Edge;
using Spectre.Console;

class Program
{
  static void Main(string[] args)
  {
    // come back to this later
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

    var options = new EdgeOptions();
    options.AddExcludedArgument("enable-logging");
    var driver = new EdgeDriver(options);
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
      AnsiConsole.WriteException(ex);
      bingFunctions.CloseSelenium(15);
    }

  }
}
