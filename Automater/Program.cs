using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Chrome;
using Spectre.Console;

class Program
{
  static void Main(string[] args)
  {
    /*
		var edgeOptions = new EdgeOptions();
		edgeOptions.AddExcludedArgument("enable-logging");
		var driver = new EdgeDriver(edgeOptions);
		var bingFunctions = new BingFunctions(driver);
		*/

    var options = new ChromeOptions();
		options.AddExcludedArgument("enable-logging");
    var driver = new ChromeDriver(options);
    var bingFunctions = new BingFunctions(driver);

    try
    {
      if (bingFunctions.RewardsLogin())
      {
        bingFunctions.AutomatedSearches();
        bingFunctions.ActivateRewardCards();
        bingFunctions.ActivateQuestAndPunchCards();
        bingFunctions.CloseSelenium(15);
      }
    }
    catch (Exception ex)
    {
      bingFunctions.CloseSelenium(15);
      AnsiConsole.WriteException(ex);
    }

    /*
		driver.Navigate().GoToUrl("https://rewards.bing.com/");
		var punchcardText = driver.FindElement(By.CssSelector("h1[mee-heading='heading']"));
		// wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("")));
		punchcardText.Click();
		*/

  }
}

