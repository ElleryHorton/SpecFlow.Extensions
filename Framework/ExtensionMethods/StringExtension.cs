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
                return original.AppendFullHash(hashLength);
            }
            else if (original.HasHash(hashLength))
            {
                return original.RandomizeHash(hashLength);
            }
            else
            {
                return original.AppendFullHash(hashLength);
            }
        }

        public static string RandomizeShort(this string original, int hashLength = 8, string hashDelimit = "-")
        {
            if (string.IsNullOrEmpty(original))
            {
                return original.AppendShortHash(hashLength);
            }
            else if (original.HasHash(hashLength))
            {
                return original.RandomizeHash(hashLength);
            }
            else
            {
                return original.AppendShortHash(hashLength);
            }
        }

        private static string AppendFullHash(this string original, int hashLength = 4, string hashDelimit = "-")
        {
            return string.Format("{1}{2}{0}{3}{0}{4}", hashDelimit, GenerateTesterTestPrefix(hashDelimit), original, DateTime.Now.ToString("yyyyMMddHHmmssfff"), GenerateRandomHash(hashLength));
        }

        private static string AppendShortHash(this string original, int hashLength = 8, string hashDelimit = "-")
        {
            return string.Format("{1}{2}{0}{3}", hashDelimit, GenerateTesterTestPrefix(hashDelimit), original, GenerateRandomHash(hashLength));
        }

        private static string GenerateTesterTestPrefix(string hashDelimit)
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
            return prefix;
        }

        private static string RandomizeHash(this string hashString, int hashLength = 4, string hashDelimit = "-")
        {
            return hashString.Substring(0, (hashString.Length - (hashString.Length - hashString.LastIndexOf(hashDelimit)) + 1)) + GenerateRandomHash(hashLength);
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
