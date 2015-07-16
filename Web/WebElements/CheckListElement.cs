using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecFlow.Extensions.Web
{
    public class CheckListElement
    {
        private Dictionary<string, IWebElement> _checkboxes;
        private IWebElement _content;
        public CheckListElement(IWebElement element)
        {
            _checkboxes = new Dictionary<string, IWebElement>();
            _content = element;

            var checkBoxLabels = _content.FindElements(By.TagName("span"));
            var checkBoxElements = _content.FindElements(By.TagName("input"));
            if (checkBoxLabels.Count != checkBoxElements.Count)
                throw new InvalidCastException();

            for (int i = 0; i < checkBoxLabels.Count; i++)
            {
                _checkboxes.Add(checkBoxLabels[i].Text, checkBoxElements[i]);
            }
        }

        public IWebElement GetCheckBoxElement(string text)
        {
            return _checkboxes.FirstOrDefault(checkbox => checkbox.Key.Equals(text)).Value;
        }

        public ByEx GetByExId(string text)
        {
            return new ByEx(By.Id(GetCheckBoxElement(text).GetAttribute("id")));
        }

        public ByEx GetByExName(string text)
        {
            return new ByEx(By.Name(GetCheckBoxElement(text).GetAttribute("name")));
        }
    }
}
