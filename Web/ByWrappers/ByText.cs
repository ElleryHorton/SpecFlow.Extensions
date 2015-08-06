using OpenQA.Selenium;
using System;

namespace SpecFlow.Extensions.Web.ByWrappers
{
    public class ByText : ByEx
    {
        public string Text = string.Empty;
        public Func<string, string, bool> ComparisonMethod;
        public bool hasText { get { return !string.IsNullOrEmpty(Text); } }

        public ByText(By by, string text, bool visibleOnly = true) : this(by, text, string.Equals, visibleOnly)
        {
        }

        public ByText(By by, string text, Func<string, string, bool> textComparisonMethod, bool visibleOnly = true) : base(by, visibleOnly)
        {
            Text = text;
            ComparisonMethod = textComparisonMethod;
        }
    }
}
