using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecFlow.Extensions.Web.ExtensionMethods
{
    public class ByEx
    {
        public By By;

        public string Text = string.Empty;
        public Func<string, string, bool> ComparisonMethod;
        public Dictionary<string, string> Attributes { get; private set; }
        public string[] Columns = null;

        public bool hasText { get { return !string.IsNullOrEmpty(Text); } }
        public bool hasAttributes { get { return (Attributes == null) ? false : Attributes.Count > 0; } }
        public bool isVisible;

        public ByEx(By by, bool visibleOnly = true)
        {
            By = by; 
            isVisible = visibleOnly;
        }

        public ByEx(By by, string text, bool visibleOnly = true) : this(by, visibleOnly)
        {
            Text = text;
            ComparisonMethod = string.Equals;
        }

        public ByEx(By by, string text, Func<string, string, bool> textComparisonMethod, bool visibleOnly = true) : this(by, visibleOnly)
        {
            Text = text;
            ComparisonMethod = textComparisonMethod;
        }

        public ByEx(By by, string attributeName, string attributeValue, bool visibleOnly = true) : this(by, visibleOnly)
        {
            Attributes = new Dictionary<string, string>();
            Attributes.Add(attributeName, attributeValue);
            ComparisonMethod = string.Equals;
        }

        public ByEx(By by, string attributeName, string attributeValue, Func<string, string, bool> attrComparisonMethod, bool visibleOnly = true) : this(by, visibleOnly)
        {
            Attributes = new Dictionary<string, string>();
            Attributes.Add(attributeName, attributeValue);
            ComparisonMethod = attrComparisonMethod;
        }

        public ByEx(By by, Dictionary<string, string> attr, bool visibleOnly = true) : this(by, visibleOnly)
        {
            Attributes = attr ?? new Dictionary<string, string>();
        }

        public ByEx(By by, string[] columnHeaders, bool visibleOnly = true) : this(by, visibleOnly)
        {
            Columns = columnHeaders;
        }
    }
}
