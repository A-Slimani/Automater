using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;

namespace Automater
{
    internal static class SeleniumFunctions
    {
        public static IWebDriver CurrentBrowserInstance()
        {
            var options = new EdgeOptions
            {
                DebuggerAddress = "localhost:9222"
            };
            return new EdgeDriver(options);
        }

        public static IWebDriver InitialiseEdgeDriver()
        {
            EdgeOptions options = new();
            options.AddExcludedArgument("enable-logging");
            return new EdgeDriver(options);
        }

        public static void DownloadAllElementsToFile(ICollection<IWebElement> elements, string filename, Func<IWebElement, string> getInfo, string fileType)
        {
            string filepath = @$"D:\programming\C#\Automater\Automater\{filename}.{fileType}";

            if (File.Exists(filepath)) File.Delete(filepath);

            string result = $"{elements.Count} results found for {filename}";
            Console.WriteLine(result);

            foreach (var element in elements)
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

            foreach (var element in webElements)
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

        private static IEnumerable<IWebElement> FilterElements(ICollection<IWebElement> webElements, Regex regex)
        {
            return webElements.Where(element => regex.IsMatch(element.GetAttribute("innerHTML")));
        }

        public static void ActivateRewards(IWebDriver driver)
        {
            driver.Navigate().GoToUrl("https://rewards.bing.com/?signin=1");

            var cardElements = driver.FindElements(By.XPath("//mee-rewards-daily-set-item-content | //mee-rewards-more-activities-card-item"));

            var filteredElements = SeleniumFunctions.FilterElements(cardElements, new Regex(@"mee-icon-AddMedium")).ToList();

            var actions = new Actions(driver);
            foreach (var element in filteredElements)
            {
                actions.KeyDown(Keys.Control).Click(element).KeyUp(Keys.Control).Build().Perform();

                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(dv =>
                {
                    if (((IJavaScriptExecutor)dv).ExecuteScript("return document.readyState").Equals("complete"))
                    {
                        Console.WriteLine("Page loaded.");
                        dv.SwitchTo().Window(dv.WindowHandles.Last()).Close();
                        dv.SwitchTo().Window(dv.WindowHandles.First());
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
            wait.Until(dv =>
            {
                if (((IJavaScriptExecutor)dv).ExecuteScript("return document.readyState").Equals("complete"))
                {
                    var r = new Random();

                    // instead of a set number try to get the amount from remaining points
                    for (int i = 0; i < 2; i++)
                    {
                        string randomWord = lines[r.Next(lines.Length)];

                        Console.WriteLine("Page loaded.");

                        Thread.Sleep(500);
                        IWebElement searchBar = dv.FindElement(By.XPath("//*[@id=\"sb_form_q\"]"));
                        searchBar.SendKeys($"{randomWord}");
                        searchBar.SendKeys(Keys.Enter);
                        Console.WriteLine($"{i}: Searched for {randomWord}");

                        wait.Until(d => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
                        dv.Navigate().Back();
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

        public static void CloseSelenium(int seconds, IWebDriver driver)
        {
            Console.WriteLine($"program will end in {seconds} seconds...");
            Thread.Sleep(1000 * seconds);
            driver.Quit();
        }
    }
}
