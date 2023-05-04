using Automater;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Interactions;
using System.Text.RegularExpressions;


class Program
{
    static void Main(string[] args)
    {
        EdgeOptions options = new EdgeOptions();
        options.AddExcludedArgument("enable-logging");
        IWebDriver driver = new EdgeDriver(options);

        driver.Navigate().GoToUrl("https://rewards.bing.com/?signin=1");

        ICollection<IWebElement> cardElements =
            driver.FindElements(By.XPath("//mee-rewards-daily-set-item-content | //mee-rewards-more-activities-card-item"));

        // Dont think I will need this anymore
        // ICollection<IWebElement> pointElements = driver.FindElements(By.XPath("//*[@class='mee-icon mee-icon-AddMedium']"));

        SeleniumFunctions.downloadAllElements(cardElements, "card-elements", e => e.GetAttribute("innerHTML"), "html");

        ICollection<IWebElement> filteredElements =
            SeleniumFunctions.FilterElements(cardElements, new Regex(@"mee-icon-AddMedium")).ToList();

        SeleniumFunctions.downloadAllElements(filteredElements, "filtered_elements", e => e.Text, "txt");

        Actions action = new Actions(driver);
        foreach (IWebElement element in filteredElements)
        {
            action.KeyDown(Keys.Control).Click(element).KeyUp(Keys.Control).Build().Perform();
        }

        // END CLOSING E
        // 1000 = 1 sec
        // Console.WriteLine("program will end in 30 seconds...");
        // Thread.Sleep(1000 * 30);
        // driver.Quit();
    }
}