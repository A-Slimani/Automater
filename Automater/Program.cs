using Automater;

class Program
{
    static void Main(string[] args)
    {
        var driver = BingFunctions.InitialiseEdgeDriver();

        BingFunctions.AutomatedSearches(driver, BingElements.GetRemainingPoints(driver));
        BingFunctions.ActivateRewardCards(driver);
        BingFunctions.CloseSelenium(15, driver);
    }
}