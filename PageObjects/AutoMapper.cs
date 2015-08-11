﻿using SpecFlow.Extensions.Framework.ExtensionMethods;
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
        private static bool _ignoreMissingMembers;

        public static void Fill(Table table, object context, bool ignoreMissingMembers = true)
        {
            _ignoreMissingMembers = ignoreMissingMembers;
            BuildCustomTable(table);
            var contextMembers = GetMembers(context.GetType());
            SetMembers(context, contextMembers, SetMemberValue);
        }

        public static void Fill(Table table, IPortalDriver driver, Page page, bool ignoreMissingMembers = true)
        {
            _ignoreMissingMembers = ignoreMissingMembers;
            BuildCustomTable(table);
            _driver = driver;
            IList<MemberWrapper> pageByExs = GetMembersByEx(page.GetType());
            SetMembers(page, pageByExs, SetMemberValueByEx);
        }

        public static void Fill(Table table, object context, IPortalDriver driver, Page page, bool ignoreMissingMembers = true)
        {
            _ignoreMissingMembers = ignoreMissingMembers;
            BuildCustomTable(table);
            _driver = driver;
            IList<MemberWrapper> pageByExs = GetMembersByEx(page.GetType());
            var contextMembers = GetMembers(context.GetType());
            SetContextAndPageMembers(page, context, contextMembers, pageByExs);
        }

        public static void Fill(object objSource, object objDestination, bool ignoreMissingMembers = true)
        {
            _ignoreMissingMembers = ignoreMissingMembers;
            SetMembers(objSource, objDestination, GetMembers(objSource.GetType()), GetMembers(objDestination.GetType()),
                GetMemberValue, SetMemberValue);
        }

        public static void Fill(object obj, Page page, bool ignoreMissingMembers = true)
        {
            _ignoreMissingMembers = ignoreMissingMembers;
            SetMembers(obj, page, GetMembers(obj.GetType()), GetMembersByEx(page.GetType()),
                GetMemberValue, SetMemberValueByEx);
        }

        public static void Fill(Page page, object obj, bool ignoreMissingMembers = true)
        {
            _ignoreMissingMembers = ignoreMissingMembers;
            SetMembers(page, obj, GetMembersByEx(page.GetType()), GetMembers(obj.GetType()),
                GetMemberValueByEx, SetMemberValue);
        }

        public static bool Verify(Table table, object context, Func<string, string, bool> compareMethod, bool ignoreMissingMembers = true)
        {
            BuildCustomTable(table);
            return VerifyTableToObject(context, GetMembers, GetMemberValue, compareMethod);
        }

        public static bool Verify(object context, Table table, Func<string, string, bool> compareMethod, bool ignoreMissingMembers = true)
        {
            BuildCustomTable(table);
            return VerifyObjectToTable(context, GetMembers, GetMemberValue, compareMethod);
        }

        public static bool Verify(Table table, Page page, IPortalDriver driver, Func<string, string, bool> compareMethod, bool ignoreMissingMembers = true)
        {
            _driver = driver;
            BuildCustomTable(table);
            return VerifyTableToObject(page, GetMembersByEx, GetMemberValueByEx, compareMethod);
        }

        public static bool Verify(Page page, Table table, IPortalDriver driver, Func<string, string, bool> compareMethod, bool ignoreMissingMembers = true)
        {
            _driver = driver;
            BuildCustomTable(table);
            return VerifyObjectToTable(page, GetMembersByEx, GetMemberValueByEx, compareMethod);
        }

        public static bool Verify(object context, Page page, IPortalDriver driver, Func<string, string, bool> compareMethod, bool ignoreMissingMembers = true)
        {
            _driver = driver;
            return CompareMembersToMembers(context, page, GetMembers, GetMembersByEx, GetMemberValue, GetMemberValueByEx, compareMethod);
        }

        public static bool Verify(Page page, object context, IPortalDriver driver, Func<string, string, bool> compareMethod, bool ignoreMissingMembers = true)
        {
            _driver = driver;
            return CompareMembersToMembers(page, context, GetMembersByEx, GetMembers, GetMemberValueByEx, GetMemberValue, compareMethod);
        }

        private static bool CompareMembersToMembers(object objSource, object objDestination,
            Func<Type, IList<MemberWrapper>> getSourceMembers, Func<Type, IList<MemberWrapper>> getDestinationMembers,
            Func<object, MemberWrapper, object> getSourceMemberValue, Func<object, MemberWrapper, object> getDestinationMemberValue,
            Func<string, string, bool> compareMethod)
        {
            foreach (var member in getSourceMembers(objSource.GetType()))
            {
                var foundMember = getDestinationMembers(objDestination.GetType()).FirstOrDefault(m => m.Name == member.Name);
                if (foundMember == null)
                {
                    if (!_ignoreMissingMembers)
                    {
                        throw new MissingMemberException();
                    } // else do nothing
                }
                else
                {
                    if (!compareMethod(getSourceMemberValue(objSource, member).ToString(), getDestinationMemberValue(objDestination, foundMember).ToString()))
                    {
                        return false;
                    }
                }
                return true;
            }
            return true;
        }

        private static bool VerifyTableToObject(object obj,
            Func<Type, IList<MemberWrapper>> getMembers,
            Func<object, MemberWrapper, object> getMemberValue,
            Func<string, string, bool> compareMethod)
        {
            foreach (var row in Rows)
            {
                foreach (var header in Headers)
                {
                    var foundMember = getMembers(obj.GetType()).FirstOrDefault(m => m.Name == header);
                    if (foundMember == null)
                    {
                        if (!_ignoreMissingMembers)
                        {
                            throw new MissingMemberException();
                        } // else do nothing
                    }
                    else
                    {
                        if (!compareMethod(getMemberValue(obj, foundMember).ToString(), row[header]))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private static bool VerifyObjectToTable(object obj,
            Func<Type, IList<MemberWrapper>> getMembers,
            Func<object, MemberWrapper, object> getMemberValue,
            Func<string, string, bool> compareMethod)
        {
            var members = getMembers(obj.GetType());
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

        private static void SetContextAndPageMembers(Page page, object context, IList<MemberWrapper> contextMembers, IList<MemberWrapper> pageByExs)
        {
            foreach (var row in Rows)
            {
                foreach (var header in Headers)
                {
                    // context
                    SetMember(context, contextMembers, header, row[header], SetMemberValue);
                    // pageObject
                    SetMember(page, pageByExs, header, row[header], SetMemberValueByEx);
                }
            }
        }

        private static void SetMembers(object obj, IList<MemberWrapper> members, Action<MemberWrapper, object, string> setMemberValue)
        {
            foreach (var row in Rows)
            {
                foreach (var header in Headers)
                {
                    // context
                    SetMember(obj, members, header, row[header], setMemberValue);
                }
            }
        }

        private static void SetMembers(object objSource, object objDestination,
            IList<MemberWrapper> sourceMembers, IList<MemberWrapper> destinationMembers,
            Func<object, MemberWrapper, object> getMemberValue,
            Action<MemberWrapper, object, string> setMemberValue)
        {
            foreach (var member in sourceMembers)
            {
                SetMember(objDestination, destinationMembers, member.Name, (getMemberValue).ToString(), setMemberValue);
            }
        }

        private static void SetMember(object obj, IList<MemberWrapper> members, string memberName, string value, Action<MemberWrapper, object, string> setMember)
        {
            var foundMember = members.FirstOrDefault(m => m.Name == memberName);
            if (foundMember == null)
            {
                if (!_ignoreMissingMembers)
                {
                    throw new MissingMemberException();
                } // else do nothing
            }
            else
            {
                setMember(foundMember, obj, value);
            }
        }

        private static void SetMemberValueByEx(MemberWrapper member, object page, string value)
        {
            _driver.Set((ByEx)(member.GetValue(page)), value);
        }

        private static void SetMemberValue(MemberWrapper member, object obj, string value)
        {
            member.SetValue(obj, value);
        }

        private static IList<MemberWrapper> GetMembersByEx(Type type)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance).Where(member => member.FieldType == typeof(ByEx)).ToList();
            var props = type.GetProperties().Where(member => member.PropertyType == typeof(ByEx)).ToList();
            return WrapMembers(fields, props);
        }

        private static IList<MemberWrapper> GetMembers(Type type)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
            var props = type.GetProperties().ToList();
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