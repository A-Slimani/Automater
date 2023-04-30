using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using Automater;

class Program
{
    static void Main(string[] args)
    {
        IWebDriver driver = new EdgeDriver();
        driver.Navigate().GoToUrl("https://www.bing.com/");
        ICollection<IWebElement> elements = driver.FindElements(By.XPath("//*[@id]"));

        IEnumerable<IWebElement> clickableElements = SeleniumFunctions.FindAllClickableElements(elements);

        foreach (IWebElement element in clickableElements)
        {
            if (element.Text.Equals("Rewards"))
            {
                Console.WriteLine("Rewards Found");
            }
        }

        driver.Quit();
    }
}