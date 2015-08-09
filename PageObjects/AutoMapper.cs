using SpecFlow.Extensions.Framework.ExtensionMethods;
using SpecFlow.Extensions.Framework.Helpers;
using SpecFlow.Extensions.Web.ByWrappers;
using SpecFlow.Extensions.Web.ExtensionMethods;
using SpecFlow.Extensions.WebDriver.PortalDriver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow;

namespace SpecFlow.Extensions.PageObjects
{
    [Binding]
    public static class AutoMapper
    {
        public const string Delimiter = "!";
        public const string RandomMacro = "!Random";
        public const string RandomShortMacro = "!RandomShort";
        public const string RandomHashMacro = "!Hash";
        public const string DataMacro = "!Data";
        public const string EmailMacro = "!Email";
        public const string SkipMacro = "!Skip";

        public static IList<string> Headers;
        public static IList<IDictionary<string, string>> Rows;
        private static IPortalDriver _driver;

        public static void Fill(Table table, object context)
        {
            Fill(table, context, null, null);
        }

        public static void Fill(Table table, IPortalDriver driver, Page page)
        {
            Fill(table, null, driver, page);
        }

        public static void Fill(Table table, object context, IPortalDriver driver, Page page)
        {
            BuildCustomTable(table);
            IEnumerable<MemberInfo> pageByExs = null;
            if (driver != null && page != null)
            {
                pageByExs = GetByExs(page.GetType());
            }
            if (context == null)
            {
                if (pageByExs != null)
                {
                    SetPageMembers(driver, page, pageByExs);
                }
            }
            else
            {
                var contextMembers = GetMembers(context.GetType());
                if (pageByExs == null)
                {
                    SetContextMembers(context, contextMembers);
                }
                else
                {
                    SetContextAndPageMembers(driver, page, context, contextMembers, pageByExs);
                }
            }
        }

        public static void Fill(object objSource, object objDestination)
        {
            var sourceMembers = GetMembers(objSource.GetType());
            var destinationMembers = GetMembers(objDestination.GetType());
            foreach (var member in sourceMembers)
            {
                SetMember(objDestination, destinationMembers, member.Name, GetMemberValue(objSource, member).ToString());
            }
        }

        public static bool Verify(Table table, object context, Func<string, string, bool> compareMethod)
        {
            BuildCustomTable(table);
            return VerifyTableToObject(context, CompareTableToObject, compareMethod);
        }

        public static bool Verify(object context, Table table, Func<string, string, bool> compareMethod)
        {
            BuildCustomTable(table);
            return VerifyObjectToTable(context, CompareObjectMemberToTable, compareMethod);
        }

        public static bool Verify(Table table, Page page, IPortalDriver driver, Func<string, string, bool> compareMethod)
        {
            _driver = driver;
            BuildCustomTable(table);
            return VerifyTableToObject(page, CompareTableToPage, compareMethod);
        }

        public static bool Verify(Page page, Table table, IPortalDriver driver, Func<string, string, bool> compareMethod)
        {
            _driver = driver;
            BuildCustomTable(table);
            return VerifyObjectToTable(page, ComparePageElementToTable, compareMethod);
        }

        public static bool Verify(object context, Page page, IPortalDriver driver, Func<string, string, bool> compareMethod)
        {
            _driver = driver;
            return CompareMembersToMembers(context, page, GetMembers(context.GetType()), GetByExs(page.GetType()), compareMethod);
        }

        public static bool Verify(Page page, object context, IPortalDriver driver, Func<string, string, bool> compareMethod)
        {
            _driver = driver;
            return CompareMembersToMembers(context, page, GetByExs(page.GetType()), GetMembers(context.GetType()), compareMethod);
        }

        private static bool CompareMembersToMembers(object context, Page page,
            IEnumerable<MemberInfo> membersToLoop, IEnumerable<MemberInfo> membersToSearch,
            Func<string, string, bool> compareMethod)
        {
            foreach (var member in membersToLoop)
            {
                var foundMember = membersToSearch.FirstOrDefault(m => m.Name == member.Name);
                if (!ComparePageElementToTable(foundMember, page, GetMemberValue(context, member).ToString(), compareMethod))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool VerifyTableToObject(object obj, Func<object, string, string, Func<string, string, bool>, bool> compareMemberMethod,
            Func<string, string, bool> compareMethod)
        {
            foreach (var row in Rows)
            {
                foreach (var header in Headers)
                {
                    if (!compareMemberMethod(obj, header, row[header], compareMethod))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool VerifyObjectToTable(object obj, Func<MemberInfo, object, string, Func<string, string, bool>, bool> compareMemberMethod,
            Func<string, string, bool> compareMethod)
        {
            var members = GetMembers(obj.GetType());
            foreach (var row in Rows)
            {
                foreach (var member in members)
                {
                    if (!compareMemberMethod(member, obj, row[member.Name], compareMethod))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool CompareTableToObject(object context, string memberName, string value, Func<string, string, bool> compareMethod)
        {
            var members = GetMembers(context.GetType());
            var member = members.FirstOrDefault(m => m.Name == memberName);
            return CompareObjectMemberToTable(member, context, value, compareMethod);
        }

        private static bool CompareObjectMemberToTable(MemberInfo member, object context, string value, Func<string, string, bool> compareMethod)
        {
            return compareMethod(GetMemberValue(context, member).ToString(), value);
        }

        private static object GetMemberValue(object obj, MemberInfo member)
        {
            if (member is FieldInfo)
            {
                return ((FieldInfo)member).GetValue(obj);
            }
            else
            {
                return ((PropertyInfo)member).GetValue(obj);
            }
        }

        private static bool CompareTableToPage(object page, string memberName, string value, Func<string, string, bool> compareMethod)
        {
            var pageByExs = GetByExs(page.GetType());
            var member = pageByExs.FirstOrDefault(m => m.Name == memberName);
            return ComparePageElementToTable(member, page, value, compareMethod);
        }

        private static bool ComparePageElementToTable(MemberInfo member, object page, string value, Func<string, string, bool> compareMethod)
        {
            ByEx byEx = (ByEx)GetMemberValue(page, member);
            if (byEx.Input == Input.Select)
            {
                return compareMethod(_driver.FindSelect(byEx).Value(), value);
            }
            else
            {
                return compareMethod(_driver.Find(byEx).Value(), value);
            }
        }

        private static void SetContextAndPageMembers(IPortalDriver driver, Page page, object context, IEnumerable<MemberInfo> contextMembers, IEnumerable<MemberInfo> pageByExs)
        {
            foreach (var row in Rows)
            {
                foreach (var header in Headers)
                {
                    // context
                    SetMember(context, contextMembers, header, row[header]);
                    // pageObject
                    SetPageElement(driver, page, pageByExs, header, row[header]);
                }
            }
        }

        private static void SetContextMembers(object context, IEnumerable<MemberInfo> contextMembers)
        {
            foreach (var row in Rows)
            {
                foreach (var header in Headers)
                {
                    // context
                    SetMember(context, contextMembers, header, row[header]);
                }
            }
        }

        private static void SetPageMembers(IPortalDriver driver, Page page, IEnumerable<MemberInfo> pageByExs)
        {
            foreach (var row in Rows)
            {
                foreach (var header in Headers)
                {
                    // pageObject
                    SetPageElement(driver, page, pageByExs, header, row[header]);
                }
            }
        }

        private static void SetPageElement(IPortalDriver driver, Page page, IEnumerable<MemberInfo> pageByExs, string memberName, string value)
        {
            var member = pageByExs.FirstOrDefault(m => m.Name == memberName);
            if (member is FieldInfo)
                driver.Set((ByEx)((FieldInfo)member).GetValue(page), value);
            else
                driver.Set((ByEx)((PropertyInfo)member).GetValue(page), value);

        }

        private static void SetMember(object Context, IEnumerable<MemberInfo> contextMembers, string memberName, string value)
        {
            var member = contextMembers.FirstOrDefault(m => m.Name == memberName);
            if (member is FieldInfo)
                ((FieldInfo)member).SetValue(Context, value);
            else
                ((PropertyInfo)member).SetValue(Context, value);
        }

        private static IEnumerable<MemberInfo> GetMembers(Type type)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance).Cast<MemberInfo>();
            var props = type.GetProperties().Cast<MemberInfo>();
            return fields.Union(props);
        }

        private static IEnumerable<MemberInfo> GetByExs(Type type)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance).Where(member => member.FieldType == typeof(ByEx)).Cast<MemberInfo>();
            var props = type.GetProperties().Where(member => member.PropertyType == typeof(ByEx)).Cast<MemberInfo>();
            return fields.Union(props);
        }

        private static void BuildCustomTable(Table table)
        {
            Headers = new List<string>();
            Rows = new List<IDictionary<string, string>>();
            var LookupTable = new Dictionary<string, string>();
            foreach (var header in table.Header)
            {
                if (header.Contains(Delimiter))
                {
                    bool skip = false;
                    var substring = header.Substring(header.LastIndexOf(Delimiter));
                    switch (substring)
                    {
                        case RandomMacro:
                            foreach (var row in table.Rows)
                            {
                                row[header] = row[header].Randomize();
                            }
                            break;
                        case RandomShortMacro:
                            foreach (var row in table.Rows)
                            {
                                row[header] = row[header].RandomizeNoTimestamp();
                            }
                            break;
                        case RandomHashMacro:
                            foreach (var row in table.Rows)
                            {
                                row[header] = row[header].RandomizeHashOnly();
                            }
                            break;
                        case DataMacro:
                            foreach (var row in table.Rows)
                            {
                                row[header] = Tester.TestDataPath(row[header]);
                            }
                            break;
                        case EmailMacro:
                            foreach (var row in table.Rows)
                            {
                                row[header] = Tester.Email;
                            }
                            break;
                        case SkipMacro:
                            skip = true;
                            break;
                        default: // macro not supported, do nothing
                            break;
                    }
                    if (skip) // don't include in Headers
                    {
                        var newHeader = header.Replace(" ", string.Empty);
                        LookupTable.Add(header, newHeader);
                    }
                    else
                    {
                        var newHeader = header.Substring(0, header.Length - substring.Length)
                            .TrimEnd()
                            .Replace(" ", string.Empty);
                        Headers.Add(newHeader);
                        LookupTable.Add(header, newHeader);
                    }
                }
                else
                {
                    var newHeader = header.Replace(" ", string.Empty);
                    Headers.Add(newHeader);
                    LookupTable.Add(header, newHeader);
                }
            }
            foreach (var row in table.Rows)
            {
                var dict = new Dictionary<string, string>();
                foreach (var pair in row)
                {
                    dict.Add(LookupTable[pair.Key], pair.Value);
                }
                Rows.Add(dict);
            }
        }
    }
}