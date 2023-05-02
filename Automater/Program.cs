using Automater;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

class Program
{
    static void Main(string[] args)
    {
        IWebDriver driver = new EdgeDriver();

        driver.Navigate().GoToUrl("https://rewards.bing.com/?signin=1");

        ICollection<IWebElement> cardElements = driver.FindElements(By.XPath("//*[@class=\"c-card-content\"]"));

        ICollection<IWebElement> pointElements = driver.FindElements(By.XPath("//*[@aria-label=\"\"]"));

        // Use the PrintAllElements function here 
        SeleniumFunctions.PrintAllElementsText(pointElements, "point-elements"); 

        // driver.Quit();
    }
}