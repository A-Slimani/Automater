namespace Automater;
internal static class Program
{
    private static void Main(string[] args)
    {
        var driver = BingFunctions.InitialiseEdgeDriver();
        
        BingFunctions.AutomatedSearches(driver, BingElements.GetRemainingPoints(driver));
        BingFunctions.ActivateRewards(driver);
        BingFunctions.CloseSelenium(15, driver);
    }
}