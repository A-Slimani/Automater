using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
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
            // this wont work on other devices
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

            var filteredElements = FilterElements(cardElements, new Regex(@"mee-icon-AddMedium")).ToList();

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

            var lines = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "word_list.txt"));

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            for (int i = 0; i < 30; i++)
            {
                var randomWord = lines[new Random().Next(lines.Length)];

                Console.WriteLine($"Searching for {randomWord}");

                var searchBar = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("sb_form_q")));
                searchBar.Clear();
                searchBar.SendKeys(randomWord);
                searchBar.Submit();

                wait.Until(ExpectedConditions.TitleContains(randomWord));

                // Modify this to check how many points you have left
                Console.WriteLine($"Found {randomWord}");

                driver.Navigate().Back();
            }
        }

        public static void CloseSelenium(int seconds, IWebDriver driver)
        {
            Console.WriteLine($"Rewards Automater complete. Program will end in {seconds} seconds...");
            Thread.Sleep(1000 * seconds);
            driver.Quit();
        }
    }
}
