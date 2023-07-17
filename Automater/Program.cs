using OpenQA.Selenium.Edge;
using Spectre.Console;
using Automater;
using Serilog;
using System;

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

    // DESKTOP
    var edgeDesktopOptions = new EdgeOptions();
    edgeDesktopOptions.AddExcludedArgument("enable-logging");
    var desktopDriver = new EdgeDriver(edgeDesktopOptions);
    var bingDesktopFunctions = new BingFunctions(desktopDriver, Log.Logger);

    try
    {
      if (bingDesktopFunctions.RewardsLogin())
      {
        bingDesktopFunctions.AutomatedSearches(ClientType.Desktop);
        bingDesktopFunctions.ActivateRewardCards();
        bingDesktopFunctions.ActivateQuestAndPunchCards();
      }
      bingDesktopFunctions.CloseSelenium(1);
    }
    catch (Exception ex)
    {
      bingDesktopFunctions.CloseSelenium(1);
      Log.Error(ex.ToString());
    }

    // MOBILE
    var edgeMobileOptions = new EdgeOptions();
    edgeMobileOptions.AddExcludedArgument("enable-logging");
    edgeMobileOptions.AddArgument(
      "--user-agent=Mozilla/5.0 (Linux; Android 11; SM-G998B Build/RP1A.200720.012; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/91.0.4472.120 Mobile Safari/537.36"
    );
    var mobileDriver = new EdgeDriver(edgeMobileOptions);
    var bingMobileFunctions = new BingFunctions(mobileDriver, Log.Logger);

    try
    {
      if (bingMobileFunctions.RewardsLogin())
      {
        bingMobileFunctions.AutomatedSearches(ClientType.Mobile);
      }
      bingMobileFunctions.CloseSelenium(15);
    }
    catch (Exception ex)
    {
      bingMobileFunctions.CloseSelenium(15);
      Log.Error(ex.ToString());
    }


    /*
		driver.Navigate().GoToUrl("https://www.bing.com/search?q=test&form=QBLH&sp=-1&lq=0&pq=tes&sc=10-3&qs=n&sk=&cvid=76F1B249122840479379B9E2F249D038&ghsh=0&ghacc=0&ghpl=");
		var punchcardText = driver.FindElement(By.Id("id_a"));
    Console.WriteLine(punchcardText.GetAttribute("value"));
    */

  }
}

