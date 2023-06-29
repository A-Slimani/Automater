using Automater;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Spectre.Console;

using System.Text.RegularExpressions;

public class BingFunctions
{
    private const string BingUrl = "https://bing.com";
    private const string RewardsUrl = "https://rewards.bing.com";
    private static readonly string WordListFilePath = Path.Combine(Directory.GetCurrentDirectory(), "word_list.txt");

    private readonly IWebDriver _driver;

    public BingFunctions(IWebDriver driver)
    {
        _driver = driver;
    }

    public void AutomatedSearches()
    {
        var lines = File.ReadAllLines(WordListFilePath);
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

        int remainingPoints = BingElements.GetRemainingPoints(_driver);
        while (remainingPoints > 0)
        {
            _driver.Navigate().GoToUrl(BingUrl);

            var randomWord = lines[new Random().Next(lines.Length)];

            AnsiConsole.MarkupLine($"Searching for: [yellow]{randomWord}[/]");

            try
            {
                var searchBar = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("sb_form_q")));
                searchBar.Clear();
                searchBar.SendKeys(randomWord);
                searchBar.Submit();
                wait.Until(ExpectedConditions.TitleContains(randomWord));
                remainingPoints--;

                if (remainingPoints == 0) remainingPoints = BingElements.GetRemainingPoints(_driver);
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
            }
        }
    }

    public void ActivateRewardCards()
    {
        _driver.Navigate().GoToUrl(RewardsUrl);

        var cardElements = _driver.FindElements(By.XPath("//mee-rewards-daily-set-item-content | //mee-rewards-more-activities-card-item"));
        var incompletedCards = BingElements.FilterElements(cardElements, new Regex(@"mee-icon-AddMedium")).ToList();

        var actions = new Actions(_driver);
        foreach (var element in incompletedCards)
        {
            actions.KeyDown(Keys.Control).Click(element).KeyUp(Keys.Control).Build().Perform();

            string cardNameText = element.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None)[1];
            var questionCard = new Regex("(quiz|question|that?)", RegexOptions.IgnoreCase);

            if (questionCard.IsMatch(cardNameText)) ActivateQuestionCard();

            var answersText = _driver.FindElements(By.ClassName("bt_cardText"));
            answersText.Select(answers => answers.Text).ToList().ForEach(Console.WriteLine);

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            BingHelperFunctions.OpenSetOfElements(element, wait, BingFunctionType.RewardCard);
        }
    }

    private void ActivateQuestionCard()
    {
        var handles = _driver.WindowHandles;
        _driver.SwitchTo().Window(handles[1]);

        AnsiConsole.MarkupLine("Starting [blue]Quiz Element[/]");

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

        // Phase 1: Click on the Start Quiz button
        try
        {
            var startQuizElement = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("rqStartQuiz")));
            _driver.ExecuteJavaScript("arguments[0].click()", startQuizElement);

            // Phase 2: Check what class is being used 
            IList<IWebElement> answerElements;
            string[] answerElementsClasses = { "bt_cardText", "rqOption", "btOptionCard" };
            string currentCSSTag = answerElementsClasses[0];
            answerElements = _driver.FindElements(By.ClassName(answerElementsClasses[0]));
            int count = 1;
            while (answerElements.Count == 0)
            {
                answerElements = _driver.FindElements(By.ClassName(answerElementsClasses[count]));
                currentCSSTag = answerElementsClasses[count];
                count++;
            }

            // Phase 3: Click on all answers
            try
            {
                // try and get the actual numbers instead of hardcoding it
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < answerElements.Count; j++)
                    {
                        answerElements = _driver.FindElements(By.ClassName(currentCSSTag));
                        _driver.ExecuteJavaScript("arguments[0].click()", answerElements[j]);
                    }
                }
            }
            catch
            {
                AnsiConsole.MarkupLine("[yellow]Quiz Complete...[/]");
            }
        }
        catch
        {
            AnsiConsole.MarkupLine("[red]No Start quiz button... Continuing with the process[/]");
        }

        _driver.SwitchTo().Window(handles[0]);
    }

    public void ActivateQuestAndPunchCards()
    {
        AnsiConsole.MarkupLine("[yellow]Starting Punch Cards... [/]");
        _driver.Navigate().GoToUrl(RewardsUrl);

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

        var punchCardElement = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("h1[mee-heading='heading']")));
        punchCardElement.Click();
        _driver.SwitchTo().Window(_driver.WindowHandles.Last());

        var checklistElements = _driver.FindElements(By.CssSelector("div.btn-primary.btn.win-color-border-0.card-button-height.pull-left.margin-right-24.padding-left-24.padding-right-24"));
        foreach (var element in checklistElements)
        {
            BingHelperFunctions.OpenSetOfElements(element, wait, BingFunctionType.PunchCard);
        }
    }

    public void CloseSelenium(int seconds)
    {
        // Console.WriteLine($"POINTS EARNED TODAY: {BingElements.GetPointsEarnedToday(_driver)}");
        AnsiConsole.MarkupLine($"[aqua]POINTS EARNED TODAY:[/] [green]{BingElements.GetPointsEarnedToday(_driver)}[/]");
        Console.WriteLine($"Rewards Automater complete. Program will end in {seconds} seconds...");
        Thread.Sleep(1000 * seconds);
        _driver.Quit();
    }
}

