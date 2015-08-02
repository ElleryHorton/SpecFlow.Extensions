using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;

namespace SpecFlow.Extensions.WebDriver.PortalDriver
{
    public interface IPortal
    {
        bool LoggedIn();
        void Login();
        void Logout();
        void NavigateHome();
        void NavigateTo(string url);
    }
}
