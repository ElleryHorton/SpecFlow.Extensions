using OpenQA.Selenium;
using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace SpecFlow.Extensions.Web.CustomBys
{
    public class ById : BaseCustomBy
    {
        private static readonly Func<string, By> activator = By.Id;

        public ById(string usingString) : base(activator, usingString) { }

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

        public class TitleCase : BaseCustomBy
        {
            private static readonly TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            public TitleCase(string usingString) : base(activator, textInfo.ToTitleCase(usingString)) { }
        }

        public class Trim : BaseTrim
        {
            public Trim(string usingString) : base(activator, usingString) { }
        }
    }
}
