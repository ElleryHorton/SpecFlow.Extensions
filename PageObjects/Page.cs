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
		private const string DllExtension = ".dll";
		private const string SamePageSubSectionDelimiter = "_";

        public Page()
            : this(DriverFactory == null ? null : DriverFactory.GetDriver().WrappedDriver)
        {
        }

        public Page(IWrapsDriver WrapsDriver)
            : this(WrapsDriver.WrappedDriver)
        {
        }

        public Page(IWebDriver WebDriver)
        {
            IElementLocator retryingLocator = new RetryingElementLocator(WebDriver, TimeSpan.FromSeconds(5));
            OpenQA.Selenium.Support.PageObjects.PageFactory.InitElements(this, retryingLocator);
        }

		public string Uri
		{
			get
			{
				var assemblyName = System.Reflection.Assembly.GetAssembly(GetType()).ManifestModule.Name;
				assemblyName = assemblyName.Substring(0, assemblyName.Length - DllExtension.Length);
				int subsectionDivider = assemblyName.IndexOf(SamePageSubSectionDelimiter);
				if (subsectionDivider >= 0)
				{
					assemblyName = assemblyName.Substring(0, (assemblyName.Length - subsectionDivider) + 1);
				}
				return GetType().FullName.Replace(assemblyName, string.Empty).Replace(".", @"/");
			}
		}
	}
}
