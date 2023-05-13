using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Text.RegularExpressions;

namespace Automater
{
    internal static class BingFunctions
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

        // Create a base class then make it a lil different depending on the card??
        public static void ActivateRewardCards(IWebDriver driver)
        {
            driver.Navigate().GoToUrl("https://rewards.bing.com/?signin=1");

            // maybe add a check here if it ever breaks
            var cardElements = driver.FindElements(By.XPath("//mee-rewards-daily-set-item-content | //mee-rewards-more-activities-card-item"));

            var filteredElements = BingElements.FilterElements(cardElements, new Regex(@"mee-icon-AddMedium")).ToList();

            var actions = new Actions(driver);
            foreach (var element in filteredElements)
            {
                actions.KeyDown(Keys.Control).Click(element).KeyUp(Keys.Control).Build().Perform();

                // filters out other text from the element
                string cardNameText = element.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None)[1];

                // add a regex check if `cardNameText` equals to either quiz, question or poll
                Regex questionCard = new Regex("(quiz|question|poll)", RegexOptions.IgnoreCase);

                if (questionCard.IsMatch(cardNameText))
                {
                    // run ActivateCardQuestionsAndPolls() here
                    Console.WriteLine($"--- QUESTION CARD --- {cardNameText} --- FOUND");
                }

                var answersText = driver.FindElements(By.ClassName("bt_cardText"));
                answersText.Select(answers => answers.Text).ToList().ForEach(Console.WriteLine);

                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(dv =>
                {
                    if (((IJavaScriptExecutor)dv).ExecuteScript("return document.readyState").Equals("complete"))
                    {
                        Console.WriteLine($"Reward card {cardNameText} complete.");
                        dv.SwitchTo().Window(dv.WindowHandles.Last()).Close();
                        dv.SwitchTo().Window(dv.WindowHandles.First());
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"Reward card {cardNameText} failed to load.");
                        return false;
                    }
                });
            }
        }

        public static void ActivateCardQuestionsAndPolls(IWebDriver driver, string url)
        {
            // find the quiz element on the page  
            // have some sort of flow chart to click on it

            // list of potential buttons
            // id = rqStartQuiz
            //  -> class btOptions
            //  -> 
            // class = btOptions2 bt_pollOptions

            // just a test url for now
            driver.Navigate().GoToUrl(url);
            // driver.Navigate().GoToUrl(url);

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            var startQuizElement = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("rqStartQuiz")));

            // hack since the standard click doesnt work. For some reason there another element ontop of it 
            // preventing it from being clicked
            driver.ExecuteJavaScript("arguments[0].click()", startQuizElement);

            wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("btOptions")));
            // make it an if statement check if its already been clicked. by the tick or cross
            // maybe use a find first of that element 
            var answerElements = driver.FindElements(By.ClassName("bt_cardText"));

            foreach (var element in answerElements)
            {
                if (element.GetCssValue("color") != "rbga(200, 0, 0, 1)")
                {
                    driver.ExecuteJavaScript("arguments[0].click()", element);
                }
                // answerElements = wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("bt_cardText")));
            }
        }

        public static void AutomatedSearches(IWebDriver driver)
        {
            driver.Navigate().GoToUrl("https://bing.com");

            var lines = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "word_list.txt"));

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));


            int remainingPoints = BingElements.GetRemainingPoints(driver);
            while (remainingPoints > 0)
            {
                driver.Navigate().GoToUrl("https://bing.com");

                var randomWord = lines[new Random().Next(lines.Length)];

                Console.WriteLine($"Searching for {randomWord}");

                try
                {
                    var searchBar = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("sb_form_q")));
                    searchBar.Clear();
                    searchBar.SendKeys(randomWord);
                    searchBar.Submit();
                    wait.Until(ExpectedConditions.TitleContains(randomWord));
                    remainingPoints--;

                    Console.WriteLine($"Search complete. {remainingPoints} searches remaining");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Search failed: {ex}");
                }

                // make sure to go back and check if all points are caught
                if (remainingPoints == 0) remainingPoints = BingElements.GetRemainingPoints(driver);
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
