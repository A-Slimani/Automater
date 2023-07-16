using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Chrome;
using Spectre.Console;
using OpenQA.Selenium;

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
      if (bingFunctions.RewardsLogin())
      {
        bingFunctions.AutomatedSearches();
        bingFunctions.ActivateRewardCards();
        bingFunctions.ActivateQuestAndPunchCards();
      }
      bingFunctions.CloseSelenium(15);
    }
    catch (Exception ex)
    {
      bingFunctions.CloseSelenium(15);
      AnsiConsole.WriteException(ex);
    }

    /*
		driver.Navigate().GoToUrl("https://www.bing.com/search?q=test&form=QBLH&sp=-1&lq=0&pq=tes&sc=10-3&qs=n&sk=&cvid=76F1B249122840479379B9E2F249D038&ghsh=0&ghacc=0&ghpl=");
		var punchcardText = driver.FindElement(By.Id("id_a"));
    Console.WriteLine(punchcardText.GetAttribute("value"));
    */
    
  }
}

