using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SpecFlow.WebExtension
{
    public static class ISearchContextExtension
    {
        private const int MAX_RETRIES = 3;


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
            }
            catch
            {
                e = null;
            }
            return e;
        }

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

        public static IEnumerable<IWebElement> FindAll(this ISearchContext iFind, ByEx id)
        {
            IEnumerable<IWebElement> elements = new List<IWebElement>();

            int tryCount = 0;
            int count = elements.Count();
            while ((tryCount < MAX_RETRIES) && count == 0)
            {
                elements = SelectFindAllMethod(iFind, id, elements);

                tryCount++;
                if (count == 0)
                {
                    Thread.Sleep(2000);
                }
            }

            return elements;
        }

        private static IWebElement SelectFindMethod(ISearchContext iFind, ByEx id, IWebElement e)
        {
            if (id.hasText)
            {
                e = iFind.FindElementByText(id);
            }
            else if (id.hasAttributes)
            {
                e = iFind.FindElementByAttributes(id);
            }
            else
            {
                e = iFind.FindElementOrNull(id);
            }
            return e;
        }

        private static IWebElement FindElementByAttributes(this ISearchContext iFind, ByEx id)
        {
            return FirstElementOrThrowNoSuchElementException(iFind.FindElementsByAttributes(id));
        }

        private static IWebElement FindElementByText(this ISearchContext iFind, ByEx id)
        {
            return FirstElementOrThrowNoSuchElementException(iFind.FindElementsByText(id));
        }

        private static IWebElement FirstElementOrThrowNoSuchElementException(IEnumerable<IWebElement> list)
        {
            if (list.Count() == 0)
            {
                throw new NoSuchElementException();
            }
            return list.First();
        }

        private static IEnumerable<IWebElement> SelectFindAllMethod(ISearchContext iFind, ByEx id, IEnumerable<IWebElement> elements)
        {
            if (id.hasText)
            {
                elements = iFind.FindElementsByText(id);
            }
            else if (id.hasAttributes)
            {
                elements = iFind.FindElementsByAttributes(id);
            }
            else
            {
                elements = iFind.FindElements(id.By);
            }
            return elements;
        }

        private static IEnumerable<IWebElement> FindElementsByAttributes(this ISearchContext iFind, ByEx id)
        {
            IEnumerable<IWebElement> elements = iFind.FindElements(id.By);
            elements = elements.Where(element => id.Attributes.All(attribute => element.GetAttribute(attribute.Key) == attribute.Value));
            return elements;
        }

        private static IEnumerable<IWebElement> FindElementsByText(this ISearchContext iFind, ByEx id)
        {
            var elements = iFind.FindElements(id.By);
            return elements.Where(e => id.TextComparisonMethod(e.Text, id.Text));
        }
    }
}