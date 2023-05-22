using OpenQA.Selenium.Edge;
using Spectre.Console;

class Program
{
  static void Main(string[] args)
  {
    var options = new EdgeOptions();
    options.AddExcludedArgument("enable-logging");
    var driver = new EdgeDriver(options);
    var bingFunctions = new BingFunctions(driver);
    try
    {
      // bingFunctions.AutomatedSearches();
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