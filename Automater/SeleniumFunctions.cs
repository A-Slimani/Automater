using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;

namespace Automater
{
    internal class SeleniumFunctions
    {
        public static IWebDriver currentBrowserInstance()
        {
            EdgeOptions options = new EdgeOptions();
            options.DebuggerAddress = "localhost:9222";

            return new EdgeDriver(options);
        }

        public static void downloadAllElements(ICollection<IWebElement> elements, string filename, Func<IWebElement, string> getInfo, string fileType)
        {
            string filepath = @$"D:\programming\C#\Automater\Automater\{filename}.{fileType}";

            if (File.Exists(filepath)) File.Delete(filepath);

            string result = $"{elements.Count} results found for {filename}";
            Console.WriteLine(result);

            foreach (IWebElement element in elements)
            {
                if (!string.IsNullOrEmpty(getInfo(element)))
                {
                    File.AppendAllText(filepath, getInfo(element));
                    File.AppendAllText(filepath, "\n");
                }
            }
        }

        // learn about classes - try chatgpt to improve these functions i.e. Polymorphism
        // Write both functions first
        public static IEnumerable<IWebElement> GetAllClickableElements(ICollection<IWebElement> webElements)
        {

            foreach (IWebElement element in webElements)
            {

                bool isClickable = false;

                try
                {
                    isClickable = element.Enabled && element.Displayed;
                }
                catch (StaleElementReferenceException ex)
                {
                    Console.WriteLine($"Error {ex.Message}");
                }

                if (isClickable) yield return element;
            }
        }

        public static IEnumerable<IWebElement> FilterElements(ICollection<IWebElement> webElements, Regex regex)
        {
            foreach (IWebElement element in webElements)
            {
                if (regex.IsMatch(element.GetAttribute("innerHTML")))
                {
                    yield return element;
                }
            }
        }

        public static void ActivateRewards(ICollection<IWebElement> webElements, IWebDriver driver)
        {
            Actions actions = new Actions(driver);
            foreach (IWebElement element in webElements)
            {
                // this only covers rewards that dont have another complete scenario i.e. questions

                actions.KeyDown(Keys.Control).Click(element).KeyUp(Keys.Control).Build().Perform();

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(driver =>
                {
                    if (((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"))
                    {
                        Console.WriteLine("Page loaded.");
                        // check if the page has a question to run another function here 
                        // create a step to close the tab once its loaded here
                        driver.SwitchTo().Window(driver.WindowHandles.Last());
                        driver.Close();
                        driver.SwitchTo().Window(driver.WindowHandles.First());
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Failed to load.");
                        return false;
                    }
                });
            }
        }

        public static void AutomatedSearches(IWebDriver driver)
        {
            driver.Navigate().GoToUrl("https://bing.com");

            IWebElement searchButton = driver.FindElement(By.XPath("//*[@id=\"search_icon\"]"));

            string[] lines = File.ReadAllLines(@"D:\programming\C#\Automater\Automater\MOCK_DATA.txt");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(driver =>
            {
                if (((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"))
                {
                    // placeholder for now
                    // find a better way to handle this look for remaining points and calculate from there
                    for (int i = 0; i < 30; i++)
                    {
                        Random r = new Random();
                        string randomWord = lines[r.Next(lines.Length)];

                        Console.WriteLine("Page loaded.");

                        IWebElement searchBar = driver.FindElement(By.XPath("//*[@id=\"sb_form_q\"]"));
                        searchBar.SendKeys($"{randomWord}");
                        searchBar.SendKeys(Keys.Enter);
                        Console.WriteLine($"Searched for: {randomWord}");

                        // make a check to only go back once the word has been searched
                        driver.Navigate().Back();
                    }
                    return true;
                }
                else
                {
                    Console.WriteLine("Failed to load.");
                    return false;
                }
            });
        }
    }
}
