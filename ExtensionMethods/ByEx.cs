using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecFlow.WebExtension
{
    public class ByEx
    {
        public By By;
        public string Text = string.Empty;
        public Func<string, string, bool> TextComparisonMethod;
        public Dictionary<string, string> Attributes { get; private set; }

        public bool hasText {get { return string.IsNullOrEmpty(Text); } }
        public bool hasAttributes { get { return (Attributes == null) ? false : Attributes.Count > 0; } }

        public ByEx(By by)
        {
            By = by;
        }

        public ByEx(By by, string text)
        {
            By = by;
            Text = text;
            TextComparisonMethod = string.Equals;
        }

        public ByEx(By by, string text, Func<string, string, bool> textComparisonMethod)
        {
            By = by;
            Text = text;
            TextComparisonMethod = textComparisonMethod;
        }

        public ByEx(By by, string attributeName, string attributeValue)
        {
            By = by;
            Attributes = new Dictionary<string, string>();
            Attributes.Add(attributeName, attributeValue);
        }

        public ByEx(By by, Dictionary<string, string> attr)
        {
            By = by;
            Attributes = attr ?? new Dictionary<string, string>();
        }
    }
}
