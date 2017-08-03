using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace BGP.Utils.Common
{
    public static class StringExtension
    {
        const string HTML_TAG_PATTERN = "<.*?>";
        public static string RemoveDiacritics(this String s)
        {
            String normalizedString = s.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < normalizedString.Length; i++)
            {
                Char c = normalizedString[i];
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }

        public static bool Contains(this string str, string value, bool accentSensitive, bool caseSensitive)
        {
            if (!caseSensitive)
            {
                str = str.ToUpper();
                value = value.ToUpper();
            }

            if (!accentSensitive)
            {
                str = str.RemoveDiacritics();
                value = value.RemoveDiacritics();
            }

            return str.Contains(value);
        }

        public static bool IsDigitOnly(this string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        public static string Truncate(this string input, int length)
        {
            if (input == null || input.Length < length)
                return input;
            int iNextSpace = input.LastIndexOf(" ", length);
            return string.Format("{0}...", input.Substring(0, (iNextSpace > 0) ? iNextSpace : length).Trim());
        }

        public static string StripHTML(this string input)
        {
            return Regex.Replace(input, HTML_TAG_PATTERN, string.Empty);
        }
    }
}
