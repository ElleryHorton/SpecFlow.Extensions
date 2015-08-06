using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.PageObjects;
using SpecFlow.Extensions.WebDriver;
using System;

namespace SpecFlow.Extensions.PageObjects
{
    public abstract class Page
    {
        public static IDriverFactory DriverFactory;

        public Page() : this(DriverFactory.GetDriver().WrappedDriver) { }

        public Page(IWrapsDriver WrapsDriver) : this(WrapsDriver.WrappedDriver) { }

        public Page(IWebDriver WebDriver)
        {
            IElementLocator retryingLocator = new RetryingElementLocator(WebDriver, TimeSpan.FromSeconds(5));
            OpenQA.Selenium.Support.PageObjects.PageFactory.InitElements(this, retryingLocator);
        }

		public string Uri
        {
            get
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.Name;
                assembly = assembly.Substring(0, assembly.Length - ".dll".Length);
                return GetType().FullName.Replace(assembly, string.Empty).Replace(".", @"/");
            }
        }
    }
}
