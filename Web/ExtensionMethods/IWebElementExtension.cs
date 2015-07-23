using OpenQA.Selenium;

namespace SpecFlow.Extensions.Web
{
    public static class IWebElementExtension
    {
        public static string Value(this IWebElement e)
        {
            return e.GetAttribute("value");
        }

        public static string Text(this SelectElement e)
        {
            return e.SelectedOption.Text;
        }
    }
}
