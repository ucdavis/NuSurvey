using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NuSurvey.MVC.Helpers
{
    public static class SpanishHelper
    {
        public static bool IsSpanish(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            if (value.Trim().EndsWith("S", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
    }
}