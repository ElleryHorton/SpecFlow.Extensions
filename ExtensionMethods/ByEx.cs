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
        public Dictionary<string, string> Attributes = new Dictionary<string,string>();

        public bool hasText {get {return Text != string.Empty;}}
        public bool hasAttributes {get {return Attributes.Count>0;}}

        public ByEx(By by)
        {
            By = by;
        }

        public ByEx(By by, string text)
        {
            By = by;
            Text = text;
        }

        public ByEx(By by, string attributeName, string attributeValue)
        {
            By = by;
            Attributes.Add(attributeName, attributeValue);
        }

        public ByEx(By by, Dictionary<string, string> attr)
        {
            By = by;
            Attributes = attr;
        }
    }
}
