using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Automater
{
  public static class BingElements 
  {
    public static int GetRemainingPoints(IWebDriver driver)
    {

      driver.Navigate().GoToUrl("https://rewards.bing.com/?signin=1");

      var pointsBreakDownElement = driver.FindElement(By.Id("dailypointColumnCalltoAction"));
      pointsBreakDownElement.Click();

      var pcSearchElement = driver.FindElement(By.XPath("//p[@class=\"pointsDetail c-subheading-3 ng-binding\"]"));

      int currentPoints = int.Parse(pcSearchElement.Text.Split('/')[0].Trim()); //  just gets the first number

      int remainingSearches = (90 - currentPoints) / 3;

      Console.WriteLine($"Remaining Searches: {remainingSearches}");

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

    public static void ActivateCardQuestionsAndPolls(IWebDriver driver, ICollection<IWebElement>? webElements) 
    {
      // find the quiz element on the page  
      // have some sort of flow chart to click on it

      // list of potential buttons
      // id = rqStartQuiz
      //  -> class btOptions
      //  -> 
      // class = btOptions2 bt_pollOptions

      driver.Navigate().GoToUrl("https://www.bing.com/search?q=Star%20Wars%20movies&rnoreward=1&mkt=EN-AU&FORM=ML12JG&skipopalnative=true&rqpiodemo=1&filters=BTEPOKey:%22REWARDSQUIZ_ENAU_ThursdayBonus_20230504%22%20BTROID:%22Gamification_DailySet_ENAU_20230504_Child2%22%20BTROEC:%220%22%20BTROMC:%2230%22");

      var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

      var startQuizElement = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("rqStartQuiz")));
      // hack since the standard click doesnt work. For some reason there another element ontop of it 
      // preventing it from being clicked
      driver.ExecuteJavaScript("arguments[0].click()", startQuizElement);
      
      wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("btOptions")));
      var answerElements = driver.FindElements(By.ClassName("bt_cardText"));

      Console.WriteLine(answerElements.Count);
      answerElements.Select(ans => ans.Text).ToList().ForEach(Console.WriteLine);
    }
  }
}
