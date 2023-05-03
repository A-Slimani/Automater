using Automater;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

class Program
{
    static void Main(string[] args)
    {
        EdgeOptions options = new EdgeOptions();
        options.AddExcludedArgument("enable-logging");
        IWebDriver driver = new EdgeDriver(options);

        driver.Navigate().GoToUrl("https://rewards.bing.com/?signin=1");

        ICollection<IWebElement> cardElements = driver.FindElements(By.XPath("//*[@class=\"c-card-content\"]"));

        ICollection<IWebElement> pointElements = driver.FindElements(By.XPath("//*[@class='mee-icon mee-icon-AddMedium']"));

        SeleniumFunctions.PrintAllElementsText(cardElements, "card-elements", e => e.Text);



        driver.Quit();
    }
}