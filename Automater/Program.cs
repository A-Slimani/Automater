namespace Automater;
internal static class Program
{
    private static void Main(string[] args)
    {
        var driver = SeleniumFunctions.InitialiseEdgeDriver();

        SeleniumFunctions.AutomatedSearches(driver);
        SeleniumFunctions.ActivateRewards(driver);
        SeleniumFunctions.CloseSelenium(15, driver);
    }
}