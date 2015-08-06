using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace SpecFlow.Extensions.Web.ByWrappers
{
    public class ByAttribute : ByEx
    {
        public Dictionary<string, string> Attributes { get; private set; }
        public Func<string, string, bool> ComparisonMethod;
        public bool hasAttributes { get { return (Attributes == null) ? false : Attributes.Count > 0; } }
        
        public ByAttribute(By by, string attributeName, string attributeValue, bool visibleOnly = true) : this(by, attributeName, attributeValue, string.Equals, visibleOnly)
        {
        }

        public ByAttribute(By by, string attributeName, string attributeValue, Func<string, string, bool> attrComparisonMethod, bool visibleOnly = true) : base(by, visibleOnly)
        {
            Attributes = new Dictionary<string, string>();
            Attributes.Add(attributeName, attributeValue);
            ComparisonMethod = attrComparisonMethod;
        }

        public ByAttribute(By by, Dictionary<string, string> attr, bool visibleOnly = true) : this(by, attr, string.Equals, visibleOnly)
        {
        }

        public ByAttribute(By by, Dictionary<string, string> attr, Func<string, string, bool> attrComparisonMethod, bool visibleOnly = true) : this(by, attr, visibleOnly)
        {
            Attributes = attr ?? new Dictionary<string, string>();
            ComparisonMethod = attrComparisonMethod;
        }

    }
}
