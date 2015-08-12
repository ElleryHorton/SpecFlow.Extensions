using System;
using System.Collections.Generic;

namespace SpecFlow.Extensions.PageObjects
{
    public class ComparisonMismatch
    {
        private List<string> _mismatches = new List<string>();
        public IReadOnlyList<string> Mismatches { get { return _mismatches; } }

        public int Errors { get { return Mismatches.Count; } }

        public bool IsExact { get { return Mismatches.Count == 0; } }

        public void Add(string message)
        {
            _mismatches.Add(message);
        }

        public bool Compare<T> (T expected, T actual, Func<T, T, bool> compareMethod) where T : IComparable
        {
            return CompareInternal<T>(expected, actual, compareMethod,
                string.Format("{0}: expected '{1}' but was '{2}'", string.Format("expected: {0}   |   actual: {1}", expected.ToString(), actual.ToString())));
        }

        public bool Compare<T>(T expected, T actual, Func<T, T, bool> compareMethod, string message) where T : IComparable
        {
            return CompareInternal<T>(expected, actual, compareMethod,
                string.Format("{0}: expected '{1}' but was '{2}'", message, expected.ToString(), actual.ToString()));
        }

        private bool CompareInternal<T>(T expected, T actual, Func<T, T, bool> compareMethod, string message) where T : IComparable
        {
            bool isExact = compareMethod(expected, actual);
            if (!isExact)
            {
                _mismatches.Add(message);
            }
            return isExact;
        }
    }
}
