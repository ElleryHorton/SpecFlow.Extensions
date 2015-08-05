using System;

namespace SpecFlow.Extensions.PageObjects
{
    public class PageAttribute : Attribute
    {
        public string Name;
        public string Url;
    }
}
