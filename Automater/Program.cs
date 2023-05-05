using Automater;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Interactions;
using System.Text.RegularExpressions;


class Program
{
  static void Main(string[] args)
  {

    string path = @"C:\Users\aboud\Documents\programming\Automater\Automater\wordlist.json";
    string json = File.ReadAllText(path);

    /*
    EdgeOptions options = new EdgeOptions();
    options.AddExcludedArgument("enable-logging");
    IWebDriver driver = new EdgeDriver(options);

    driver.Navigate().GoToUrl("https://rewards.bing.com/?signin=1");

    ICollection<IWebElement> cardElements =
        driver.FindElements(By.XPath("//mee-rewards-daily-set-item-content | //mee-rewards-more-activities-card-item"));

    // Dont think I will need this anymore
    // ICollection<IWebElement> pointElements = driver.FindElements(By.XPath("//*[@class='mee-icon mee-icon-AddMedium']"));

    // SeleniumFunctions.downloadAllElements(cardElements, "card-elements", e => e.GetAttribute("innerHTML"), "html");

    ICollection<IWebElement> filteredElements =
        SeleniumFunctions.FilterElements(cardElements, new Regex(@"mee-icon-AddMedium")).ToList();

    // REACTIVATE THIS 
    // SeleniumFunctions.downloadAllElements(filteredElements, "filtered_elements", e => e.Text, "txt");

    // REACTIVATE THIS
    // SeleniumFunctions.ActivateRewards(filteredElements, driver);

    // SeleniumFunctions.AutomatedSearches(driver);

    // END APPLICATION 
    int seconds = 15;
    Console.WriteLine($"program will end in {seconds} seconds...");
    Thread.Sleep(1000 * seconds);
    driver.Quit();
    */
  }
}