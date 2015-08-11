using SpecFlow.Extensions.WebDriver.PortalDriver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SpecFlow.Extensions.PageObjects
{
    public static class PageFactory
    {
        private static Dictionary<Type, Page> _pageBag = new Dictionary<Type, Page>();

        public static Page GetPage(string pageName)
        {
            bool assemblyNotLoaded = false;
            int assembliesLoaded = 0;

            Type pageType = GetAssemblies(ref assemblyNotLoaded, ref assembliesLoaded).SelectMany(assembly => assembly.GetTypes()).FirstOrDefault(type => type.FullName.EndsWith(pageName));
            if (pageType != null)
            {
                return GetCachedPage(pageType);
            }

            // detect specific assembly load error and throw exception
            if (assemblyNotLoaded)
            {
                if (assembliesLoaded > 0)
                {
                    throw new Exception(string.Format("PageObjectNotFound: Assemblies found and loaded but do not contain page '{0}'.", pageName));
                }
                else
                {
                    throw new DllNotFoundException(string.Format("PageObjectNotLoaded: No loaded assembly contains pages and no assembly named PageObjects to load, page '{0}'.", pageName));
                }
            }
            throw new Exception(String.Format("PageObjectNotFound: Loaded assemblies do not contain page '{0}'.", pageName));
        }

        private static Page GetCachedPage(Type pageType)
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

        private static IEnumerable<Assembly> GetAssemblies(ref bool assemblyNotLoaded, ref int assembliesLoaded)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.IsDefined(typeof(AssemblyContainsPages)));
            if (assemblies.Count() == 0)
            {
                assemblyNotLoaded = true;
                foreach (var filePaths in Directory.GetFiles(Directory.GetCurrentDirectory(), "*PageObjects*.dll"))
                {
                    ++assembliesLoaded;
                    Assembly.LoadFrom(filePaths);
                }
                assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.IsDefined(typeof(AssemblyContainsPages)));
            }
            return assemblies;
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