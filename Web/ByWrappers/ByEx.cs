using OpenQA.Selenium;

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
    }
}