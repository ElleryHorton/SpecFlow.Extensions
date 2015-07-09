using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;

namespace SpecFlow.WebExtension
{
    public class TableElement
    {
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

        public IWebElement GetCell(string header, string text, Func<string,string,bool> ComparisonMethod)
        {
            IWebElement e = null;
            foreach (var row in _rows)
            {
                e = row.Find(new ByEx(By.TagName("th"), text, ComparisonMethod));
                if (e != null)
                {
                    return e;
                }
                e = row.Find(new ByEx(By.TagName("td"), text, ComparisonMethod));
                if (e != null)
                {
                    return e;
                }
            }
            return e;
        }

        public int RowCount
        {
            get { return _rows.Count; }
        }

        public int RowDataCount
        {
            get { return _rows.Count - 1; }
        }

        public int ColumnCount
        {
            get { return _headerToIndex.Count; }
        }

        public bool CompareToSpecFlowTable(Table table)
        {
            if (table.Rows.Count != RowDataCount)
            {
                return false;
            }

            int rowIndex = 1;
            bool isExact = true;
            foreach (var row in table.Rows)
            {
                foreach (var header in table.Header)
                {
                    string actualCellText = GetCell(rowIndex, header).Text;
                    isExact = isExact && (row[header] == actualCellText);
                    Console.Write(string.Format("{0}\t", actualCellText));
                }
                Console.WriteLine();
                rowIndex++;
            }
            return isExact;
        }

        private IReadOnlyCollection<IWebElement> _rows;
        private Dictionary<string, int> _headerToIndex = new Dictionary<string, int>();

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