using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SpecFlow.Extensions.Web.ExtensionMethods
{
    public static class ISearchContextExtension
    {
        private const int MAX_RETRIES = 3;

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
                e = SelectFindMethod(iFind, byEx, e);

                tryCount++;
                if (e == null)
                {
                    Thread.Sleep(1000);
                }
            }

            if (e == null) // fail normally with built-in FindElement
            {
                if (byEx.hasText || byEx.hasAttributes) // default FindElement(By) won't find it
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
            var elements = SelectFindAllMethod(iFind, byEx);
            while ((retries > 0) && elements.Count() == 0)
            {
                Thread.Sleep(milliseconds);
                elements = SelectFindAllMethod(iFind, byEx);
                retries--;
            }
            return elements;
        }

        public static IWebElement FindElementOrNull(this ISearchContext iFind, ByEx byEx)
        {
            try
            {
                IEnumerable<IWebElement> elements = byEx.isVisible ? iFind.FindElements(byEx.By).Where(e => e.Displayed) : iFind.FindElements(byEx.By);
                if (byEx.hasText && elements.Count() > 0)
                {
                    elements = FilterElementsByText(elements, byEx);
                }
                if (byEx.hasAttributes && elements.Count() > 0)
                {
                    elements = FilterElementsByAttributes(elements, byEx);
                }
                return elements.FirstOrDefault();
            }
            catch
            {
                return null;
            }
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


        private static IWebElement SelectFindMethod(ISearchContext iFind, ByEx byEx, IWebElement e)
        {
            if (byEx.hasText)
            {
                e = iFind.FindElementByText(byEx);
            }
            else if (byEx.hasAttributes)
            {
                e = iFind.FindElementByAttributes(byEx);
            }
            else
            {
                e = iFind.FindElementOrNull(byEx);
            }
            return e;
        }

        private static IWebElement FindElementByAttributes(this ISearchContext iFind, ByEx byEx)
        {
            return FirstElementOrThrowNoSuchElementException(iFind.FindElementsByAttributes(byEx));
        }

        private static IWebElement FindElementByText(this ISearchContext iFind, ByEx byEx)
        {
            return FirstElementOrThrowNoSuchElementException(iFind.FindElementsByText(byEx));
        }

        private static IWebElement FirstElementOrThrowNoSuchElementException(IEnumerable<IWebElement> list)
        {
            if (list.Count() == 0)
            {
                throw new NoSuchElementException();
            }
            return list.First();
        }

        private static IEnumerable<IWebElement> SelectFindAllMethod(ISearchContext iFind, ByEx byEx)
        {
            IEnumerable<IWebElement> elements = null;
            if (byEx.hasText)
            {
                elements = iFind.FindElementsByText(byEx);
            }
            else if (byEx.hasAttributes)
            {
                elements = iFind.FindElementsByAttributes(byEx);
            }
            else
            {
                elements = iFind.FindElements(byEx.By);
            }
            return elements;
        }

        private static IEnumerable<IWebElement> FindElementsByAttributes(this ISearchContext iFind, ByEx byEx)
        {
            var elements = byEx.isVisible ? iFind.FindElements(byEx.By).Where(e => e.Displayed) : iFind.FindElements(byEx.By);
            return FilterElementsByAttributes(elements, byEx);
        }

        private static IEnumerable<IWebElement> FilterElementsByAttributes(IEnumerable<IWebElement> elements, ByEx byEx)
        {
            return elements.Where(element => byEx.Attributes.All(attribute => element.GetAttribute(attribute.Key) == attribute.Value));
        }

        private static IEnumerable<IWebElement> FindElementsByText(this ISearchContext iFind, ByEx byEx)
        {
            var elements = byEx.isVisible ? iFind.FindElements(byEx.By).Where(e => e.Displayed) : iFind.FindElements(byEx.By);
            return FilterElementsByText(elements, byEx);
        }

        private static IEnumerable<IWebElement> FilterElementsByText(IEnumerable<IWebElement> elements, ByEx byEx)
        {
            return elements.Where(e => byEx.TextComparisonMethod(e.Text, byEx.Text));
        }
    }
}