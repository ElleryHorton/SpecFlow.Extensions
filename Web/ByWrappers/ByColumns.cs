using OpenQA.Selenium;
using System.Collections.Generic;

namespace SpecFlow.Extensions.Web.ByWrappers
{
    public class ByColumns : ByEx
    {
        public string[] Columns = null;

        public ByColumns(By by, string[] columnHeaders, bool visibleOnly = true) : base(by, visibleOnly)
        {
            Columns = columnHeaders;
        }
    }
}
