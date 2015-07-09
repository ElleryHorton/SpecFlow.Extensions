using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SpecFlow.WebExtension
{
    abstract public class WrapWebDriver : IWrapWebDriver
    {
        private const int MAX_RETRIES = 3;
        public WrapWebDriver(IWebDriver driver)
        {
            Driver = driver;
        }
        public IWebDriver Driver { get; set; }

        protected bool _loggedIn;
        public bool LoggedIn
        {
            get
            {
                return _loggedIn;
            }
        }

        abstract public void Login();

        abstract public void Logout();

        abstract  public void NavigateTo(string url);

        public void WaitForPageLoad()
        {
            Driver.WaitForPageLoad();
        }

        public IWebElement Find(ByEx id)
        {
            return Driver.Find(id);
        }

        public IEnumerable<IWebElement> FindAll(ByEx id)
        {
            return Driver.FindAll(id);
        }

        public void Click(ByEx id)
        {
            TryAgain(() =>
            {
                Driver.Find(id).Click();
                return true;
            });
        }
        public void SendKeys(ByEx id, string text)
        {
            TryAgain(() =>
            {
                Driver.Find(id).SendKeys(text);
                return true;
            });
        }
        public void Clear(ByEx id)
        {
            TryAgain(() =>
            {
                Driver.Find(id).Clear();
                return true;
            });
        }
        public bool Displayed(ByEx id)
        {
            return TryAgain(() =>
        {
                return Driver.Find(id).Displayed;
            });
        }

        public bool TryAgain(Func<bool> func)
        {
            int tryCount = 0;
            bool success = false;
            while (tryCount < MAX_RETRIES && !success)
            {
                try
                {
                    success = func();
                }
                catch (Exception)
                {
                    success = false;
                }
                tryCount++;
                if (!success)
                {
                    Thread.Sleep(2000);
                }
            }
            return success;
        }
    }
}