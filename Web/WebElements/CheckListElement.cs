using OpenQA.Selenium;
using SpecFlow.Extensions.Web.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecFlow.Extensions.Web
{
    public class CheckListElement
    {
        private ByEx _byEx;
        private IWebElement _element;
        private IWebDriver _driver;

        public CheckListElement(ByEx byEx, IWebDriver driver)
        {
            _byEx = byEx;
            _element = _driver.FindElement(_byEx);
            _driver = driver;
        }

        public CheckListElement(IWebElement element, IWebDriver driver)
        {
            _element = element;
            _driver = driver;
        }

        public IWebElement GetCheckBoxElement(string text)
        {
            var checkBoxLabels = _element.FindElement(_byEx).FindElements(By.TagName("span")).Where(label => !string.IsNullOrEmpty(label.Text)).ToList();
            var checkBoxElements = _element.FindElement(_byEx).FindElements(By.TagName("input"));
            if (checkBoxLabels.Count != checkBoxElements.Count)
                throw new InvalidCastException();

            return checkBoxElements.ElementAt(checkBoxLabels.IndexOf(checkBoxLabels.FirstOrDefault(checkbox => checkbox.Text == text)));
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
