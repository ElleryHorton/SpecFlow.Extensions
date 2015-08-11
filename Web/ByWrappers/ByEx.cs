using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;

namespace SpecFlow.Extensions.Web.ByWrappers
{
    public class ByEx
    {
        public By By;
        public bool isVisible;
        public Input Input;

        public ByEx(By by, Input input = Input.Type, bool visibleOnly = true)
        {
            By = by;
            isVisible = visibleOnly;
            Input = input;
        }

        virtual public IEnumerable<IWebElement> FilterElements(IEnumerable<IWebElement> elements)
        {
            return isVisible ? elements.Where(e => e.Displayed) : elements;
        }
    }
}