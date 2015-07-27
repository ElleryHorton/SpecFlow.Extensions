using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SpecFlow.Extensions.Web
{
    public static class IWebElementExtension
    {
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
