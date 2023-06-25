using Microsoft.Extensions.Configuration;
using OpenQA.Selenium.Edge;
using Spectre.Console;

class Program
{
    static void Main(string[] args)
    {
        // come back to this later
        /*
        var bingSelection = AnsiConsole.Prompt(
          new MultiSelectionPrompt<string>()
            .Title("Choose what functions to run...")
            .InstructionsText(
              "Press <space> to select. <Enter> to accept"
            )
            .AddChoiceGroup("All", new[] {
              "Automated Searches",
              "Activate Reward Cards",
              "Activate Question And PunchCards"
            })
        );
        */

        var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
        var configuration = builder.Build();

        // var profileDirectory = @"C:\\Users\\aboud\\AppData\\Local\\Microsoft\\Edge\\User Data";

        var edgeOptions = new EdgeOptions();
        edgeOptions.AddExcludedArgument("enable-logging");
        edgeOptions.AddUserProfilePreference("profile.default_content_setting_values.popups", 0);
        edgeOptions.AddArgument("--disable-features=WindowsAccountsConsent");
        edgeOptions.AddArgument("--disable-features=EnableEphemeralFlashPermission");
        edgeOptions.AddArgument("--disable-features=PreloadMediaEngagementData");
        edgeOptions.AddArgument("--profile-directory=Default");
        // edgeOptions.AddArgument($"user-data-dir={profileDirectory}");
        // edgeOptions.AddArgument($"--profile-directory=Default");
        var driver = new EdgeDriver(edgeOptions);
        var bingFunctions = new BingFunctions(driver);

        try
        {
            bingFunctions.AutomatedSearches();
            bingFunctions.ActivateRewardCards();
            bingFunctions.ActivateQuestAndPunchCards();
            bingFunctions.CloseSelenium(15);
        }
        catch (Exception ex)
        {
            bingFunctions.CloseSelenium(15);
            AnsiConsole.WriteException(ex);
        }

    }
}
