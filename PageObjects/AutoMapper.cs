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
            IList<MemberWrapper> pageByExs = null;
            if (driver != null && page != null)
            {
                pageByExs = GetMembersByEx(page);
            }
            if (context == null)
            {
                if (pageByExs != null)
                {
                    SetMembers(page, pageByExs, SetPageElement);
                }
            }
            else
            {
                var contextMembers = GetMembers(context);
                if (pageByExs == null)
                {
                    SetMembers(context, contextMembers, SetMember);
                }
                else
                {
                    SetContextAndPageMembers(driver, page, context, contextMembers, pageByExs);
                }
            }
        }

        public static void Fill(object objSource, object objDestination)
        {
            var sourceMembers = GetMembers(objSource);
            var destinationMembers = GetMembers(objDestination);
            foreach (var member in sourceMembers)
            {
                SetMember(objDestination, destinationMembers, member.Name, GetMemberValue(objSource, member).ToString());
            }
        }

        public static bool Verify(Table table, object context, Func<string, string, bool> compareMethod)
        {
            BuildCustomTable(table);
            return VerifyTableToObject(context, compareMethod, GetMembers, GetMemberValue);
        }

        public static bool Verify(object context, Table table, Func<string, string, bool> compareMethod)
        {
            BuildCustomTable(table);
            return VerifyObjectToTable(context, compareMethod, GetMembers, GetMemberValue);
        }

        public static bool Verify(Table table, Page page, IPortalDriver driver, Func<string, string, bool> compareMethod)
        {
            _driver = driver;
            BuildCustomTable(table);
            return VerifyTableToObject(page, compareMethod, GetMembersByEx, GetMemberValueByEx);
        }

        public static bool Verify(Page page, Table table, IPortalDriver driver, Func<string, string, bool> compareMethod)
        {
            _driver = driver;
            BuildCustomTable(table);
            return VerifyObjectToTable(page, compareMethod, GetMembersByEx, GetMemberValueByEx);
        }

        public static bool Verify(object context, Page page, IPortalDriver driver, Func<string, string, bool> compareMethod)
        {
            _driver = driver;
            return CompareMembersToMembers(context, page, GetMembers(context), GetMembersByEx(page), compareMethod);
        }

        public static bool Verify(Page page, object context, IPortalDriver driver, Func<string, string, bool> compareMethod)
        {
            _driver = driver;
            return CompareMembersToMembers(context, page, GetMembersByEx(page), GetMembers(context), compareMethod);
        }

        private static bool CompareMembersToMembers(object context, Page page,
            IList<MemberWrapper> membersToLoop, IList<MemberWrapper> membersToSearch,
            Func<string, string, bool> compareMethod)
        {
            foreach (var member in membersToLoop)
            {
                var foundMember = membersToSearch.FirstOrDefault(m => m.Name == member.Name);
                if (!compareMethod(GetMemberValueByEx(page, foundMember).ToString(), GetMemberValue(context, member).ToString()))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool VerifyTableToObject(object obj, Func<string, string, bool> compareMethod,
            Func<object, IList<MemberWrapper>> getMembers, Func<object, MemberWrapper, object> getMemberValue)
        {
            foreach (var row in Rows)
            {
                foreach (var header in Headers)
                {
                    var member = getMembers(obj).FirstOrDefault(m => m.Name == header);
                    if (!compareMethod(getMemberValue(obj, member).ToString(), row[header]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool VerifyObjectToTable(object obj, Func<string, string, bool> compareMethod,
            Func<object, IList<MemberWrapper>> getMembers, Func<object, MemberWrapper, object> getMemberValue)
        {
            var members = getMembers(obj);
            foreach (var row in Rows)
            {
                foreach (var member in members)
                {
                    if (!compareMethod(getMemberValue(obj, member).ToString(), row[member.Name]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static object GetMemberValue(object obj, MemberWrapper member)
        {
            return member.GetValue(obj);
        }

        private static object GetMemberValueByEx(object page, MemberWrapper member)
        {
            ByEx byEx = (ByEx)GetMemberValue(page, member);
            if (byEx.Input == Input.Select)
            {
                return _driver.FindSelect(byEx).Value();
            }
            else
            {
                return _driver.Find(byEx).Value();
            }
        }

        private static void SetContextAndPageMembers(IPortalDriver driver, Page page, object context, IList<MemberWrapper> contextMembers, IList<MemberWrapper> pageByExs)
        {
            foreach (var row in Rows)
            {
                foreach (var header in Headers)
                {
                    // context
                    SetMember(context, contextMembers, header, row[header]);
                    // pageObject
                    SetPageElement(page, pageByExs, header, row[header]);
                }
            }
        }

        private static void SetMembers(object obj, IList<MemberWrapper> members, Action<object, IList<MemberWrapper>, string, string> setMember)
        {
            foreach (var row in Rows)
            {
                foreach (var header in Headers)
                {
                    // context
                    setMember(obj, members, header, row[header]);
                }
            }
        }

        private static void SetPageElement(object page, IList<MemberWrapper> members, string memberName, string value)
        {
            var member = members.FirstOrDefault(m => m.Name == memberName);
            _driver.Set((ByEx)(member.GetValue(page)), value);
        }

        private static void SetMember(object obj, IList<MemberWrapper> members, string memberName, string value)
        {
            var member = members.FirstOrDefault(m => m.Name == memberName);
            member.SetValue(obj, value);
        }

        private static IList<MemberWrapper> GetMembersByEx(object page)
        {
            var fields = page.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance).Where(member => member.FieldType == typeof(ByEx)).ToList();
            var props = page.GetType().GetProperties().Where(member => member.PropertyType == typeof(ByEx)).ToList();
            return WrapMembers(fields, props);
        }

        private static IList<MemberWrapper> GetMembers(object obj)
        {
            var fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
            var props = obj.GetType().GetProperties().ToList();
            return WrapMembers(fields, props);
        }

        private static List<MemberWrapper> WrapMembers(List<FieldInfo> fields, List<PropertyInfo> props)
        {
            List<MemberWrapper> members = new List<MemberWrapper>();
            fields.ForEach(f => members.Add(new FieldWrapper(f)));
            props.ForEach(p => members.Add(new PropertyWrapper(p)));
            return members;
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