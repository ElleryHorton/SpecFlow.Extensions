using OpenQA.Selenium;
using SpecFlow.Extensions.Web;
using System.Collections.Generic;

namespace SpecFlow.Extensions.WebDriver
{
    public interface IWrapWebDriver
    {
        IWebDriver Driver { get; }
        bool LoggedIn { get; }

        void Login();
        void Logout();
        void NavigateTo(string url);
        void WaitForPageLoad();

        // convenience methods
        bool Exists(ByEx id);
        IWebElement Find(ByEx id);
        IEnumerable<IWebElement> FindAll(ByEx id);
        void Click(ByEx id);
        void ClickInvisible(ByEx id);
        void Select(ByEx id);
        void SendKeys(ByEx id, string text);
        void Clear(ByEx id);
        void Type(ByEx id, string text);
        bool Displayed(ByEx id);
    }
}
