using OpenQA.Selenium;
using System.Collections.Generic;

namespace SpecFlow.WebExtension
{
    public interface IWrapWebDriver
    {
        bool LoggedIn { get; }

        void Login();
        void Logout();
        void NavigateTo(string url);
        void WaitForPageLoad();

        // convenience methods
        IWebElement Find(ByEx id);
        IEnumerable<IWebElement> FindAll(ByEx id);
        void Click(ByEx id);
        void SendKeys(ByEx id, string text);
        void Clear(ByEx id);
        bool Displayed(ByEx id);
    }
}
