using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Interactions;
using Automater;
using OpenQA.Selenium.Remote;
using WebDriverManager;
using OpenQA.Selenium.Support.UI;

class Program
{
    static void Main(string[] args)
    {
        IWebDriver driver = SeleniumFunctions.newBrowserInstance();
        
        driver.Navigate().GoToUrl("https://www.bing.com/");
        ICollection<IWebElement> elements = driver.FindElements(By.XPath("//*[@id]"));

        IEnumerable<IWebElement> clickableElements = SeleniumFunctions.FindAllClickableElements(elements);

        IWebElement? rewardElement = clickableElements.FirstOrDefault(e => e.Text == "Rewards");

        if(rewardElement != null)
        {
            rewardElement.Click();
            // Add a list of actions with this
            // open a new tab and complete each one
            ICollection<IWebElement> rewardsElements = driver.FindElements(By.XPath("//*[@id=\"modern-flyout\"]"));
        }
        else
        {
            Console.WriteLine("no rewards element found");
        }
 
        // driver.Quit();
    }
}