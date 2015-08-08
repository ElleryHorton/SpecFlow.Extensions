﻿using OpenQA.Selenium;
using System;

namespace SpecFlow.Extensions.Web.ByWrappers
{
    public class ByText : ByEx
    {
        public string Text = string.Empty;
        public Func<string, string, bool> ComparisonMethod;

        public bool hasText { get { return !string.IsNullOrEmpty(Text); } }

        public ByText(By by, string text, Input input = Input.Type, bool visibleOnly = true)
            : this(by, text, string.Equals, input, visibleOnly)
        {
        }

        public ByText(By by, string text, Func<string, string, bool> textComparisonMethod, Input input = Input.Type, bool visibleOnly = true)
            : base(by, input, visibleOnly)
        {
            Text = text;
            ComparisonMethod = textComparisonMethod;
        }
    }
}