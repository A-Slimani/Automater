﻿using OpenQA.Selenium;
using System.Text.RegularExpressions;
using Spectre.Console;
using Automater.Models;

namespace Automater
{
    public static class BingElements
    {
        public static BingPoints GetPointsEarnedToday(IWebDriver driver)
        {
            driver.Navigate().GoToUrl("https://rewards.bing.com");
            Thread.Sleep(1000);

            var pointsEarnedToday = driver.FindElement(By.XPath("//*[@id=\"dailypointToolTipDiv\"]/p/mee-rewards-counter-animation/span"));
            var totalPointsEarned = driver.FindElement(By.XPath("//*[@id=\"balanceToolTipDiv\"]/p/mee-rewards-counter-animation/span"));

            int today = int.Parse(pointsEarnedToday.Text);
            int total = int.Parse(totalPointsEarned.Text.Replace(",", ""));

            return new BingPoints { Today = today, Total = total };
        }

        public static int GetRemainingSearches(IWebDriver driver, ClientType type, Serilog.ILogger logger)
        {
            driver.Navigate().GoToUrl("https://rewards.bing.com");

            var pointsBreakDownElement = driver.FindElement(By.Id("dailypointColumnCalltoAction"));
            pointsBreakDownElement.Click();

            var searchElements = driver.FindElements(By.XPath("//p[@class=\"pointsDetail c-subheading-3 ng-binding\"]"));

            int currentPoints = type == ClientType.Desktop
              ? int.Parse(searchElements[0].Text.Split('/')[0].Trim())
              : int.Parse(searchElements[1].Text.Split('/')[0].Trim());

            int remainingSearches = type == ClientType.Desktop
              ? (int.Parse(searchElements[0].Text.Split('/')[1]) - currentPoints) / 3
              : (int.Parse(searchElements[1].Text.Split('/')[1]) - currentPoints) / 3;

            if (remainingSearches > 0)
            {
                AnsiConsole.MarkupLine($"[Blue]{remainingSearches}[/] searches remaining...");
            }
            else
            {
                logger.Information("Searches Complete...");
            }

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
