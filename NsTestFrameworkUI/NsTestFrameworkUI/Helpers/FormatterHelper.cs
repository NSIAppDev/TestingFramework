using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace NsTestFrameworkUI.Helpers
{
    public static class FormatterHelper
    {
        public static string CleanNumber(this string number)
        {
            return Regex.Replace(number.Trim().
                    Replace("$", string.Empty).
                    Replace(":", string.Empty).
                    Replace("#", string.Empty).
                    Replace("(", string.Empty).
                    Replace(")", string.Empty).
                    Replace("!", string.Empty).
                    Replace(",", string.Empty), "[a-zA-Z]+", string.Empty).
                    Replace("&", string.Empty).
                    Replace(" ", string.Empty).
                    Replace("/", string.Empty);
        }

        public static decimal ConvertStringToDecimal(this string value)
        {
            return value.Equals(string.Empty) ? 0 : decimal.Parse(value, NumberStyles.Currency);
        }

        public static int ConvertStringToInt32(this string value)
        {
            return Convert.ToInt32(value);
        }

        public static string Capitalize(this string @this)
        {
            return @this.Substring(0, 1).ToUpper() + @this.Substring(1);
        }

        public static string ConvertToValidCSharpPropertyName(this string str)
        {
            var arr = str.Where(c => (char.IsLetterOrDigit(c) || c == '_')).ToArray();

            var newStr = new string(arr);
            return newStr;
        }
    }
}
