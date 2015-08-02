using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SpecFlow.Extensions.Web;
using SpecFlow.Extensions.Web.ExtensionMethods;
using System;
using System.Collections.Generic;

namespace SpecFlow.Extensions.WebDriver.PortalDriver
{
    public interface IPortalInteract
    {
        void WaitForPageLoad();

        /*
         * These are convenience methods so that users of IPortalDriver do not have to access the Driver.Find methods
         * 
         * Find calls out to Driver.Find
         *      which in turn calls the derivative Find methods of ISearchContext and ISearchContextExtension
         *      
         * The other methods should call out to their respective Driver.Find.<method> Methods
         */
        IWebElement Find(ByEx byEx);
        IEnumerable<IWebElement> FindAll(ByEx byEx);
        SelectElement FindSelect(IWebElement element);
        TableElement FindTable(IWebElement element);

        void Clear(IWebElement element);
        void Click(IWebElement element);
        void ClickInvisible(IWebElement element);
        bool Displayed(IWebElement element);
        bool Exists(Func<IWebElement> pageObjectElement);
        void Select(IWebElement element);
        void SendKeys(IWebElement element, string text);
        void Type(IWebElement element, string text);
    }
}
