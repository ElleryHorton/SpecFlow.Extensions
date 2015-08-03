using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using SpecFlow.Extensions.Web.CustomBys;
using SpecFlow.Extensions.WebDriver.PortalDriver;

namespace SpecFlow.Extensions.PageObjects
{
    public class LoginExamplePage : Page
    {
        public LoginExamplePage(IPortalDriver driver) : base(driver) { }

        [FindsBy(How = How.Custom, CustomFinderType = typeof(ById))]
        public IWebElement Username;

        [FindsBy(How = How.Custom, CustomFinderType = typeof(ById))]
        public IWebElement Password;

        [FindsBy(How = How.Custom, CustomFinderType = typeof(ByTagName.Text), Using = "input Log In")]
        public IWebElement LoginButton;
    }
}
