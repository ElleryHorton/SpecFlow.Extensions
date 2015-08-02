using OpenQA.Selenium;
using System;
using System.Collections.ObjectModel;

namespace SpecFlow.Extensions.Web.CustomBys
{
    public class ByName : BaseCustomBy
    {
        private static readonly Func<string, By> activator = By.Name;

        public ByName(string usingString) : base(activator, usingString) { }

        public class Text : BaseText
        {
            public Text(string usingString) : base(activator, usingString) { }
        }

        public class TextPartial : BaseTextPartial
        {
            public TextPartial(string usingString) : base(activator, usingString) { }
        }

        public class Attribute : BaseAttribute
        {
            public Attribute(string usingString) : base(activator, usingString) { }
        }
    }
}
