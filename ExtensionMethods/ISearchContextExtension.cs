using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;
using System.Threading;

namespace SpecFlow.WebExtension
{
    public static class ISearchContextExtension
    {
        private static ByEx _lastIdUsed;
        private const int MAX_RETRIES = 3;

        public static IWebElement Find(this ISearchContext iFind, ByEx id)
        {
            IWebElement e = null;

            int tryCount = 0;
            while ((tryCount < MAX_RETRIES) && e == null)
            {
                e = SelectFindMethod(iFind, id, e);

                tryCount++;
                if (e == null)
                {
                    Thread.Sleep(2000);
                }
            }

            if (e == null) // fail normally with built-in FindElement
            {
                e = iFind.FindElement(id.By);
            }

            return e;
        }

        private static IWebElement SelectFindMethod(ISearchContext iFind, ByEx id, IWebElement e)
        {
            if (id.hasText)
            {
                e = iFind.FindElementByText(id, string.Equals);
            }
            else if (id.hasAttributes)
            {
                e = iFind.FindElementByAttribute(id);
            }
            else
            {
                e = iFind.FindElementOrNull(id);
            }
            return e;
        }

        public static SelectElement FindSelect(this ISearchContext iFind, ByEx id)
        {
            return new SelectElement(iFind.Find(id));
        }

        public static TableElement FindTable(this ISearchContext iFind, ByEx id)
        {
            return new TableElement(iFind.Find(id));
        }

        public static IWebElement FindElementOrNull(this ISearchContext iFind, ByEx id)
        {
            IWebElement e;
            try
            {
                e = iFind.FindElement(id.By);
                _lastIdUsed = id;
            }
            catch
            {
                e = null;
            }
            return e;
        }

        public static IWebElement FindElementByAttribute(this ISearchContext iFind, ByEx id)
        {
            var elements = iFind.FindElements(id.By);
            var element = elements.FirstOrDefault(e => e.GetAttribute(id.Attributes.Keys.ToArray()[0]) == id.Attributes.Values.ToArray()[0]);
            return element;
        }

        public static IWebElement FindElementByText(this ISearchContext iFind, ByEx id, Func<string, string, bool> ComparisonMethod)
        {
            var elements = iFind.FindElements(id.By);
            var element = elements.FirstOrDefault(e => ComparisonMethod(e.Text, id.Text));
            return element;
        }
    }
}