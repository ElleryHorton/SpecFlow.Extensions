using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace SpecFlow.Extensions.Web.ExtensionMethods
{
    public static class WebDriverExtension
    {
        public static void WaitForPageLoad(this IWebDriver driver, int seconds = 60)
        {
            IWait<IWebDriver> wait = new WebDriverWait(driver, TimeSpan.FromSeconds(seconds));
            wait.Until(w => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        }

        public static void WaitForElement(this IWebDriver driver, ByEx byEx)
        {
            const int TIME_MAX = 10;
            int time_current = 0;

            var e = driver.FindElementOrNull(byEx);
            while ( (time_current < TIME_MAX) && e == null )
            {
                Thread.Sleep(1000);
                time_current++;
                e = driver.FindElementOrNull(byEx);
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