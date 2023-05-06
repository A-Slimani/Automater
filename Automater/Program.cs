using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace Automater;
internal static class Program
{
    private static void Main(string[] args)
    {
        var driver = SeleniumFunctions.InitialiseEdgeDriver();

        SeleniumFunctions.AutomatedSearches(driver);
        SeleniumFunctions.ActivateRewards(driver);
        SeleniumFunctions.CloseSelenium(30, driver);
    }
}