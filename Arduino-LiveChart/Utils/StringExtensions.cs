using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arduino_LiveChart.Utils
{
    static class StringExtensions
    {
        public static string GetBefore(this string value, char x)
        {
            int xPos = value.IndexOf(x);
            return xPos == -1 ? string.Empty : value.Substring(0, xPos);
        }

        public static string GetAfter(this string value, char x)
        {
            int xPos = value.IndexOf(x);

            if (xPos == -1) return string.Empty;

            int startIndex = xPos + 1;
            return startIndex >= value.Length ? string.Empty : value.Substring(startIndex);
        }


        public static string GetUntilOrEmpty(this string text, char stopAt)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt);

                if (charLocation > 0)
                {
                    return text.Substring(0, charLocation);
                }
            }

            return string.Empty;
        }

        public static string GetFromOrEmpty(this string text, char stopAt)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt);

                if (charLocation < text.Length - 1)
                {
                    return text.Substring(charLocation, text.Length-1);
                }
            }

            return string.Empty;
        }
    }
}
