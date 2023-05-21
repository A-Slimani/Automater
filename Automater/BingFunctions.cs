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
	private const string RewardsUrl = "https://rewards.bing.com/?signin=1";
	private static readonly string WordListFilePath = Path.Combine(Directory.GetCurrentDirectory(), "word_list.txt");

	private readonly IWebDriver _driver;

	public BingFunctions(IWebDriver driver)
	{
		_driver = driver;
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
			var questionCard = new Regex("(quiz|question|poll)", RegexOptions.IgnoreCase);
			if (questionCard.IsMatch(cardNameText))
			{
				// ActivateCardQuestionsAndPolls(_driver, _driver.Url);
				Console.WriteLine($"--- QUESTION CARD --- {cardNameText} --- FOUND");
			}

			var answersText = _driver.FindElements(By.ClassName("bt_cardText"));
			answersText.Select(answers => answers.Text).ToList().ForEach(Console.WriteLine);

			var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
			wait.Until(dv =>
			{
				var scriptResult = ((IJavaScriptExecutor)dv).ExecuteScript("return document.readyState");
				if (scriptResult.Equals("complete"))
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

	public void ActivateCardQuestionsAndPolls(string url)
	{
		_driver.Navigate().GoToUrl(url);

		var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
		var startQuizElement = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("rqStartQuiz")));
		_driver.ExecuteJavaScript("arguments[0].click()", startQuizElement);

		wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("btOptions")));

		var answerElements = _driver.FindElements(By.ClassName("bt_cardText"));
		foreach (var element in answerElements)
		{
			if (element.GetCssValue("color") != "rbga(200, 0, 0, 1)")
			{
				_driver.ExecuteJavaScript("arguments[0].click()", element);
			}
		}
	}

	public void AutomatedSearches()
	{
		_driver.Navigate().GoToUrl(BingUrl);

		var lines = File.ReadAllLines(WordListFilePath);
		var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

		int remainingPoints = BingElements.GetRemainingPoints(_driver);
		while (remainingPoints > 0)
		{
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

				//Console.WriteLine($"Search complete. {remainingPoints} searches remaining");
				AnsiConsole.Progress()
					.Start(ctx =>
					{
						//Define tasks
						var searches = ctx.AddTask("Searches complete");
						while (!ctx.IsFinished)
						{
							searches.Increment(1);
						}
					});
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Search failed: {ex}");
			}

			if (remainingPoints == 0) remainingPoints = BingElements.GetRemainingPoints(_driver);
		}
	}

	public void ActivateQuestAndPunchCards()
	{
		_driver.Navigate().GoToUrl(RewardsUrl);

		var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

		var punchCardElement = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("h1[mee-heading='heading']")));
		string url = punchCardElement.GetAttribute("href");
		punchCardElement.Click();
		_driver.SwitchTo().Window(_driver.WindowHandles.Last());
		// _driver.Navigate().GoToUrl(url);
		// _driver.ExecuteJavaScript("arguments[0].click()", punchCardElement);

		// does it carry over different tabs??
		var checklistElements = wait.Until(ExpectedConditions.ElementToBeClickable(By.TagName("b")));
		var test = _driver.FindElements(By.TagName("b"));
		foreach (var x in test)
		{
			AnsiConsole.MarkupLine($"Opening: [yellow]{x.Text}[/]");
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