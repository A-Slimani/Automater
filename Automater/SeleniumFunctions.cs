using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Remote;

namespace Automater
{
  internal class SeleniumFunctions
  {
    public static IWebDriver newBrowserInstance()
    {
      EdgeDriverService driverService = EdgeDriverService.CreateDefaultService();
      return new RemoteWebDriver(driverService.ServiceUrl, new EdgeOptions());
    }
    public static IWebDriver currentBrowserInstance()
    {
      EdgeOptions options = new EdgeOptions();
      options.DebuggerAddress = "localhost:9222";

      return new EdgeDriver(options);
    }
    public static IEnumerable<IWebElement> FindAllClickableElements(ICollection<IWebElement> webElements)
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
  }
}
