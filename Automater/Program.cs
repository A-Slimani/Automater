using Automater;

class Program
{
    static void Main(string[] args)
    {
        var driver = BingFunctions.InitialiseEdgeDriver();

        BingFunctions.AutomatedSearches(driver);
        BingFunctions.ActivateRewardCards(driver);
        //BingFunctions.ActivateCardQuestionsAndPolls(driver, "https://www.bing.com/search?q=Star%20Wars%20movies&rnoreward=1&mkt=EN-AU&FORM=ML12JG&skipopalnative=true&rqpiodemo=1&filters=BTEPOKey:%22REWARDSQUIZ_ENAU_ThursdayBonus_20230504%22%20BTROID:%22Gamification_DailySet_ENAU_20230504_Child2%22%20BTROEC:%220%22%20BTROMC:%2230%22");
        BingFunctions.CloseSelenium(15, driver);
    }
}