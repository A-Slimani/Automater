using OpenQA.Selenium;

namespace Automater
{
    internal class SeleniumFunctions
    {
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
