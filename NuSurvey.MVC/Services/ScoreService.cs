using System.Collections.Generic;
using System.Linq;
using NuSurvey.Core.Domain;
using NuSurvey.MVC.Controllers;
using NuSurvey.MVC.Helpers;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace NuSurvey.MVC.Services
{
    public interface IScoreService
    {
        QuestionAnswerParameter ScoreQuestion(IQueryable<Question> questions, QuestionAnswerParameter questionAnswerParameter);
        void CalculateScores(IRepository repository, SurveyResponse surveyResponse);        
    }

    public class ScoreService : IScoreService
    {
        /// <summary>
        /// Score question and check for validity
        /// If an answer is invalid, the Value QuestionAnswerParameter.Invalid will be true and the QuestionAnswerParameter.Message will have the error.
        /// </summary>
        /// <param name="questions"></param>
        /// <param name="questionAnswerParameter"></param>
        /// <returns></returns>
        public QuestionAnswerParameter ScoreQuestion(IQueryable<Question> questions, QuestionAnswerParameter questionAnswerParameter)
        {
            questionAnswerParameter.Score = 0;
            questionAnswerParameter.Invalid = false;
            questionAnswerParameter.Message = string.Empty;
            questionAnswerParameter.OpenEndedNumericAnswer = null;

            var question = questions.Where(a => a.Id == questionAnswerParameter.QuestionId).Single();
            if (question.IsOpenEnded)
            {
                #region Open Ended Question
                if (string.IsNullOrWhiteSpace(questionAnswerParameter.Answer))
                {
                    questionAnswerParameter.Invalid = true;
                    questionAnswerParameter.Message = "Answer is required";
                    questionAnswerParameter.Score = 0;
                }
                else
                {
                    int? responseId = null;
                    switch ((QuestionType)question.OpenEndedQuestionType)
                    {
                        case QuestionType.WholeNumber:
                            #region Old Way  Open Ended Int Question
                            //int number;                            
                            //if (int.TryParse(questionAnswerParameter.Answer, out number))
                            //{
                            //    questionAnswerParameter.OpenEndedNumericAnswer = number;

                            //    #region Exact Match
                            //    var response = question.Responses.Where(a => a.IsActive && a.Value == number.ToString()).FirstOrDefault();
                            //    if (response != null)
                            //    {
                            //        responseId = response.Id;
                            //    }
                            //    #endregion Exact Match

                            //    #region High Value
                            //    if (responseId == null)
                            //    {
                            //        //Check For High Value
                            //        response =
                            //            question.Responses.Where(a => a.IsActive && a.Value.Contains("+")).FirstOrDefault();
                            //        if (response != null)
                            //        {
                            //            int highValue;
                            //            if (Int32.TryParse(response.Value, out highValue))
                            //            {
                            //                if (number >= highValue)
                            //                {
                            //                    responseId = response.Id;
                            //                }
                            //            }
                            //        }
                            //    }
                            //    #endregion High Value

                            //    #region Low Value
                            //    if (responseId == null)
                            //    {
                            //        //Check for low value
                            //        response = question.Responses.Where(a => a.IsActive && a.Value.Contains("-")).FirstOrDefault();
                            //        if (response != null)
                            //        {
                            //            int lowValue;
                            //            if (Int32.TryParse(response.Value, out lowValue))
                            //            {
                            //                if (number <= Math.Abs(lowValue))
                            //                {
                            //                    responseId = response.Id;
                            //                }
                            //            }
                            //        }
                            //    }
                            //    #endregion Low Value

                            //    //Found an exact match, or a high/low score
                            //    if (responseId != null)
                            //    {
                            //        questionAnswerParameter.ResponseId = responseId.Value;
                            //        questionAnswerParameter.Score = question.Responses.Where(a => a.Id == responseId.Value).Single().Score;
                            //    }
                            //    else
                            //    {
                            //        //questionAnswerParameter.Invalid = true;
                            //        questionAnswerParameter.Message = "Matching Value to score not found";
                            //        questionAnswerParameter.Score = 0;
                            //    }
                            //}
                            //else
                            //{
                            //    questionAnswerParameter.Invalid = true;
                            //    questionAnswerParameter.Message = "Answer must be a whole number";
                            //    questionAnswerParameter.Score = 0;
                            //}
                            #endregion Old Way Open Ended Int Question

                            //If it finds an exact match it uses that.
                            //If the passed value is less than smallest response, it uses the smallest response
                            //if the passed value is greater than the largest response, it uses the largest response
                            //If the passed value is between two responses, it uses the one that it is closer to
                            //... if exactly in the middle, it chooses the one with the higher score.
                            #region Open Ended Int Question
                            int number;
                            if (int.TryParse(questionAnswerParameter.Answer, out number))
                            {
                                var intDict = new Dictionary<int, int>();
                                int? high = null;

                                //Get a list of all active responses don't use any that can't be changed into a float.
                                foreach (var response in question.Responses.Where(a => a.IsActive))
                                {
                                    int tempInt;
                                    if (int.TryParse(response.Value, out tempInt))
                                    {
                                        intDict.Add(tempInt, response.Id);
                                    }
                                }

                                //Sort the valid responses.
                                var sortedIntDict = intDict.OrderBy(a => a.Key);
                                for (int i = 0; i < sortedIntDict.Count(); i++)
                                {
                                    if (number == sortedIntDict.ElementAt(i).Key)
                                    {
                                        responseId = sortedIntDict.ElementAt(i).Value;
                                        break;
                                    }
                                    if (number < sortedIntDict.ElementAt(i).Key)
                                    {
                                        high = i;
                                        break;
                                    }
                                }

                                //Didn't find an exact match
                                if (responseId == null)
                                {
                                    if (high != null)
                                    {
                                        if (high.Value == 0) //Use the lowest value
                                        {
                                            responseId = sortedIntDict.ElementAt(high.Value).Value;
                                        }
                                        else //Somewhere inbetween. pick closest one, or one with highest score
                                        {
                                            var lowDiff = number - sortedIntDict.ElementAt(high.Value - 1).Key;
                                            var highDiff = sortedIntDict.ElementAt(high.Value).Key - number;
                                            if (lowDiff == highDiff)
                                            {
                                                if (question.Responses.Where(a => a.Id == sortedIntDict.ElementAt(high.Value - 1).Value).Single().Score > question.Responses.Where(a => a.Id == sortedIntDict.ElementAt(high.Value).Value).Single().Score)
                                                {
                                                    responseId = sortedIntDict.ElementAt(high.Value - 1).Value;
                                                }
                                                else
                                                {
                                                    responseId = sortedIntDict.ElementAt(high.Value).Value;
                                                }
                                            }
                                            else if (lowDiff < highDiff)
                                            {
                                                responseId = sortedIntDict.ElementAt(high.Value - 1).Value;
                                            }
                                            else
                                            {
                                                responseId = sortedIntDict.ElementAt(high.Value).Value;
                                            }
                                        }
                                    }
                                    else if (sortedIntDict.Count() > 0) //Use the highest value
                                    {
                                        responseId = sortedIntDict.ElementAt(sortedIntDict.Count() - 1).Value;
                                    }
                                }




                                //Found an exact match, or a high/low score
                                if (responseId != null)
                                {
                                    questionAnswerParameter.ResponseId = responseId.Value;
                                    questionAnswerParameter.Score = question.Responses.Where(a => a.Id == responseId.Value).Single().Score;
                                }
                                else
                                {
                                    //questionAnswerParameter.Invalid = true;
                                    questionAnswerParameter.Message = "Matching Value to score not found";
                                    questionAnswerParameter.Score = 0;
                                }
                            }
                            else
                            {
                                questionAnswerParameter.Invalid = true;
                                questionAnswerParameter.Message = "Answer must be a whole number";
                                questionAnswerParameter.Score = 0;
                            }
                            #endregion Open Ended Int Question
                            break;

                        case QuestionType.Decimal:
                            //If it finds an exact match it uses that.
                            //If the passed value is less than smallest response, it uses the smallest response
                            //if the passed value is greater than the largest response, it uses the largest response
                            //If the passed value is between two responses, it uses the one that it is closer to
                            //... if exactly in the middle, it chooses the one with the higher score.
                            #region Open Ended Decimal Question
                            float floatNumber;
                            if (float.TryParse(questionAnswerParameter.Answer, out floatNumber))
                            {
                                var floatDict = new Dictionary<float, int>();
                                int? high = null;

                                //Get a list of all active responses don't use any that can't be changed into a float.
                                foreach (var response in question.Responses.Where(a => a.IsActive))
                                {
                                    float tempFloat;
                                    if (float.TryParse(response.Value, out tempFloat))
                                    {
                                        floatDict.Add(tempFloat, response.Id);
                                    }
                                }

                                //Sort the valid responses.
                                var sortedDict = floatDict.OrderBy(a => a.Key);
                                for (int i = 0; i < sortedDict.Count(); i++)
                                {
                                    if (floatNumber == sortedDict.ElementAt(i).Key)
                                    {
                                        responseId = sortedDict.ElementAt(i).Value;
                                        break;
                                    }
                                    if (floatNumber < sortedDict.ElementAt(i).Key)
                                    {
                                        high = i;
                                        break;
                                    }
                                }

                                //Didn't find an exact match
                                if (responseId == null)
                                {
                                    if (high != null)
                                    {
                                        if (high.Value == 0) //Use the lowest value
                                        {
                                            responseId = sortedDict.ElementAt(high.Value).Value;
                                        }
                                        else //Somewhere inbetween. pick closest one, or one with highest score
                                        {
                                            var lowDiff = floatNumber - sortedDict.ElementAt(high.Value - 1).Key;
                                            var highDiff = sortedDict.ElementAt(high.Value).Key - floatNumber;
                                            if (lowDiff == highDiff)
                                            {
                                                if (question.Responses.Where(a => a.Id == sortedDict.ElementAt(high.Value - 1).Value).Single().Score > question.Responses.Where(a => a.Id == sortedDict.ElementAt(high.Value).Value).Single().Score)
                                                {
                                                    responseId = sortedDict.ElementAt(high.Value - 1).Value;
                                                }
                                                else
                                                {
                                                    responseId = sortedDict.ElementAt(high.Value).Value;
                                                }
                                            }
                                            else if (lowDiff < highDiff)
                                            {
                                                responseId = sortedDict.ElementAt(high.Value - 1).Value;
                                            }
                                            else
                                            {
                                                responseId = sortedDict.ElementAt(high.Value).Value;
                                            }
                                        }
                                    } else if (sortedDict.Count() > 0) //Use the highest value
                                    {
                                        responseId = sortedDict.ElementAt(sortedDict.Count()-1).Value;
                                    }
                                }




                                //Found an exact match, or a high/low score
                                if (responseId != null)
                                {
                                    questionAnswerParameter.ResponseId = responseId.Value;
                                    questionAnswerParameter.Score = question.Responses.Where(a => a.Id == responseId.Value).Single().Score;
                                }
                                else
                                {
                                    //questionAnswerParameter.Invalid = true;
                                    questionAnswerParameter.Message = "Matching Value to score not found";
                                    questionAnswerParameter.Score = 0;
                                }
                            }
                            else
                            {
                                questionAnswerParameter.Invalid = true;
                                questionAnswerParameter.Message = "Answer must be a number (decimal ok)";
                                questionAnswerParameter.Score = 0;
                            }
                            #endregion Open Ended Decimal Question
                            break;

                        case QuestionType.Time:
                            //Works the same way as the Decimal above by first converting the time into a float.
                            #region Open Ended Time Question
                            float floatTime;
                            if (questionAnswerParameter.Answer.TimeTryParse(out floatTime))
                            {
                                var floatDict = new Dictionary<float, int>();
                                int? high = null;

                                //Get a list of all active responses don't use any that can't be changed into a float.
                                foreach (var response in question.Responses.Where(a => a.IsActive))
                                {
                                    float tempFloat;
                                    if (response.Value.TimeTryParse(out tempFloat))
                                    {
                                        floatDict.Add(tempFloat, response.Id);
                                    }
                                }

                                //Sort the valid responses.
                                var sortedDict = floatDict.OrderBy(a => a.Key);
                                for (int i = 0; i < sortedDict.Count(); i++)
                                {
                                    if (floatTime == sortedDict.ElementAt(i).Key)
                                    {
                                        responseId = sortedDict.ElementAt(i).Value;
                                        break;
                                    }
                                    if (floatTime < sortedDict.ElementAt(i).Key)
                                    {
                                        high = i;
                                        break;
                                    }
                                }

                                //Didn't find an exact match
                                if (responseId == null)
                                {
                                    if (high != null)
                                    {
                                        if (high.Value == 0) //Use the lowest value
                                        {
                                            responseId = sortedDict.ElementAt(high.Value).Value;
                                        }
                                        else //Somewhere inbetween. pick closest one, or one with highest score
                                        {
                                            var lowDiff = floatTime - sortedDict.ElementAt(high.Value - 1).Key;
                                            var highDiff = sortedDict.ElementAt(high.Value).Key - floatTime;
                                            if (lowDiff == highDiff)
                                            {
                                                if (question.Responses.Where(a => a.Id == sortedDict.ElementAt(high.Value - 1).Value).Single().Score > question.Responses.Where(a => a.Id == sortedDict.ElementAt(high.Value).Value).Single().Score)
                                                {
                                                    responseId = sortedDict.ElementAt(high.Value - 1).Value;
                                                }
                                                else
                                                {
                                                    responseId = sortedDict.ElementAt(high.Value).Value;
                                                }
                                            }
                                            else if (lowDiff < highDiff)
                                            {
                                                responseId = sortedDict.ElementAt(high.Value - 1).Value;
                                            }
                                            else
                                            {
                                                responseId = sortedDict.ElementAt(high.Value).Value;
                                            }
                                        }
                                    } else if (sortedDict.Count() > 0) //Use the highest value
                                    {
                                        responseId = sortedDict.ElementAt(sortedDict.Count()-1).Value;
                                    }
                                }


                                //Found an exact match, or a high/low score
                                if (responseId != null)
                                {
                                    questionAnswerParameter.ResponseId = responseId.Value;
                                    questionAnswerParameter.Score = question.Responses.Where(a => a.Id == responseId.Value).Single().Score;
                                }
                                else
                                {
                                    //questionAnswerParameter.Invalid = true;
                                    questionAnswerParameter.Message = "Matching Value to score not found";
                                    questionAnswerParameter.Score = 0;
                                }
                            }
                            else
                            {
                                questionAnswerParameter.Invalid = true;
                                questionAnswerParameter.Message = "Answer must be a Time (hh:mm)";
                                questionAnswerParameter.Score = 0;
                            }
                            #endregion Open Ended Time Question

                            break;

                        case QuestionType.TimeAmPm:
                            //Works the same way as the Decimal above by first converting the time into a float.
                            #region Open Ended Time AM/PM Question
                            float floatTimeAmPm;
                            if (questionAnswerParameter.Answer.TimeTryParseAmPm(out floatTimeAmPm))
                            {
                                var floatDict = new Dictionary<float, int>();
                                int? high = null;

                                //Get a list of all active responses don't use any that can't be changed into a float.
                                foreach (var response in question.Responses.Where(a => a.IsActive))
                                {
                                    float tempFloat;
                                    if (response.Value.TimeTryParseAmPm(out tempFloat))
                                    {
                                        floatDict.Add(tempFloat, response.Id);
                                    }
                                }

                                //Sort the valid responses.
                                var sortedDict = floatDict.OrderBy(a => a.Key);
                                for (int i = 0; i < sortedDict.Count(); i++)
                                {
                                    if (floatTimeAmPm == sortedDict.ElementAt(i).Key)
                                    {
                                        responseId = sortedDict.ElementAt(i).Value;
                                        break;
                                    }
                                    if (floatTimeAmPm < sortedDict.ElementAt(i).Key)
                                    {
                                        high = i;
                                        break;
                                    }
                                }

                                //Didn't find an exact match
                                if (responseId == null)
                                {
                                    if (high != null)
                                    {
                                        if (high.Value == 0) //Use the lowest value
                                        {
                                            responseId = sortedDict.ElementAt(high.Value).Value;
                                        }
                                        else //Somewhere inbetween. pick closest one, or one with highest score
                                        {
                                            var lowDiff = floatTimeAmPm - sortedDict.ElementAt(high.Value - 1).Key;
                                            var highDiff = sortedDict.ElementAt(high.Value).Key - floatTimeAmPm;
                                            if (lowDiff == highDiff)
                                            {
                                                if (question.Responses.Where(a => a.Id == sortedDict.ElementAt(high.Value - 1).Value).Single().Score > question.Responses.Where(a => a.Id == sortedDict.ElementAt(high.Value).Value).Single().Score)
                                                {
                                                    responseId = sortedDict.ElementAt(high.Value - 1).Value;
                                                }
                                                else
                                                {
                                                    responseId = sortedDict.ElementAt(high.Value).Value;
                                                }
                                            }
                                            else if (lowDiff < highDiff)
                                            {
                                                responseId = sortedDict.ElementAt(high.Value - 1).Value;
                                            }
                                            else
                                            {
                                                responseId = sortedDict.ElementAt(high.Value).Value;
                                            }
                                        }
                                    }
                                    else if (sortedDict.Count() > 0) //Use the highest value
                                    {
                                        responseId = sortedDict.ElementAt(sortedDict.Count() - 1).Value;
                                    }
                                }


                                //Found an exact match, or a high/low score
                                if (responseId != null)
                                {
                                    questionAnswerParameter.ResponseId = responseId.Value;
                                    questionAnswerParameter.Score = question.Responses.Where(a => a.Id == responseId.Value).Single().Score;
                                }
                                else
                                {
                                    //questionAnswerParameter.Invalid = true;
                                    questionAnswerParameter.Message = "Matching Value to score not found";
                                    questionAnswerParameter.Score = 0;
                                }
                            }
                            else
                            {
                                questionAnswerParameter.Invalid = true;
                                questionAnswerParameter.Message = "Answer must be a Time (hh:mm AM/PM)";
                                questionAnswerParameter.Score = 0;
                            }
                            #endregion Open Ended Time AM/PM Question

                            break;

                        case QuestionType.TimeRange:
                            //Works the same way as the Decimal above by first converting the time into a float.
                            #region Open Ended Time Range Question
                            var timeRangeValid = true;
                            float startTime;
                            float endTime;
                            float ajustedDifference;
                            if (!questionAnswerParameter.Answer.TimeTryParseAmPm(out startTime))
                            {
                                timeRangeValid = false;
                            }
                            if (!questionAnswerParameter.AnswerRange.TimeTryParseAmPm(out endTime))
                            {
                                timeRangeValid = false;
                            }

                            if(timeRangeValid)
                            {
                                if(startTime > endTime)
                                {
                                    endTime += 24; //Add a day.
                                }
                                ajustedDifference = endTime - startTime;

                                var floatDict = new Dictionary<float, int>();
                                int? high = null;

                                //Get a list of all active responses don't use any that can't be changed into a float.
                                foreach (var response in question.Responses.Where(a => a.IsActive))
                                {
                                    float tempFloat;
                                    if (float.TryParse(response.Value, out tempFloat))
                                    {
                                        floatDict.Add(tempFloat, response.Id);
                                    }
                                }

                                                                //Sort the valid responses.
                                var sortedDict = floatDict.OrderBy(a => a.Key);
                                for (int i = 0; i < sortedDict.Count(); i++)
                                {
                                    if (ajustedDifference == sortedDict.ElementAt(i).Key)
                                    {
                                        responseId = sortedDict.ElementAt(i).Value;
                                        break;
                                    }
                                    if (ajustedDifference < sortedDict.ElementAt(i).Key)
                                    {
                                        high = i;
                                        break;
                                    }
                                }

                                //Didn't find an exact match
                                if (responseId == null)
                                {
                                    if (high != null)
                                    {
                                        if (high.Value == 0) //Use the lowest value
                                        {
                                            responseId = sortedDict.ElementAt(high.Value).Value;
                                        }
                                        else //Somewhere inbetween. pick closest one, or one with highest score
                                        {
                                            var lowDiff = ajustedDifference - sortedDict.ElementAt(high.Value - 1).Key;
                                            var highDiff = sortedDict.ElementAt(high.Value).Key - ajustedDifference;
                                            if (lowDiff == highDiff)
                                            {
                                                if (question.Responses.Where(a => a.Id == sortedDict.ElementAt(high.Value - 1).Value).Single().Score > question.Responses.Where(a => a.Id == sortedDict.ElementAt(high.Value).Value).Single().Score)
                                                {
                                                    responseId = sortedDict.ElementAt(high.Value - 1).Value;
                                                }
                                                else
                                                {
                                                    responseId = sortedDict.ElementAt(high.Value).Value;
                                                }
                                            }
                                            else if (lowDiff < highDiff)
                                            {
                                                responseId = sortedDict.ElementAt(high.Value - 1).Value;
                                            }
                                            else
                                            {
                                                responseId = sortedDict.ElementAt(high.Value).Value;
                                            }
                                        }
                                    }
                                    else if (sortedDict.Count() > 0) //Use the highest value
                                    {
                                        responseId = sortedDict.ElementAt(sortedDict.Count() - 1).Value;
                                    }
                                }

                                //Found an exact match, or a high/low score
                                if (responseId != null)
                                {
                                    questionAnswerParameter.ResponseId = responseId.Value;
                                    questionAnswerParameter.Score = question.Responses.Where(a => a.Id == responseId.Value).Single().Score;
                                }
                                else
                                {
                                    //questionAnswerParameter.Invalid = true;
                                    questionAnswerParameter.Message = "Matching Value to score not found";
                                    questionAnswerParameter.Score = 0;
                                }

                            }
                            else
                            {
                                questionAnswerParameter.Invalid = true;
                                questionAnswerParameter.Message = "Answers must be a Time (hh:mm AM/PM)";
                                questionAnswerParameter.Score = 0;
                            }

                            #endregion Open Ended Time Range Question
                            break;

                        default:
                            Check.Require(false, string.Format("Unknown QuestionType encountered: '{0}'", question.OpenEndedQuestionType));
                            break;
                    }

                }
                #endregion Open Ended Question
            }
            else
            {
                #region Radio Button Question
                var response = question.Responses.Where(a => a.Id == questionAnswerParameter.ResponseId && a.IsActive).FirstOrDefault();
                if (response == null)
                {
                    questionAnswerParameter.Invalid = true;
                    questionAnswerParameter.Message = "Answer is required";
                    questionAnswerParameter.Score = 0;
                }
                else
                {
                    questionAnswerParameter.Score = response.Score;
                }
                #endregion Radio Button Question
            }

            if (question.Category.DoNotUseForCalculations)
            {
                questionAnswerParameter.Score = 0;
            }

            return questionAnswerParameter;
        }

        public void CalculateScores(IRepository repository, SurveyResponse surveyResponse)
        {
            var bypassedAnswers = surveyResponse.Answers.Where(a => a.BypassScore);
            
            var scores = new List<Scores>();
            foreach (var category in surveyResponse.Survey.Categories.Where(a => !a.DoNotUseForCalculations && a.IsActive && a.IsCurrentVersion))
            {
                var score = new Scores();
                score.Category = category;
                var totalMax = repository.OfType<CategoryTotalMaxScore>().GetNullableById(category.Id);
                if (totalMax == null) //No Questions most likely
                {
                    continue;
                }
                score.MaxScore = totalMax.TotalMaxScore;
                Category category1 = category;
                foreach (var bypassedAnswer in bypassedAnswers.Where(a => a.Category == category1))
                {
                    score.MaxScore = score.MaxScore - bypassedAnswer.Question.Responses.Where(a => a.IsActive).Max(a => a.Score);
                }
                Category category2 = category;
                score.TotalScore =
                    surveyResponse.Answers.Where(a => a.Category == category2).Sum(b => b.Score);
                if (score.MaxScore == 0)
                {
                    //?Don't score it?
                    continue;
                }
                score.Percent = (score.TotalScore / score.MaxScore) * 100m;
                score.Rank = category.Rank;
                scores.Add(score);

            }

            if (scores.OrderByDescending(a => a.Percent)
                .FirstOrDefault() != null)
            { 
            surveyResponse.PositiveCategory = scores
                .OrderByDescending(a => a.Percent)
                .ThenBy(a => a.Rank)
                .FirstOrDefault().Category;
            }
            if (scores.Where(a => a.Category != surveyResponse.PositiveCategory)                
                .FirstOrDefault() != null)
            {
                surveyResponse.NegativeCategory1 = scores
                .Where(a => a.Category != surveyResponse.PositiveCategory)
                .OrderBy(a => a.Percent)
                .ThenBy(a => a.Rank)
                .FirstOrDefault().Category;
            }
            if (scores.Where(a => a.Category != surveyResponse.PositiveCategory && a.Category != surveyResponse.NegativeCategory1)
                .FirstOrDefault() != null)
            {
                surveyResponse.NegativeCategory2 = scores
                .Where(a => a.Category != surveyResponse.PositiveCategory && a.Category != surveyResponse.NegativeCategory1)
                .OrderBy(a => a.Percent)
                .ThenBy(a => a.Rank)
                .FirstOrDefault().Category;
            }


            //If some are null because not questions are all answers were skipped for that category, still try to put something in.
            if (surveyResponse.PositiveCategory == null)
            {
                surveyResponse.PositiveCategory = surveyResponse.Survey.Categories.Where(a => !a.DoNotUseForCalculations && a.IsActive && a.IsCurrentVersion).OrderBy(a => a.Rank).FirstOrDefault();
            }
            if (surveyResponse.NegativeCategory1 == null)
            {
                surveyResponse.NegativeCategory1 = surveyResponse.Survey.Categories.Where(a => !a.DoNotUseForCalculations && a.IsActive && a.IsCurrentVersion && a != surveyResponse.PositiveCategory).OrderBy(a => a.Rank).FirstOrDefault();
            }
            if (surveyResponse.NegativeCategory2 == null)
            {
                surveyResponse.NegativeCategory2 = surveyResponse.Survey.Categories.Where(a => !a.DoNotUseForCalculations && a.IsActive && a.IsCurrentVersion && a != surveyResponse.PositiveCategory && a != surveyResponse.NegativeCategory1).OrderBy(a => a.Rank).FirstOrDefault();
            }


            return;
        }


    }
}