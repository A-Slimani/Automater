using Automater;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Spectre.Console;
using System.Text.Json;

using System.Text.RegularExpressions;

public class BingFunctions
{

  private class Login
  {
    public string? email { get; set; }
    public string? password { get; set; }
  }

  private const string BingUrl = "https://bing.com";
  private const string RewardsUrl = "https://rewards.bing.com";
  private static readonly string WordListFilePath = Path.Combine(Directory.GetCurrentDirectory(), "word_list.txt");

  private readonly IWebDriver _driver;

  public BingFunctions(IWebDriver driver)
  {
    _driver = driver;
  }

  public bool RewardsLogin()
  {
    _driver.Navigate().GoToUrl(RewardsUrl);

    // get login values
    string json = File.ReadAllText("./logins.json");
    var data = JsonSerializer.Deserialize<Login>(json);

    if (data?.email != null && data?.password != null)
    {
      AnsiConsole.MarkupLine($"logging in with email: [green]{data.email}[/]");

      var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
      var emailInput = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[type='email']")));
      emailInput.SendKeys(data?.email);

      var nextButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("idSIButton9")));
      nextButton.Click();

      var passwordInput = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[type='password']")));
      passwordInput.SendKeys(data?.password);

      nextButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("idSIButton9")));
      nextButton.Click();

      var staySignedInButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("idBtn_Back")));
      staySignedInButton.Click();

      return true;
    }
    else
    {
      AnsiConsole.MarkupLine("[red] Missing login details[/]");
      return false;
    }
  }

  public void AutomatedSearches(ClientType type)
  {
    AnsiConsole.MarkupLine($"[aqua]Starting Automatic Searches for {type}...[/]");

    var lines = File.ReadAllLines(WordListFilePath);
    var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

    _driver.Navigate().GoToUrl(BingUrl);

    try
    {
      AnsiConsole.MarkupLine("Login check...");

      if (type == ClientType.Desktop)
      {
        var signInElement = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("id_s")));
        if (signInElement.Text == "Sign in")
        {
          signInElement.Click();
        }
        AnsiConsole.MarkupLine("Search login success!");
      }
      else if(type == ClientType.Mobile)
      {
        var hamburgerElement = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("mHamburger")));
        hamburgerElement.Click();

        var signInElement = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("hb_a")));
        signInElement.Click();
      }
    }
    catch
    {
      AnsiConsole.MarkupLine("Already Logged in...");
    }

    int remainingPoints = BingElements.GetRemainingSearches(_driver, type); 

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

        if (remainingPoints == 0) remainingPoints = BingElements.GetRemainingSearches(_driver, type);
      }
      catch (Exception ex)
      {
        AnsiConsole.WriteException(ex);
      }
    }
  }

  public void ActivateRewardCards()
  {
    AnsiConsole.MarkupLine("[aqua]Starting Reward Cards...[/]");

    _driver.Navigate().GoToUrl(RewardsUrl);

    var cardElements = _driver.FindElements(By.XPath("//mee-rewards-daily-set-item-content | //mee-rewards-more-activities-card-item"));
    var incompletedCards = BingElements.FilterElements(cardElements, new Regex(@"mee-icon-AddMedium")).ToList();

    var actions = new Actions(_driver);
    foreach (var element in incompletedCards)
    {
      actions.KeyDown(Keys.Control).Click(element).KeyUp(Keys.Control).Build().Perform();

      string cardNameText = element.Text.Split(new string[] { "\n" }, StringSplitOptions.None)[1];
      var questionCardRegex = new Regex("(quiz|question|that?)", RegexOptions.IgnoreCase);

      if (questionCardRegex.IsMatch(cardNameText)) ActivateQuestionCard();

      var answersText = _driver.FindElements(By.ClassName("bt_cardText"));
      answersText.Select(answers => answers.Text).ToList().ForEach(Console.WriteLine);

      var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
      BingHelperFunctions.OpenSetOfElements(element, wait, BingCardType.RewardCard);
    }
  }

  private void ActivateQuestionCard()
  {
    var handles = _driver.WindowHandles;
    _driver.SwitchTo().Window(handles[1]);

    AnsiConsole.MarkupLine("Starting [blue]Quiz Element[/]");

    BingHelperFunctions.AnswerQuestions(_driver);

    _driver.SwitchTo().Window(handles[0]);
  }

  public void ActivateQuestAndPunchCards()
  {
    AnsiConsole.MarkupLine("[aqua]Starting Punch Cards... [/]");
    _driver.Navigate().GoToUrl(RewardsUrl);

    var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

    try
    {
      // doesnt seem to work right now, will not open the element at all
      var punchCardElement = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("h1[mee-heading='heading']")));
      punchCardElement.Click();
      _driver.SwitchTo().Window(_driver.WindowHandles.Last());

      var checklistElements = _driver.FindElements(By.CssSelector("div.btn-primary.btn.win-color-border-0.card-button-height.pull-left.margin-right-24.padding-left-24.padding-right-24"));
      foreach (var element in checklistElements)
      {
        var questionRegex = new Regex("(quiz|question|play|that?)", RegexOptions.IgnoreCase);
        if (questionRegex.IsMatch(element.Text))
        {
          BingHelperFunctions.AnswerQuestions(_driver);
        }
        BingHelperFunctions.OpenSetOfElements(element, wait, BingCardType.PunchCard);
      }
    }
    catch
    {
      AnsiConsole.MarkupLine("[red]Punchcard Element not found...[/]");
    }
  }

  public void CloseSelenium(int seconds)
  {
    AnsiConsole.MarkupLine($"[aqua]POINTS EARNED TODAY:[/] [green]{BingElements.GetPointsEarnedToday(_driver)}[/]");
    Console.WriteLine($"Rewards Automater complete. Program will end in {seconds} seconds...");
    Thread.Sleep(1000 * seconds);
    _driver.Quit();
  }
}

