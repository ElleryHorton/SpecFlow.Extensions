using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace SpecFlow.Extensions.Framework.ExtensionMethods
{
    public static class StringExtension
    {
        private static Random _random = new Random();

        public static string TestHash {get; set;}

        public static string TesterHash {get; set;}

        public static string Randomize(this string original, int hashLength = 4, string hashDelimit = "-")
        {
            if (string.IsNullOrEmpty(original))
            {
                return original.AppendHash();
            }
            else if (original.HasHash())
            {
                return original.RandomizeHash();
            }
            else
            {
                return original.AppendHash();
            }
        }

        private static string AppendHash(this string original, int hashLength = 4, string hashDelimit = "-")
        {
            string prefix = string.Empty;
            if (!string.IsNullOrEmpty(TesterHash))
            {
                prefix = string.Format("{0}{1}{2}", prefix, TesterHash, hashDelimit);
            }
            if (!string.IsNullOrEmpty(TestHash))
            {
                prefix = string.Format("{0}{1}{2}", prefix, TestHash, hashDelimit);
            }

            return string.Format("{1}{2}{0}{3}{0}{4}", hashDelimit, prefix, original, DateTime.Now.ToString("yyyyMMddHHmmssfff"), GenerateRandomHash());
        }

        private static string RandomizeHash(this string hashString, int hashLength = 4, string hashDelimit = "-")
        {
            return hashString.Substring(0, (hashString.Length - (hashString.Length - hashString.LastIndexOf(hashDelimit)) + 1)) + GenerateRandomHash();
        }

        public static string GenerateRandomHash(int hashLength = 4)
        {
            StringBuilder sb = new StringBuilder();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            for (int i = 0; i < hashLength; i++)
            {
                sb.Append(chars[_random.Next(chars.Length)]);
            }
            return sb.ToString();
        }

        private static bool HasHash(this string hashString, int hashLength = 4, string hashDelimit = "-")
        {
            return hashString.Contains(hashDelimit) && (hashString.LastIndexOf(hashDelimit) + hashDelimit.Length + hashLength == hashString.Length);
        }
    }
}
