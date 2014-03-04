using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NuSurvey.Web.Helpers
{
    public static class StringExtension
    {
        /// <summary>
        /// Returns the index of all occurances of a specific string
        /// </summary>
        /// <remarks>
        /// Code taken from: http://www.dijksterhuis.org/manipulating-strings-in-csharp-finding-all-occurrences-of-a-string-within-another-string/
        /// </remarks>
        /// <param name="source"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public static IEnumerable IndexOfAll(this string source, string searchTerm)
        {
            int pos, offset = 0;

            while ((pos = source.IndexOf(searchTerm)) > 0)
            {
                source = source.Substring(pos + searchTerm.Length);
                offset += pos;
                yield return offset;
            }
        }

        /// <summary>
        /// Try to parse a time like 6:20 into a float 6.333333333
        /// </summary>
        /// <param name="source">string with time h:mm</param>
        /// <param name="parsed"></param>
        /// <returns>true if successful</returns>
        public static bool TimeTryParse(this string source, out float parsed)
        {
            parsed = 0;
            try
            {
                if (string.IsNullOrWhiteSpace(source))
                {
                    return false;
                }
                if (!source.Contains(":"))
                {
                    return false;
                }
                if (source.Length < 4 || source.Length > 5)
                {
                    return false;
                }

                var hour = source.Split(':').ElementAt(0);
                int iHour;
                if (!int.TryParse(hour, out iHour))
                {
                    return false;
                }
                if (iHour > 12)
                {
                    return false;
                }
                if (iHour < 1)
                {
                    return false;
                }
                var minute = source.Split(':').ElementAt(1);
                int iMinute;
                if (!int.TryParse(minute, out iMinute))
                {
                    return false;
                }
                if (iMinute > 59)
                {
                    return false;
                }
                if (iMinute < 0)
                {
                    return false;
                }

                parsed = iHour + (iMinute / 60.0F);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool TimeTryParseAmPm(this string source, out float parsed)
        {
            parsed = 0;
            try
            {
                if (string.IsNullOrWhiteSpace(source))
                {
                    return false;
                }
                if (!source.Contains(":"))
                {
                    return false;
                }
                if (source.Length < 7 || source.Length > 8)
                {
                    return false;
                }
                if (!source.Contains("AM") && !source.Contains("PM"))
                {
                    return false;
                }

                var hour = source.Split(':').ElementAt(0);
                int iHour;
                if (!int.TryParse(hour, out iHour))
                {
                    return false;
                }
                if (iHour > 12)
                {
                    return false;
                }
                if (iHour < 1)
                {
                    return false;
                }
                var minute = source.Split(':').ElementAt(1).Substring(0,2);
                int iMinute;
                if (!int.TryParse(minute, out iMinute))
                {
                    return false;
                }
                if (iMinute > 59)
                {
                    return false;
                }
                if (iMinute < 0)
                {
                    return false;
                }

                if (source.Contains("AM"))
                {
                    if (iHour == 12)
                    {
                        iHour = 0;
                    }
                }
                if (source.Contains("PM"))
                {
                    if (iHour != 12)
                    {
                        iHour += 12;
                    }
                }

                parsed = iHour + (iMinute / 60.0F);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}