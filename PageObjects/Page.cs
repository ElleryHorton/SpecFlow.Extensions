using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.PageObjects;
using SpecFlow.Extensions.WebDriver;
using System;

namespace SpecFlow.Extensions.PageObjects
{
	public abstract class Page
	{
		private const string DllExtension = ".dll";
		private const string SamePageSubSectionDelimiter = "_";

		public string Uri
		{
			get
			{
				var assemblyName = System.Reflection.Assembly.GetAssembly(GetType()).ManifestModule.Name;
				assemblyName = assemblyName.Substring(0, assemblyName.Length - DllExtension.Length);
				int subsectionDivider = assemblyName.IndexOf(SamePageSubSectionDelimiter);
				if (subsectionDivider >= 0)
				{
					assemblyName = assemblyName.Substring(0, (assemblyName.Length - subsectionDivider) + 1);
				}
				return GetType().FullName.Replace(assemblyName, string.Empty).Replace(".", @"/");
			}
		}
	}
}
