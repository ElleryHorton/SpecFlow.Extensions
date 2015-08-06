using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SpecFlow.Extensions.Web.ByWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SpecFlow.Extensions.Web.ExtensionMethods
{
    public static class ISearchContextExtension
    {
        private const int MAX_RETRIES = 3;
        private enum ByTypes { ByEx, ByAttribute, ByColumns, ByText };
        private static IReadOnlyDictionary<Type, ByTypes> types = new Dictionary<Type, ByTypes>
        {
            {typeof(ByEx), ByTypes.ByEx},
            {typeof(ByAttribute), ByTypes.ByAttribute},
            {typeof(ByColumns), ByTypes.ByColumns},
            {typeof(ByText), ByTypes.ByText}
        };

        public static bool HasChild(this ISearchContext iFind, ByEx byEx)
        {
            return FindAll(iFind, byEx, 1, 0).Any(e => e.Displayed == true);
        }

        public static IWebElement FindElement(this ISearchContext iFind, ByEx byEx)
        {
            IWebElement e = null;

            int tryCount = 0;
            while ((tryCount < MAX_RETRIES) && e == null)
            {
                e = FindMethod(iFind, byEx);

                tryCount++;
                if (e == null)
                {
                    Thread.Sleep(1000);
                }
            }

            if (e == null) // fail normally with built-in FindElement
            {
                if (byEx is ByText || byEx is ByAttribute) // default FindElement(By) won't find it
                {
                    throw new NoSuchElementException();
                }
                e = iFind.FindElement(byEx.By);
            }

            return e;
        }

        public static IEnumerable<IWebElement> FindElements(this ISearchContext iFind, ByEx byEx)
        {
            return FindAll(iFind, byEx, MAX_RETRIES, 2000);
        }

        private static IEnumerable<IWebElement> FindAll(this ISearchContext iFind, ByEx byEx, int retries, int milliseconds)
        {
            var elements = FindAllMethod(iFind, byEx);
            while ((retries > 0) && elements.Count() == 0)
            {
                Thread.Sleep(milliseconds);
                elements = FindAllMethod(iFind, byEx);
                retries--;
            }
            return elements;
        }

        public static IWebElement FindElementOrNull(this ISearchContext iFind, ByEx byEx)
        {
            return FindElements(iFind, byEx).FirstOrDefault();
        }

        public static SelectElement FindSelect(this ISearchContext iFind, ByEx byEx)
        {
            int tryCount = 0;
            var e = new SelectElement(iFind.FindElement(byEx));

            while ((tryCount < MAX_RETRIES) && e.Options.Count == 0)
            {
                Thread.Sleep(1000);
                e = new SelectElement(iFind.FindElement(byEx));
                tryCount++;
            }
            return e;
        }

        public static TableElement FindTable(this ISearchContext iFind, ByEx byEx)
        {
            return new TableElement(iFind.FindElement(byEx));
        }

        private static IWebElement FindMethod(ISearchContext iFind, ByEx byEx)
        {
            switch (types[byEx.GetType()])
            {
                case ByTypes.ByAttribute:
                    return FindElementsByAttributes(iFind, (ByAttribute)byEx).FirstOrDefault();
                case ByTypes.ByText:
                    return FindElementsByText(iFind, (ByText)byEx).FirstOrDefault();
                default:
                    return iFind.FindElements(byEx.By).FirstOrDefault();
            }
        }

        private static IEnumerable<IWebElement> FindAllMethod(ISearchContext iFind, ByEx byEx)
        {
            switch (types[byEx.GetType()])
            {
                case ByTypes.ByAttribute:
                    return FindElementsByAttributes(iFind, (ByAttribute)byEx);
                case ByTypes.ByText:
                    return FindElementsByText(iFind, (ByText)byEx);
                default:
                    return iFind.FindElements(byEx.By);
            }
        }

        private static IEnumerable<IWebElement> FindElementsByAttributes(this ISearchContext iFind, ByAttribute byEx)
        {
            var elements = byEx.isVisible ? iFind.FindElements(byEx.By).Where(e => e.Displayed) : iFind.FindElements(byEx.By);
            return FilterElementsByAttributes(elements, byEx);
        }

        private static IEnumerable<IWebElement> FilterElementsByAttributes(IEnumerable<IWebElement> elements, ByAttribute byEx)
        {
            return elements.Where(element => byEx.Attributes.All(attribute => byEx.ComparisonMethod(element.GetAttribute(attribute.Key), attribute.Value)));
        }

        private static IEnumerable<IWebElement> FindElementsByText(this ISearchContext iFind, ByText byEx)
        {
            var elements = byEx.isVisible ? iFind.FindElements(byEx.By).Where(e => e.Displayed) : iFind.FindElements(byEx.By);
            return FilterElementsByText(elements, byEx);
        }

        private static IEnumerable<IWebElement> FilterElementsByText(IEnumerable<IWebElement> elements, ByText byEx)
        {
            return elements.Where(e => byEx.ComparisonMethod(e.Text, byEx.Text));
        }
    }
}