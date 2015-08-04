using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using SpecFlow.Extensions.Web;
using SpecFlow.Extensions.Web.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;

namespace SpecFlow.Extensions.WebDriver.PortalDriver
{
    public abstract class PortalWebDriver : WrapWebDriver, IPortalInteract, IWebDriver, IWrapsDriver
    {
        private readonly int _maxFindAttempts;
        private readonly int _maxTimeoutMilliseconds;

        public PortalWebDriver(IWebDriver driver, int maxFindAttempts = 3, int maxTimeoutMilliseconds = 2000) : base(driver)
        {
            _maxFindAttempts = maxFindAttempts;
            _maxTimeoutMilliseconds = maxTimeoutMilliseconds;
        }
        
        public void WaitForPageLoad()
        {
            WrappedDriver.WaitForPageLoad();
        }

        public bool ClickChangesUrl(ByEx byEx)
        {
            string oldUrl = WrappedDriver.Url;
            Click(byEx);
            WrappedDriver.WaitForUrlToChange(oldUrl, _maxTimeoutMilliseconds);
            return oldUrl != WrappedDriver.Url;
        }

        public bool ClickChangesUrl(IWebElement element)
        {
            string oldUrl = WrappedDriver.Url;
            if (element.Displayed)
                Click(element);
            else
                ClickInvisible(element);
            WrappedDriver.WaitForUrlToChange(oldUrl, _maxTimeoutMilliseconds);
            return oldUrl != WrappedDriver.Url;
        }

        public IWebElement Find(ByEx byEx)
        {
            return WrappedDriver.FindElement(byEx);
        }

        public IWebElement Find(IWebElement element) // support backwards compatibility
        {
            return element; // already found by PageObject pattern
        }

        public IEnumerable<IWebElement> FindAll(ByEx byEx)
        {
            return WrappedDriver.FindElements(byEx);
        }

        public SelectElement FindSelect(ByEx byEx)
        {
            return WrappedDriver.FindSelect(byEx);
        }

        public SelectElement FindSelect(IWebElement element) // support backwards compatibility
        {
            return new SelectElement(element);
        }

        public TableElement FindTable(ByEx byEx)
        {
            return WrappedDriver.FindTable(byEx);
        }

        public TableElement FindTable(IWebElement element) // support backwards compatibility
        {
            return new TableElement(element);
        }

        public void Clear(ByEx byEx)
        {
            TryAgain(() =>
            {
                Find(byEx).Clear();
                return true;
            });
        }

        public void Clear(IWebElement element)
        {
            TryAgain(() =>
            {
                element.Clear();
                return true;
            });
        }

        public void Click(ByEx byEx)
        {
            TryAgain(() =>
            {
                Find(byEx).Click();
                return true;
            });
            WaitForPageLoad();

        }

        public void Click(IWebElement element)
        {
            TryAgain(() =>
            {
                element.Click();
                return true;
            });
            WaitForPageLoad();

        }

        public void ClickInvisible(ByEx byEx)
        {
            TryAgain(() =>
            {
                ((IJavaScriptExecutor)WrappedDriver).ExecuteScript("arguments[0].Click()", Find(byEx));
                return true;
            });
        }

        public void ClickInvisible(IWebElement element)
        {
            TryAgain(() =>
            {
                ((IJavaScriptExecutor)WrappedDriver).ExecuteScript("arguments[0].Click()", element);
                return true;
            });
        }

        public bool Displayed(ByEx byEx)
        {
            return TryAgain(() =>
            {
                return Find(byEx).Displayed;
            });
        }

        public bool Displayed(IWebElement element)
        {
            return TryAgain(() =>
            {
                return element.Displayed;
            });
        }

        public bool Exists(ByEx byEx)
        {
            return WrappedDriver.HasChild(byEx);
        }

        public bool Exists(Func<IWebElement> pageObjectElement)
        {
            bool exists = false;
            try
            {
                var element = pageObjectElement();
                exists = true;
            }
            catch
            {
                exists = false;
            }
            return exists;
        }

        public void Select(ByEx byEx)
        {
            TryAgain(() =>
            {
                Find(byEx).Click();
                WaitForPageLoad();
                return Find(byEx).Selected;
            });
        }

        public void Select(IWebElement element)
        {
            TryAgain(() =>
            {
                element.Click();
                WaitForPageLoad();
                return element.Selected;
            });
        }

        public void SendKeys(ByEx byEx, string text)
        {
            TryAgain(() =>
            {
                Find(byEx).SendKeys(text);
                return true;
            });
        }

        public void SendKeys(IWebElement element, string text)
        {
            TryAgain(() =>
            {
                element.SendKeys(text);
                return true;
            });
        }

        public void Type(ByEx byEx, string text)
        {
            TryAgain(() =>
            {
                Find(byEx).Click();
                Find(byEx).Clear();
                Find(byEx).SendKeys(text);
                return true;
            });
        }

        public void Type(IWebElement element, string text)
        {
            TryAgain(() =>
            {
                element.Click();
                element.Clear();
                element.SendKeys(text);
                return true;
            });
        }

        private bool TryAgain(Func<bool> func)
        {
            int tryCount = 0;
            bool success = false;
            Exception lastException = null;
            while (tryCount < _maxFindAttempts && !success)
            {
                try
                {
                    success = func();
                }
                catch (Exception e)
                {
                    lastException = e;
                    success = false;
                }
                tryCount++;
                if (!success)
                {
                    Thread.Sleep(1000);
                }
            }

            if (!success && lastException != null)
            {
                throw lastException;
            }

            return success;
        }
    }
}