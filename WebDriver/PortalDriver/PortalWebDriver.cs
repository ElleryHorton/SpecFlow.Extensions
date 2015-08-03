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

        public PortalWebDriver(IWebDriver driver, int maxFindAttempts = 3) : base(driver)
        {
            _maxFindAttempts = maxFindAttempts;
        }
        
        public void WaitForPageLoad()
        {
            WrappedDriver.WaitForPageLoad();
        }

        public IWebElement Find(ByEx byEx)
        {
            return WrappedDriver.FindElement(byEx);
        }

        public IEnumerable<IWebElement> FindAll(ByEx byEx)
        {
            return WrappedDriver.FindElements(byEx);
        }

        public SelectElement FindSelect(IWebElement element) // support backwards compatibility
        {
            return new SelectElement(element);
        }

        public TableElement FindTable(IWebElement element) // support backwards compatibility
        {
            return new TableElement(element);
        }

        public void Clear(IWebElement element)
        {
            TryAgain(() =>
            {
                element.Clear();
                return true;
            });
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

        public void ClickInvisible(IWebElement element)
        {
            TryAgain(() =>
            {
                ((IJavaScriptExecutor)WrappedDriver).ExecuteScript("arguments[0].Click()", element);
                return true;
            });
        }

        public bool Displayed(IWebElement element)
        {
            return TryAgain(() =>
            {
                return element.Displayed;
            });
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

        public void Select(IWebElement element)
        {
            TryAgain(() =>
            {
                element.Click();
                WaitForPageLoad();
                return element.Selected;
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