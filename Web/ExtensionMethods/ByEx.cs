using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecFlow.Extensions.Web
{
    public class ByEx
    {
        public By By;
        public string Text = string.Empty;
        public Func<string, string, bool> TextComparisonMethod;
        public Dictionary<string, string> Attributes { get; private set; }

        public bool hasText { get { return !string.IsNullOrEmpty(Text); } }
        public bool hasAttributes { get { return (Attributes == null) ? false : Attributes.Count > 0; } }
        public bool isVisible;

        public ByEx(By by, bool visibleOnly = true)
        {
            By = by;
            isVisible = visibleOnly;
        }

        public ByEx(By by, string text, bool visibleOnly = true)
        {
            By = by;
            Text = text;
            TextComparisonMethod = string.Equals;
            isVisible = visibleOnly;
        }

        public ByEx(By by, string text, Func<string, string, bool> textComparisonMethod, bool visibleOnly = true)
        {
            By = by;
            Text = text;
            TextComparisonMethod = textComparisonMethod;
            isVisible = visibleOnly;
        }

        public ByEx(By by, string attributeName, string attributeValue, bool visibleOnly = true)
        {
            By = by;
            Attributes = new Dictionary<string, string>();
            Attributes.Add(attributeName, attributeValue);
            isVisible = visibleOnly;
        }

        public ByEx(By by, Dictionary<string, string> attr, bool visibleOnly = true)
        {
            By = by;
            Attributes = attr ?? new Dictionary<string, string>();
            isVisible = visibleOnly;
        }
    }
}
