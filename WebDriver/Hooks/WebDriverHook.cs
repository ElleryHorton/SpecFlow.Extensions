using TechTalk.SpecFlow;

namespace SpecFlow.Extensions.WebDriver.Hooks
{
    [Binding]
    public class WebDriverHook
    {
        static IWrapWebDriver _portalDriver;
        bool _closeBrowsersAfterScenario = true;
        bool _logoutAfterScenario = true;

        public WebDriverHook(WebContext webContext)
        {
            //webContext.PortalDriver = DriverFactory.GetDriver();
            _portalDriver = webContext.PortalDriver;
        }

        private void SetDefaultBrowserSize()
        {
            _portalDriver.Driver.Manage().Window.Size = new System.Drawing.Size(1024, 768);
        }

        [BeforeScenario("web")]
        public void BeforeScenarioWeb()
        {
            SetDefaultBrowserSize();
        }

        [BeforeScenario("web_max")]
        public void BeforeScenarioWebMax()
        {
            _portalDriver.Driver.Manage().Window.Maximize();
        }

        [BeforeScenario("web_persist")] // enable reuse of browsers
        public void BeforeScenarioWebPersist()
        {
            _closeBrowsersAfterScenario = false;
            SetDefaultBrowserSize();
        }

        [BeforeScenario("web_persistLogin")] // enable reuse of logged in session
        public void BeforeScenarioWebPersistLogin()
        {
            _closeBrowsersAfterScenario = false;
            _logoutAfterScenario = false;
            SetDefaultBrowserSize();
        }

        [BeforeScenario("web_small")]
        public void BeforeScenarioWebSmall()
        {
            _portalDriver.Driver.Manage().Window.Size = new System.Drawing.Size(500, 500);
        }

        // TODO list all scenario web hooks here
        [AfterScenario("web", "web_max", "web_persist", "web_persistLogin", "web_small")]
        public void AfterScenarioWeb()
        {
            if (_logoutAfterScenario)
            {
                //DriverFactory.LogoutBrowsers();
            }
			else
			{
				//TODO go back to a known starting url
			}

            if (_closeBrowsersAfterScenario)
            {
                //DriverFactory.CloseBrowsers();
            }
        }

        // TODO list all scenario web hooks that don't close browsers after scenarios here
        [AfterFeature("web_persist", "web_persistLogin")]
        public static void AfterFeatureWeb()
        {
            //DriverFactory.LogoutBrowsers();
            //DriverFactory.CloseBrowsers();
        }
    }
}