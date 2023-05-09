using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Interactions;
using Automater;
using OpenQA.Selenium.Remote;

class Program
{
    static void Main(string[] args)
    {
        EdgeOptions options = new EdgeOptions();
        RemoteWebDriver driver = new RemoteWebDriver(new Uri("https://www.bing.com/"), options);


        /*
        EdgeDriverService currentDriverService = EdgeDriverService.CreateDefaultService();
        IWebDriver driver = new RemoteWebDriver(currentDriverService.ServiceUrl, new EdgeOptions());
        driver.Navigate().GoToUrl("https://www.bing.com/");
        ICollection<IWebElement> elements = driver.FindElements(By.XPath("//*[@id]"));

        IEnumerable<IWebElement> clickableElements = SeleniumFunctions.FindAllClickableElements(elements);

        foreach (IWebElement element in clickableElements)
        {
            if (element.Text.Equals("Rewards"))
            {
                Actions action = new Actions(driver);
                // action.MoveToElement(element).Click();
                element.Click();
                break;
            }
        }
        */

        // driver.Quit();
    }
}