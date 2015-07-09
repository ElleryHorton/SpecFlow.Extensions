using OpenQA.Selenium;

namespace SpecFlow.WebExtension.WebDriver
{
    public class ExampleWebDriver : WrapWebDriver, IWrapWebDriver
    {
        public ExampleWebDriver(IWebDriver driver) : base(driver) {
            // initialize your web driver for a specific web portal
            // get credentials and urls for example
        }
        public override void Login() {
            // use some credential to log in
            // set LoggedIn = true if successful
        }
        public override void Logout() {
            // log out of your web portal by clicking a link or button, etc.
        }
        public override void NavigateTo(string url) {
            // navigate to a url on your web portal
        }
    }
}
