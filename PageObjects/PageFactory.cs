using SpecFlow.Extensions.WebDriver.PortalDriver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SpecFlow.Extensions.PageObjects
{
    public static class PageFactory
    {
        private static Dictionary<Type, Page> _pageBag = new Dictionary<Type, Page>();

        public static Page GetPage(string pageName)
        {
            Type pageType = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => assembly.IsDefined(typeof(AssemblyContainsPages)))
                .SelectMany(assembly => assembly.GetTypes())
                .FirstOrDefault(type => type.FullName.EndsWith(pageName));
            if (pageType != null)
            {
                if (_pageBag.Keys.Contains(pageType))
                {
                    return _pageBag[pageType];
                }
                else
                {
                    Page newPage = (Page)Activator.CreateInstance(pageType);
                    _pageBag.Add(pageType, newPage);
                    return newPage;
                }
            }
            throw new Exception(String.Format("Unable to find page by name '{0}'", pageName));
        }

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