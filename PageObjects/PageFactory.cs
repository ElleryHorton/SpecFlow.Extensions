using SpecFlow.Extensions.WebDriver.PortalDriver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecFlow.Extensions.PageObjects
{
    public static class PageFactory
    {
        private static Dictionary<Type, Page> _pageBag = new Dictionary<Type, Page>();

        public static T Get<T>() where T : Page
        {
            if (_pageBag.Keys.Contains(typeof(T)))
            {
                return (T)_pageBag[typeof(T)];
            }
            else
            {
                var newPage = (T)Activator.CreateInstance<T>();
                _pageBag.Add(typeof(T), newPage);
                return newPage;
            }
        }

        public static T Get<T>(IPortalDriver portalDriver) where T : Page
        {
            if (_pageBag.Keys.Contains(typeof(T)))
            {
                return (T)_pageBag[typeof(T)];
            }
            else
            {
                var newPage = (T)Activator.CreateInstance(typeof(T), portalDriver);
                _pageBag.Add(typeof(T), newPage);
                return newPage;
            }
        }
    }
}
