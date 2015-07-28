using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SpecFlow.Extensions.Web
{
    public static class ISearchContextExtension
    {
        private const int MAX_RETRIES = 3;

        public static bool HasChild(this ISearchContext iFind, ByEx id)
        {
            return FindAll(iFind, id, 1, 0).Any(e => e.Displayed == true);
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
            return FindAll(iFind, id, MAX_RETRIES, 2000);
        }

        private static IEnumerable<IWebElement> FindAll(this ISearchContext iFind, ByEx id, int retries, int milliseconds)
        {
            var elements = SelectFindAllMethod(iFind, id);
            while ((retries > 0) && elements.Count() == 0)
            {
                Thread.Sleep(milliseconds);
                elements = SelectFindAllMethod(iFind, id);
                retries--;
            }
            return elements;
        }

        public static IWebElement FindElementOrNull(this ISearchContext iFind, ByEx id)
        {
            try
            {
                IEnumerable<IWebElement> elements = id.isVisible ? iFind.FindElements(id.By).Where(e => e.Displayed) : iFind.FindElements(id.By);
                if (id.hasText && elements.Count() > 0)
                {
                    elements = FilterElementsByText(elements, id);
                }
                if (id.hasAttributes && elements.Count() > 0)
                {
                    elements = FilterElementsByAttributes(elements, id);
                }
                return elements.FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public static SelectElement FindSelect(this ISearchContext iFind, ByEx id)
        {
            int tryCount = 0;
            var e = new SelectElement(iFind.Find(id));

            while ((tryCount < MAX_RETRIES) && e.Options.Count == 0)
            {
                Thread.Sleep(1000);
                e = new SelectElement(iFind.Find(id));
                tryCount++;
            }
            return e;
        }

        public static TableElement FindTable(this ISearchContext iFind, ByEx id)
        {
            return new TableElement(iFind.Find(id));
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

        private static IEnumerable<IWebElement> SelectFindAllMethod(ISearchContext iFind, ByEx id)
        {
            IEnumerable<IWebElement> elements = null;
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
            var elements = id.isVisible ? iFind.FindElements(id.By).Where(e => e.Displayed) : iFind.FindElements(id.By);
            return FilterElementsByAttributes(elements, id);
        }

        private static IEnumerable<IWebElement> FilterElementsByAttributes(IEnumerable<IWebElement> elements, ByEx id)
        {
            return elements.Where(element => id.Attributes.All(attribute => element.GetAttribute(attribute.Key) == attribute.Value));
        }

        private static IEnumerable<IWebElement> FindElementsByText(this ISearchContext iFind, ByEx id)
        {
            var elements = id.isVisible ? iFind.FindElements(id.By).Where(e => e.Displayed) : iFind.FindElements(id.By);
            return FilterElementsByText(elements, id);
        }

        private static IEnumerable<IWebElement> FilterElementsByText(IEnumerable<IWebElement> elements, ByEx id)
        {
            return elements.Where(e => id.TextComparisonMethod(e.Text, id.Text));
        }
    }
}