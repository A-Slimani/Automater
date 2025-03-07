﻿using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Spectre.Console;

namespace Automater
{
  public enum BingCardType
  {
    RewardCard,
    PunchCard
  }

  public static class BingHelperFunctions
  {
    public static void OpenSetOfElements(IWebElement element, WebDriverWait wait, BingCardType type)
    {
      wait.Until(driver =>
      {
        var scriptResult = ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState");
        if (scriptResult.Equals("complete"))
        {
          if (type == BingCardType.RewardCard)
          {
            string elementText = element.Text.Split(new string[] { "\n" }, StringSplitOptions.None)[1];
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

    public static void AnswerQuestions(IWebDriver driver)
    {
      // Phase 1: Click on the Start Quiz button if it exists
      var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
      try
      {
        var startQuizElement = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("rqStartQuiz")));
        driver.ExecuteJavaScript("arguments[0].click()", startQuizElement);
      }
      catch
      {
        AnsiConsole.MarkupLine("[red]Start Quiz button not found continuing the quiz... [/]");
      }

      // Phase 2: Check what class is being used 
      IList<IWebElement> answerElements;
      string[] answerElementsClasses = { "bt_cardText", "rqOption", "btOptionCard" };
      string currentCSSTag = answerElementsClasses[0];
      answerElements = driver.FindElements(By.ClassName(answerElementsClasses[0]));
      int count = 1; // to satisfy the while loop expression 
      while (answerElements.Count == 0)
      {
        answerElements = driver.FindElements(By.ClassName(answerElementsClasses[count]));
        currentCSSTag = answerElementsClasses[count];
        count++;
      }

      if (answerElements.Count != 1)
      {
        // Phase 3: Click on all answers
        try
        {
          int totalPoints = int.Parse(driver.FindElement(By.ClassName("rqMCredits")).Text) / 10;

          if (totalPoints == 0)
          {
            IList<string> thisOrThatElement = driver.FindElement(By.ClassName("bt_Quefooter")).Text.Split(' ').ToList();
            totalPoints = int.Parse(thisOrThatElement[2]);
          }

          for (int i = 0; i < totalPoints; i++)
          {
            for (int j = 0; j < answerElements.Count; j++)
            {
              answerElements = driver.FindElements(By.ClassName(currentCSSTag));
              driver.ExecuteJavaScript("arguments[0].click()", answerElements[j]);
            }
          }
        }
        catch
        {
          AnsiConsole.MarkupLine("[yellow]Quiz Complete...[/]");
        }
      }
      else
      {
        AnsiConsole.MarkupLine("[yellow]Quiz Complete...[/]");
      }
    }
  }
}
