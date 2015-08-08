using OpenQA.Selenium;
using SpecFlow.Extensions.Web.ByWrappers;
using SpecFlow.Extensions.Web.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;

namespace SpecFlow.Extensions.Web
{
    public class TableElement
    {
        protected IEnumerable<IWebElement> _rows;
        protected Dictionary<string, int> _headerToIndex = new Dictionary<string, int>();

        public TableElement()
        {
        }

        public TableElement(IWebElement element)
        {
            MapHeadersToIndex(GetTableHeaders(element));
            _rows = element.FindElements(By.TagName("tr"));
        }

        public IWebElement GetCell(int rowIndex, string header)
        {
            var row = _rows.ToArray()[rowIndex];
            var columnElements = row.FindElements(By.TagName("th"));
            if (columnElements.Count == 0)
            {
                columnElements = row.FindElements(By.TagName("td"));
            }
            if (columnElements.Count == 0)
            {
                throw new InvalidCastException("WebElement is not a valid TableElement, no th or td tags in tr.");
            }
            return columnElements.ToArray()[_headerToIndex[header]];
        }

        public IWebElement GetCell(string header, string text, Func<string, string, bool> ComparisonMethod)
        {
            foreach (var row in _rows)
            {
                var e = row.FindElementOrNull(new ByText(By.TagName("th"), text, ComparisonMethod));
                if (e != null)
                {
                    return e;
                }
                e = row.FindElementOrNull(new ByText(By.TagName("td"), text, ComparisonMethod));
                if (e != null)
                {
                    return e;
                }
            }
            return null;
        }

        public ByEx GetCellByEx(string header, string text, Func<string, string, bool> ComparisonMethod)
        {
            foreach (var row in _rows)
            {
                var byEx = new ByText(By.TagName("th"), text, ComparisonMethod);
                var e = row.FindElementOrNull(byEx);
                if (e != null)
                {
                    return byEx;
                }
                byEx = new ByText(By.TagName("td"), text, ComparisonMethod);
                e = row.FindElementOrNull(byEx);
                if (e != null)
                {
                    return byEx;
                }
            }
            return null;
        }

        public int RowCount
        {
            get { return _rows.Count(); }
        }

        virtual public int RowDataCount
        {
            get { return _rows.Count() - 1; }
        }

        public int ColumnCount
        {
            get { return _headerToIndex.Count; }
        }

        public List<string> GetRowText(int row)
        {
            List<string> list = new List<string>();

            foreach (var header in _headerToIndex.Keys)
            {
                list.Add(GetCell(row, header).Text);
            }

            return list;
        }

        public bool CompareToTable(List<string[]> table, int startIndex = 1)
        {
            if (table.Count != RowCount)
            {
                return false;
            }

            for (int row = startIndex; row < table.Count; row++)
            {
                var colCount = table[row].Count();
                for (int col = 0; col < colCount; col++)
                {
                    string header = table[0][col];
                    string actualCellText = GetCell(row, header).Text;
                    if (table[row][col] != actualCellText)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool CompareToTable(Table table, int startIndex = 1)
        {
            if (table.Rows.Count != RowDataCount)
            {
                return false;
            }

            int rowIndex = startIndex;
            bool isExact = true;
            foreach (var row in table.Rows)
            {
                foreach (var header in table.Header)
                {
                    string actualCellText = GetCell(rowIndex, header).Text;
                    isExact = isExact && (row[header] == actualCellText);
                }
                rowIndex++;
            }
            return isExact;
        }

        private static IReadOnlyCollection<IWebElement> GetTableHeaders(IWebElement element)
        {
            var columnElements = element.FindElements(By.TagName("th"));
            if (columnElements.Count == 0)
            {
                var headerRow = element.FindElements(By.TagName("tr")).FirstOrDefault();
                columnElements = headerRow.FindElements(By.TagName("td"));
            }
            if (columnElements.Count == 0)
            {
                throw new InvalidCastException("WebElement is not a valid TableElement, no th or td tags in first tr.");
            }
            return columnElements;
        }

        private void MapHeadersToIndex(IReadOnlyCollection<IWebElement> headers)
        {
            int index = 0;
            foreach (var headerElement in headers)
            {
                _headerToIndex.Add(headerElement.Text, index);
                index++;
            }
        }
    }
}