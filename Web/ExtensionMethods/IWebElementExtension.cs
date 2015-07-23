using OpenQA.Selenium;

namespace SpecFlow.Extensions.Web
{
    public static class IWebElementExtension
    {
        public static string Value(this IWebElement e)
        {
            return e.GetAttribute("value");
        }
    }
}
