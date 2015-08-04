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
        bool ClickChangesUrl(ByEx byEx);
        bool ClickChangesUrl(IWebElement element);

        /*
         * These are convenience methods so that users of IPortalDriver do not have to access the Driver.Find methods
         * 
         * Find calls out to Driver.Find
         *      which in turn calls the derivative Find methods of ISearchContext and ISearchContextExtension
         *      
         * The other methods should call out to their respective Driver.Find.<method> Methods
         */
        IWebElement Find(ByEx byEx);
        IWebElement Find(IWebElement element);
        IEnumerable<IWebElement> FindAll(ByEx byEx);
        SelectElement FindSelect(ByEx byEx);
        SelectElement FindSelect(IWebElement element);
        TableElement FindTable(ByEx byEx);
        TableElement FindTable(IWebElement element);

        void Clear(ByEx byEx);
        void Clear(IWebElement element);
        void Click(ByEx byEx);
        void Click(IWebElement element);
        void ClickInvisible(ByEx byEx);
        void ClickInvisible(IWebElement element);
        bool Displayed(ByEx byEx);
        bool Displayed(IWebElement element);
        bool Exists(ByEx byEx);
        bool Exists(Func<IWebElement> pageObjectElement);
        void Select(ByEx byEx);
        void Select(IWebElement element);
        void SendKeys(ByEx byEx, string text);
        void SendKeys(IWebElement element, string text);
        void Type(ByEx byEx, string text);
        void Type(IWebElement element, string text);
    }
}
