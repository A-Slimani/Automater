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
      bingFunctions.ActivateQuestionCardsAndPolls("https://www.bing.com/search?q=Famous%20artists&rnoreward=1&mkt=EN-AU&FORM=ML12JG&skipopalnative=true&rqpiodemo=1&filters=BTEPOKey:%22REWARDSQUIZ_ENAU_MicrosoftRewardsQuizCB_20230523%22%20BTROID:%22Gamification_DailySet_ENAU_20230523_Child2%22%20BTROEC:%220%22%20BTROMC:%2230%22");
      /*
      bingFunctions.AutomatedSearches();
      bingFunctions.ActivateRewardCards();
      bingFunctions.ActivateQuestAndPunchCards();
      bingFunctions.CloseSelenium(15);
      */
    }
    catch (Exception ex)
    {
      AnsiConsole.WriteException(ex);
      bingFunctions.CloseSelenium(15);
    }

  }
}
