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

        public static Answer Answer(int? counter)
        {
            var rtValue = new Answer();
            var count = counter.HasValue ? counter.Value : 1;
            rtValue.Score = count % 5;
            rtValue.SurveyResponse = new SurveyResponse();
            rtValue.Response = new Response();
            rtValue.Question = new Question();
            rtValue.Category = new Category();

            return rtValue;

        }

        public static Category Category(int? counter)
        {
            var rtValue = new Category();
            rtValue.Survey = new Survey();
            rtValue.Name = "Name" + counter.Extra();
            rtValue.Affirmation = "Affirmation" + counter.Extra();
            rtValue.Encouragement = "Encouragement" + counter.Extra();

            return rtValue;
        }

        public static CategoryGoal CategoryGoal(int? counter)
        {
            var rtValue = new CategoryGoal();
            rtValue.Name = "Name" + counter.Extra();
            rtValue.Category = new Category();

            return rtValue;
        }

        public static Question Question(int? counter)
        {
            var rtValue = new Question();
            rtValue.Name = "Name" + counter.Extra();
            rtValue.Survey = new Survey();
            rtValue.Category = new Category();

            return rtValue;
        }
    }
}
