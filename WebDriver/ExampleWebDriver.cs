using OpenQA.Selenium;
using SpecFlow.Extensions.WebDriver.PortalDriver;

namespace SpecFlow.Extensions.WebDriver
{
    public class ExampleWebDriver : PortalWebDriver, IPortalDriver, IWebDriver, ISearchContext
    {
        public ExampleWebDriver(IWebDriver driver) : base(driver) {
            // initialize your web driver for a specific web portal
            // get credentials and urls for example
        }

        public bool LoggedIn() {
            // return the logged in state
            return false;
        }
        public void Login() {
            // use some credential to log in
            // set LoggedIn = true if successful
        }
        public void Logout() {
            // log out of your web portal by clicking a link or button, etc.
        }
        public void NavigateHome()
        {
            // navigate to the home url of your web portal
            NavigateTo(string.Empty);
        }
        public void NavigateTo(string url) {
            // navigate to a url on your web portal
        }
    }
}
