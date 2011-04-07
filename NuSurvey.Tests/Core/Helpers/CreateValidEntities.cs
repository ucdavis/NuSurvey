using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuSurvey.Core.Domain;

namespace NuSurvey.Tests.Core.Helpers
{
    public static class CreateValidEntities
    {
        #region Helper Extension

        private static string Extra(this int? counter)
        {
            var extraString = "";
            if (counter != null)
            {
                extraString = counter.ToString();
            }
            return extraString;
        }

        #endregion Helper Extension

        public static Survey Survey(int? counter)
        {
            var rtValue = new Survey();
            rtValue.Name = "Name" + counter.Extra();
            rtValue.IsActive = true;

            return rtValue;
        }
    }
}
