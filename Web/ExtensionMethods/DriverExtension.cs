using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace SpecFlow.Extensions.Web
{
    public static class DriverExtension
    {
        public static void WaitForPageLoad(this IWebDriver driver)
        {
            IWait<IWebDriver> wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60.00));
            wait.Until(w => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        }

        public static void WaitForElement(this IWebDriver driver, ByEx id)
        {
            const int TIME_MAX = 10;
            int time_current = 0;

            var e = driver.FindElementOrNull(id);
            while ( (time_current < TIME_MAX) && e == null )
            {
                Thread.Sleep(1000);
                time_current++;
                e = driver.FindElementOrNull(id);
            }
            int time_delta = Math.Max(0, TIME_MAX - time_current);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(time_delta));
            wait.Until<IWebElement>((d) =>
                {
                    if (e.Displayed && e.Enabled)
                    {
                        return e;
                    }
                    else
                    {
                        return null;
                    }
                }
            );
        }
    }
}