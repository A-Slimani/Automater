using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Interactions;
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
        string cardNameText = element.Text.Split(new string[] {"\r\n"}, StringSplitOptions.None)[1];

        // add a regex check if `cardNameText` equals to either quiz, question or poll
        Regex questionCard = new Regex("(quiz|question|poll)", RegexOptions.IgnoreCase);

        if (questionCard.IsMatch(cardNameText))
        {
          // run ActivateCardQuestionsAndPolls() here
           
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

    public static void AutomatedSearches(IWebDriver driver, int remainingPoints)
    {
      driver.Navigate().GoToUrl("https://bing.com");

      var lines = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "word_list.txt"));

      var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

      for (int i = 0; i < remainingPoints; i++)
      {
        var randomWord = lines[new Random().Next(lines.Length)];

        Console.WriteLine($"Searching for {randomWord}");

        var searchBar = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("sb_form_q")));
        searchBar.Clear();
        searchBar.SendKeys(randomWord);
        searchBar.Submit();

        // Added this sleep so that its less likely to bug out
        Thread.Sleep(1000);
        wait.Until(ExpectedConditions.TitleContains(randomWord));

        Console.WriteLine($"Search complete. {remainingPoints - (i + 1)} searches remaining");

        driver.Navigate().GoToUrl("https://bing.com");
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
