using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using SpecFlow.Extensions.Web.CustomBys;

namespace SpecFlow.Extensions.PageObjects
{
    public class LoginExamplePage : Page
    {
        [FindsBy(How = How.Custom, CustomFinderType = typeof(ById))]
        public IWebElement Username;

        [FindsBy(How = How.Custom, CustomFinderType = typeof(ById))]
        public IWebElement Password;

        [FindsBy(How = How.Custom, CustomFinderType = typeof(ByTagName.Text), Using = "input Log In")]
        public IWebElement LoginButton;
    }
}