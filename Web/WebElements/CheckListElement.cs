using OpenQA.Selenium;
using System;
using System.Linq;

namespace SpecFlow.Extensions.Web
{
    public class CheckListElement
    {
        private ByEx _id;
        private IWebDriver _driver;
        public CheckListElement(ByEx id, IWebDriver driver)
        {
            _id = id;
            _driver = driver;
        }

        public IWebElement GetCheckBoxElement(string text)
        {
            var checkBoxLabels = _driver.Find(_id).FindElements(By.TagName("span")).Where(label => !string.IsNullOrEmpty(label.Text)).ToList();
            var checkBoxElements = _driver.Find(_id).FindElements(By.TagName("input"));
            if (checkBoxLabels.Count != checkBoxElements.Count)
                throw new InvalidCastException();

            return checkBoxElements.ElementAt(checkBoxLabels.IndexOf(checkBoxLabels.FirstOrDefault(checkboxes => checkboxes.Text == text)));
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
