using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Spectre.Console;

namespace Automater
{
    public enum BingFunctionType
    {
        RewardCard,
        PunchCard
    }

    public static class BingHelperFunctions
    {
        public static void OpenSetOfElements(IWebElement element, WebDriverWait wait, BingFunctionType type)
        {
            wait.Until(driver =>
            {
                var scriptResult = ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState");
                if (scriptResult.Equals("complete"))
                {
                    if (type == BingFunctionType.RewardCard)
                    {
                        string elementText = element.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None)[1];
                        driver.SwitchTo().Window(driver.WindowHandles.Last()).Close();
                        driver.SwitchTo().Window(driver.WindowHandles.First());
                        AnsiConsole.MarkupLine($"ELEMENT CARD: {elementText} [green]Complete[/]");
                        return true;
                    }
                    else
                    {
                        // I dont think I can get the element text like in the conditional above
                        // need a different way to get the text 
                        // also need to find quiz questions within the punch cards
                        element.Click();
                        driver.SwitchTo().Window(driver.WindowHandles.Last()).Close();
                        driver.SwitchTo().Window(driver.WindowHandles[1]);
                        return true;
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine($"{element.Text} [red]Failed[/]");
                    return false;
                }
            });
        }
    }
}
