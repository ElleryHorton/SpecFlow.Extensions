using OpenQA.Selenium;

namespace SpecFlow.Extensions.Web.ByWrappers
{
    public class ByEx
    {
        public By By;       
        public bool isVisible;

        public ByEx(By by, bool visibleOnly = true)
        {
            By = by; 
            isVisible = visibleOnly;
        }
    }
}
