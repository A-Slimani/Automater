using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace Automater
{
    internal class SeleniumFunctions
    {
        public static IWebDriver currentBrowserInstance()
        {
            EdgeOptions options = new EdgeOptions();
            options.DebuggerAddress = "localhost:9222";

            return new EdgeDriver(options);
        }

        public static void PrintAllElementsText(ICollection<IWebElement> elements, string filename, Func<IWebElement, string> getInfo)
        {
            string filepath = @$"D:\programming\C#\Automater\Automater\{filename}.txt";

            if (File.Exists(filepath)) File.Delete(filepath);

            string result = $"{elements.Count()} results found for {filename}";

            if (elements.Count() > 0)
            {
                File.AppendAllText(filepath, result);
                File.AppendAllText(filepath, "\nElement Text List:\n==========\n");
                Console.WriteLine(result);
            }
            else
            {
                File.AppendAllText(filepath, result);
                Console.WriteLine(result);
            }

            foreach (IWebElement element in elements)
            {
                if (!string.IsNullOrEmpty(getInfo(element)))
                {
                    File.AppendAllText(filepath, getInfo(element));
                }
            }
        }

        // learn about classes - try chatgpt to improve these functions i.e. Polymorphism
        // Write both functions first
        public static IEnumerable<IWebElement> GetAllClickableElements(ICollection<IWebElement> webElements)
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

        // initial idea: just get points hyperlink however I think I can make it generic
        //      and pass through either a string / tag or something else
        // - create a base function
        /*
        public static IEnumerable<IWebElement> GetAllElementsOfType(ICollection<IWebElement> webElements, string xPath)
        {

        }
        */
    }
}
