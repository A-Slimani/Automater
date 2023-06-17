using OpenQA.Selenium;
using System.Text.RegularExpressions;

namespace Automater
{
    public static class BingElements
    {
        public static int GetPointsEarnedToday(IWebDriver driver)
        {
            driver.Navigate().GoToUrl("https://rewards.bing.com");
            Thread.Sleep(1000);
            var pointsEarnedToday = driver.FindElement(By.XPath("//*[@id=\"dailypointToolTipDiv\"]/p/mee-rewards-counter-animation/span"));
            return int.Parse(pointsEarnedToday.Text);
        }

        public static int GetRemainingPoints(IWebDriver driver)
        {

            driver.Navigate().GoToUrl("https://rewards.bing.com");

            var pointsBreakDownElement = driver.FindElement(By.Id("dailypointColumnCalltoAction"));
            pointsBreakDownElement.Click();

            var pcSearchElement = driver.FindElement(By.XPath("//p[@class=\"pointsDetail c-subheading-3 ng-binding\"]"));

            int currentPoints = int.Parse(pcSearchElement.Text.Split('/')[0].Trim()); //  just gets the first number

            int remainingSearches = (90 - currentPoints) / 3;

            return remainingSearches;
        }

        public static IEnumerable<IWebElement> FilterElements(ICollection<IWebElement> webElements, Regex regex)
        {
            return webElements.Where(element => regex.IsMatch(element.GetAttribute("innerHTML")));
        }

        // for debugging purposes -> put at the bottom later
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
    }
}
