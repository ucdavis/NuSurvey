using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NuSurvey.Core.Domain;
using NuSurvey.Web.Controllers;

namespace NuSurvey.Web.Services
{
    public interface IScoreService
    {
        QuestionAnswerParameter ScoreQuestion(IQueryable<Question> questions, QuestionAnswerParameter questionAnswerParameter);
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
                    #region Open Ended Int Question
                    int number;
                    int? responseId = null;
                    if (int.TryParse(questionAnswerParameter.Answer, out number))
                    {
                        questionAnswerParameter.OpenEndedNumericAnswer = number;

                        #region Exact Match
                        var response = question.Responses.Where(a => a.IsActive && a.Value == number.ToString()).FirstOrDefault();
                        if (response != null)
                        {
                            responseId = response.Id;
                        }
                        #endregion Exact Match

                        #region High Value
                        if (responseId == null)
                        {
                            //Check For High Value
                            response = question.Responses.Where(a => a.IsActive && a.Value.Contains("+")).FirstOrDefault();
                            if (response != null)
                            {
                                int highValue;
                                if (Int32.TryParse(response.Value, out highValue))
                                {
                                    if (number >= highValue)
                                    {
                                        responseId = response.Id;
                                    }
                                }
                            }
                        }
                        #endregion High Value

                        #region Low Value
                        if (responseId == null)
                        {
                            //Check for low value
                            response = question.Responses.Where(a => a.IsActive && a.Value.Contains("-")).FirstOrDefault();
                            if (response != null)
                            {
                                int lowValue;
                                if (Int32.TryParse(response.Value, out lowValue))
                                {
                                    if (number <= Math.Abs(lowValue))
                                    {
                                        responseId = response.Id;
                                    }
                                }
                            }
                        }
                        #endregion Low Value

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
                        questionAnswerParameter.Message = "Answer must be a number";
                        questionAnswerParameter.Score = 0;
                    }
                    #endregion Open Ended Int Question
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
    }
}