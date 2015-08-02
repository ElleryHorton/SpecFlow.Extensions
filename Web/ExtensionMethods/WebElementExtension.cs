using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SpecFlow.Extensions.Web.ExtensionMethods
{
    public static class WebElementExtension
    {
        public static void Type(this IWebElement e, string text)
        {
            e.Click();
            e.Clear();
            e.SendKeys(text);
        }

        public static string Value(this IWebElement e)
        {
            return e.GetAttribute("value") ?? string.Empty;
        }

        public static string Text(this SelectElement e)
        {
            return e.SelectedOption.Text;
        }
    }
}
