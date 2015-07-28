using System;
using System.Linq;
using System.Text;

namespace SpecFlow.Extensions.Framework.ExtensionMethods
{
    public static class StringExtension
    {
        public static bool IsNumeric(this string expression)
        {
            double retNum;
            return Double.TryParse(Convert.ToString(expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
        }

        private static Random _random = new Random();

        public static string FeatureHash { get; set; }
        public static string ScenarioHash { get; set; }

        public static string TesterHash { get; set; }


        public static string Randomize(this string original, int hashLength = 4, string hashDelimit = "-")
        {
            return original.RandomizeSelector(AppendFullHash, hashLength, hashDelimit);
        }

        public static string RandomizeNoTimestamp(this string original, int hashLength = 4, string hashDelimit = "-")
        {
            return original.RandomizeSelector(AppendShortHash, hashLength, hashDelimit);
        }

        public static string RandomizeHashOnly(this string original, int hashLength = 4, string hashDelimit = "-")
        {
            return original.RandomizeSelector(AppendHashOnly, hashLength, hashDelimit);
        }

        private static string RandomizeSelector(this string original, Func<string, int, string, string> randomize, int hashLength, string hashDelimit)
        {
            if (string.IsNullOrEmpty(original))
            {
                return randomize(original, hashLength, hashDelimit);
            }
            else if (original.HasHash(hashLength, hashDelimit))
            {
                return original.RandomizeHash(hashLength, hashDelimit);
            }
            else
            {
                return randomize(original, hashLength, hashDelimit);
            }
        }

        private static string AppendFullHash(this string original, int hashLength, string hashDelimit)
        {
            return string.Format("{1}{2}{0}{3}{0}{4}", hashDelimit, original, GenerateTesterTestHash(hashDelimit), DateTime.Now.ToString("yyyyMMddHHmmssfff"), GenerateRandomHash(hashLength));
        }

        private static string AppendShortHash(this string original, int hashLength, string hashDelimit)
        {
            return string.Format("{1}{2}{0}{3}", hashDelimit, original, GenerateTesterTestHash(hashDelimit), GenerateRandomHash(hashLength));
        }

        private static string AppendHashOnly(this string original, int hashLength, string hashDelimit)
        {
            return string.Format("{1}{0}{2}", hashDelimit, original, GenerateRandomHash(hashLength));
        }

        private static string GenerateTesterTestHash(string hashDelimit)
        {
            string hash = string.Empty;
            if (!string.IsNullOrEmpty(TesterHash))
            {
                hash = string.Format("{1}{0}{2}", hashDelimit, hash, TesterHash);
            }
            if (!string.IsNullOrEmpty(FeatureHash))
            {
                hash = string.Format("{1}{0}{2}", hashDelimit, hash, FeatureHash);
            }
            if (!string.IsNullOrEmpty(ScenarioHash))
            {
                hash = string.Format("{1}{0}{2}", hashDelimit, hash, ScenarioHash);
            }
            return hash;
        }

        private static string RandomizeHash(this string hashString, int hashLength, string hashDelimit)
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

        private static bool HasHash(this string hashString, int hashLength, string hashDelimit)
        {
            return hashString.Contains(hashDelimit) && (hashString.LastIndexOf(hashDelimit) + hashDelimit.Length + hashLength == hashString.Length);
        }
    }
}
