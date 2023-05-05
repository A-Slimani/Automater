using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;

namespace Automater
{
  internal class SeleniumFunctions
  {
    public static IWebDriver currentBrowserInstance()
    {
      EdgeOptions options = new EdgeOptions();
      options.DebuggerAddress = "localhost:9222";

      return new EdgeDriver(options);
    }

    public static void downloadAllElements(ICollection<IWebElement> elements, string filename, Func<IWebElement, string> getInfo, string fileType)
    {
      string filepath = @$"D:\programming\C#\Automater\Automater\{filename}.{fileType}";

      if (File.Exists(filepath)) File.Delete(filepath);

      string result = $"{elements.Count} results found for {filename}";
      Console.WriteLine(result);

      foreach (IWebElement element in elements)
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

      foreach (IWebElement element in webElements)
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

    public static IEnumerable<IWebElement> FilterElements(ICollection<IWebElement> webElements, Regex regex)
    {
      foreach (IWebElement element in webElements)
      {
        if (regex.IsMatch(element.GetAttribute("innerHTML")))
        {
          yield return element;
        }
      }
    }

    public static void ActivateRewards(ICollection<IWebElement> webElements, IWebDriver driver)
    {
      Actions actions = new Actions(driver);
      foreach (IWebElement element in webElements)
      {
        // this only covers rewards that dont have another complete scenario

        // make this only get one tab at a time then closes it
        actions.KeyDown(Keys.Control).Click(element).KeyUp(Keys.Control).Build().Perform();

        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        wait.Until(driver =>
        {
          if (((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"))
          {
            Console.WriteLine("Page loaded.");
            // create a step to close the tab once its loaded here
            return true;
          }
          else
          {
            Console.WriteLine("Failed to load.");
            return false;
          }
        });
        // have another check once the page is open below to complete pages with questions
        // if (has some question element ....) 
      }
    }

    public static void AutomatedSearches(IWebDriver driver)
    {
      driver.Navigate().GoToUrl("https://bing.com");

      IWebElement searchBar = driver.FindElement(By.XPath("//*[@id=\"sb_form_q\"]"));
      searchBar.SendKeys($"{}")
      
      Actions actions = new Actions(driver); 

      
      // get a random word and search it



    }
  }
}
