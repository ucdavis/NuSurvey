using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core.Helpers;
using NuSurvey.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;

namespace NuSurvey.Tests.InterfaceTests.ScoreServiceTests.ScoreServiceQuestionTests
{
    [TestClass]
    public partial class ScoreServiceQuestionTests
    {
        public IScoreService ScoreService;
        public IRepository<Question> QuestionRepository;

        public ScoreServiceQuestionTests()
        {
            ScoreService = new ScoreService();
            QuestionRepository = MockRepository.GenerateStub<IRepository<Question>>();
            SetupQuestions();
        }

        #region Helper Methods
        private void SetupQuestions()
        {
            var questions = new List<Question>();
            for (int i = 0; i < 12; i++)
            {
                questions.Add(CreateValidEntities.Question(i + 1));
            }

            #region Question 1 (Non-Open Ended)
            questions[0].Name = "RadionButtons";
            for (int i = 0; i < 4; i++)
            {
                questions[0].AddResponse(CreateValidEntities.Response(i + 1));
                questions[0].Responses[i].SetIdTo(i + 1);
            }
            questions[0].Responses[1].IsActive = false;
            questions[0].Responses[3].Score = 9;
            questions[0].IsOpenEnded = false;
            #endregion Question 1 (Non-Open Ended)

            #region Question 2 (Non-Open Ended and Not Scoreing)
            questions[1].Name = "RadionButtons";
            for (int i = 0; i < 4; i++)
            {
                questions[1].AddResponse(CreateValidEntities.Response(i + 1));
                questions[1].Responses[i].SetIdTo(i + 1);
            }
            questions[1].Responses[1].IsActive = false;
            questions[1].Responses[3].Score = 9;
            questions[1].IsOpenEnded = false;
            questions[1].Category.DoNotUseForCalculations = true;
            #endregion Question 2 (Non-Open Ended and Not Scoreing)

            #region Question 3 (Open Ended Whole Number)
            questions[2].Name = "Whole Number";
            foreach (var response in WholeNumberResponses())
            {
                questions[2].AddResponse(response);
            }
            questions[2].IsOpenEnded = true;
            questions[2].OpenEndedQuestionType = (int)QuestionType.WholeNumber;
            #endregion Question 3 (Open Ended Whole Number)

            #region Question 4 (Open Ended Whole Number No Scoreing)
            questions[3].Name = "Whole Number";
            foreach (var response in WholeNumberResponses())
            {
                questions[3].AddResponse(response);
            }
            questions[3].IsOpenEnded = true;
            questions[3].OpenEndedQuestionType = (int)QuestionType.WholeNumber;
            questions[3].Category.DoNotUseForCalculations = true;
            #endregion Question 4 (Open Ended Whole Number No Scoreing)

            #region Question 5 (Open Ended Decimal)
            questions[4].Name = "Decimal";
            foreach (var response in DecimalResponses())
            {
                questions[4].AddResponse(response);
            }
            questions[4].IsOpenEnded = true;
            questions[4].OpenEndedQuestionType = (int)QuestionType.Decimal;
            #endregion Question 5 (Open Ended Decimal)

            #region Question 6 (Open Ended Decimal No Scoring)
            questions[5].Name = "Decimal";
            foreach (var response in DecimalResponses())
            {
                questions[5].AddResponse(response);
            }
            questions[5].IsOpenEnded = true;
            questions[5].OpenEndedQuestionType = (int)QuestionType.Decimal;
            questions[5].Category.DoNotUseForCalculations = true;
            #endregion Question 6 (Open Ended Decimal No Scoring)

            #region Question 7 (Open Ended Time)
            questions[6].Name = "Time";
            foreach (var response in TimeResponses())
            {
                questions[6].AddResponse(response);
            }
            questions[6].IsOpenEnded = true;
            questions[6].OpenEndedQuestionType = (int)QuestionType.Time;
            #endregion Question 7 (Open Ended Time)

            #region Question 8 (Open Ended Time No Scoring)
            questions[7].Name = "Time";
            foreach (var response in TimeResponses())
            {
                questions[7].AddResponse(response);
            }
            questions[7].IsOpenEnded = true;
            questions[7].OpenEndedQuestionType = (int)QuestionType.Time;
            questions[7].Category.DoNotUseForCalculations = true;
            #endregion Question 8 (Open Ended Time No Scoring)

            #region Question 9 (Open Ended Time (AM/PM))
            questions[8].Name = "Time AM/PM";
            foreach (var response in TimeResponsesAmPm())
            {
                questions[8].AddResponse(response);
            }
            questions[8].IsOpenEnded = true;
            questions[8].OpenEndedQuestionType = (int)QuestionType.TimeAmPm;
            #endregion Question 9 (Open Ended Time (AM/PM))

            #region Question 10 (Open Ended Time (AM/PM) No Scoreing)
            questions[9].Name = "Time AM/PM";
            foreach (var response in TimeResponsesAmPm())
            {
                questions[9].AddResponse(response);
            }
            questions[9].IsOpenEnded = true;
            questions[9].OpenEndedQuestionType = (int)QuestionType.TimeAmPm;
            questions[9].Category.DoNotUseForCalculations = true;
            #endregion Question 10 (Open Ended Time (AM/PM) No Scoring)

            #region Question 11 (Open Ended Time Range)
            questions[10].Name = "Time Range";
            foreach (var response in TimeRangeResponses())
            {
                questions[10].AddResponse(response);
            }
            questions[10].IsOpenEnded = true;
            questions[10].OpenEndedQuestionType = (int)QuestionType.TimeRange;
            #endregion Question 11 (Open Ended Time Range)

            #region Question 12 (Open Ended Time Range No Scoring)
            questions[11].Name = "Time Range";
            foreach (var response in TimeRangeResponses())
            {
                questions[11].AddResponse(response);
            }
            questions[11].IsOpenEnded = true;
            questions[11].OpenEndedQuestionType = (int)QuestionType.TimeRange;
            questions[11].Category.DoNotUseForCalculations = true;
            #endregion Question 12 (Open Ended Time Range No Scoring)

            new FakeQuestions(0, QuestionRepository, questions);
        }       


        private static IEnumerable<Response> WholeNumberResponses()
        {
            var responses = new List<Response>();
            for (int i = 0; i < 7; i++)
            {
                responses.Add(CreateValidEntities.Response(i+1));
                responses[i].Score = i + 2;
                responses[i].SetIdTo(i + 1);
            }
            responses[0].Value = "3";
            responses[1].Value = "5";
            responses[2].Value = "8";
            responses[3].Value = "12";
            responses[4].Value = "13";
            responses[4].IsActive = false;
            responses[5].Value = "14";
            
            responses[6].Value = "15.2";
            responses[6].Score = 99;

            var scrambledResponses = new List<Response>(); //Because the service sorts.
            scrambledResponses.Add(responses[2]);
            scrambledResponses.Add(responses[5]);
            scrambledResponses.Add(responses[0]);
            scrambledResponses.Add(responses[4]);
            scrambledResponses.Add(responses[1]);
            scrambledResponses.Add(responses[3]);
            scrambledResponses.Add(responses[6]);

            return scrambledResponses;
        }

        private static IEnumerable<Response> DecimalResponses()
        {
            var responses = new List<Response>();
            for (int i = 0; i < 11; i++)
            {
                responses.Add(CreateValidEntities.Response(i + 1));
                responses[i].Score = i + 1;
                responses[i].SetIdTo(i + 1);
            }

            responses[0].Value = "-1.2";
            responses[1].Value = "0";
            responses[2].Value = "1.5";
            responses[3].Value = "2";
            responses[4].Value = "3";
            responses[5].Value = "4.25";
            responses[6].Value = "4.5";
            responses[7].Value = "5";
            responses[7].IsActive = false;
            responses[8].Value = "6";
            responses[9].Value = "6.01";

            responses[10].Value = "TEN";
            responses[10].Score = 99;

            var scrambledResponses = new List<Response>(); //Because the service sorts.
            scrambledResponses.Add(responses[6]);
            scrambledResponses.Add(responses[5]);
            scrambledResponses.Add(responses[3]);
            scrambledResponses.Add(responses[1]);
            scrambledResponses.Add(responses[2]);
            scrambledResponses.Add(responses[7]);
            scrambledResponses.Add(responses[0]);
            scrambledResponses.Add(responses[8]);
            scrambledResponses.Add(responses[4]);
            scrambledResponses.Add(responses[10]);
            scrambledResponses.Add(responses[9]);
            return scrambledResponses;
        }

        private static IEnumerable<Response> TimeResponses()
        {
            var responses = new List<Response>();
            for (int i = 0; i < 9; i++)
            {
                responses.Add(CreateValidEntities.Response(i + 1));
                responses[i].Score = i + 1;
                responses[i].SetIdTo(i + 1);
            }

            responses[0].Value = "1:30";
            responses[1].Value = "1:45";
            responses[2].Value = "2:00";
            responses[3].Value = "3:00";
            responses[4].Value = "5:00";
            responses[5].Value = "8:00";
            responses[6].Value = "NINE";
            responses[6].Score = 99;
            responses[7].Value = "10:00";
            responses[7].IsActive = false;
            responses[8].Value = "12:30";


            var scrambledResponses = new List<Response>(); //Because the service sorts.
            scrambledResponses.Add(responses[3]);
            scrambledResponses.Add(responses[5]);
            scrambledResponses.Add(responses[6]);
            scrambledResponses.Add(responses[1]);
            scrambledResponses.Add(responses[4]);
            scrambledResponses.Add(responses[2]);
            scrambledResponses.Add(responses[0]);
            scrambledResponses.Add(responses[8]);
            scrambledResponses.Add(responses[7]);
            return scrambledResponses;
        }

        private static IEnumerable<Response> TimeResponsesAmPm()
        {
            var responses = new List<Response>();
            for (int i = 0; i < 12; i++)
            {
                responses.Add(CreateValidEntities.Response(i + 1));
                responses[i].Score = i + 1;
                responses[i].SetIdTo(i + 1);
            }

            responses[0].Value = "1:30 AM";
            responses[1].Value = "1:45 AM";
            responses[2].Value = "2:00 AM";
            responses[3].Value = "3:00 AM";
            responses[4].Value = "5:00 AM";
            responses[5].Value = "8:00 AM";
            responses[6].Value = "12:00 PM";
            responses[7].Value = "6:30 PM";
            responses[8].Value = "12:00 AM"; //Note, this will have a different sort             
            responses[9].Value = "NINE";
            responses[9].Score = 99;
            responses[10].Value = "10:00 PM";
            responses[10].IsActive = false;
            responses[11].Value = "11:59 PM";


            var scrambledResponses = new List<Response>(); //Because the service sorts.
            scrambledResponses.Add(responses[11]);
            scrambledResponses.Add(responses[4]);
            scrambledResponses.Add(responses[1]);
            scrambledResponses.Add(responses[10]);
            scrambledResponses.Add(responses[3]);
            scrambledResponses.Add(responses[5]);
            scrambledResponses.Add(responses[0]);
            scrambledResponses.Add(responses[8]);
            scrambledResponses.Add(responses[2]);
            scrambledResponses.Add(responses[6]);
            scrambledResponses.Add(responses[7]);
            scrambledResponses.Add(responses[9]);
            return scrambledResponses;
        }

        private static IEnumerable<Response> TimeRangeResponses()
        {
            var responses = new List<Response>();
            for (int i = 0; i < 17; i++)
            {
                responses.Add(CreateValidEntities.Response(i + 1));
                responses[i].Score = i + 1;
                responses[i].SetIdTo(i + 1);
            }

            responses[0].Value = "0";
            responses[1].Value = "1";
            responses[2].Value = "1.5";
            responses[3].Value = "2";
            responses[4].Value = "3";
            responses[5].Value = "4";
            responses[5].IsActive = false;
            responses[6].Value = "5";
            responses[7].Value = "12";          
            responses[8].Value = "18";
            responses[9].Value = "22";
            responses[10].Value = "TEN";
            responses[10].Score = 99;
            responses[11].Value = "23.99";
            responses[12].Value = "24"; //Can't hit this one either because the fraction is closer to 23.99
            responses[13].Value = "36"; //None of these higher values should be reachable.
            responses[14].Value = "47";
            responses[15].Value = "47.90";
            responses[16].Value = "48";

            var scrambledResponses = new List<Response>(); //Because the service sorts.
            scrambledResponses.Add(responses[3]);
            scrambledResponses.Add(responses[12]);
            scrambledResponses.Add(responses[5]);
            scrambledResponses.Add(responses[13]);
            scrambledResponses.Add(responses[7]);
            scrambledResponses.Add(responses[2]);
            scrambledResponses.Add(responses[1]);
            scrambledResponses.Add(responses[15]);
            scrambledResponses.Add(responses[6]);
            scrambledResponses.Add(responses[9]);
            scrambledResponses.Add(responses[14]);
            scrambledResponses.Add(responses[8]);
            scrambledResponses.Add(responses[16]);
            scrambledResponses.Add(responses[0]);
            scrambledResponses.Add(responses[11]);
            scrambledResponses.Add(responses[4]);
            scrambledResponses.Add(responses[10]);

            
            
            return scrambledResponses;
        }
        #endregion Helper Methods

        public class TestScoreParameters
        {
            public int Score;
            public string StartTime;
            public string EndTime;
        }

    }
    
}
