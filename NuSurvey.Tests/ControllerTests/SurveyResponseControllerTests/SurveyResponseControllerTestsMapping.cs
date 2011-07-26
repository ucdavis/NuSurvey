using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core.Extensions;
using NuSurvey.Web.Controllers;

namespace NuSurvey.Tests.ControllerTests.SurveyResponseControllerTests
{
    public partial class SurveyResponseControllerTests
    {
        #region Mapping Tests

        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestIndex1Mapping()
        {
            "~/SurveyResponse/Index/".ShouldMapTo<SurveyResponseController>(a => a.Index());
        }

        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestIndex2Mapping()
        {
            "~/SurveyResponse".ShouldMapTo<SurveyResponseController>(a => a.Index());
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestDetailsMapping()
        {
            "~/SurveyResponse/Details/5".ShouldMapTo<SurveyResponseController>(a => a.Details(5, false), true);
        }

        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestStartSurveyGetMapping()
        {
            "~/SurveyResponse/StartSurvey/5".ShouldMapTo<SurveyResponseController>(a => a.StartSurvey(5));
        }

        /// <summary>
        /// #4
        /// </summary>
        [TestMethod]
        public void TestStartSurveyPostMapping()
        {
            "~/SurveyResponse/StartSurvey/5".ShouldMapTo<SurveyResponseController>(a => a.StartSurvey(5, new SurveyResponse()), true);
        }

        /// <summary>
        /// #5
        /// </summary>
        [TestMethod]
        public void TestAnswerNextGetMapping()
        {
            "~/SurveyResponse/AnswerNext/5".ShouldMapTo<SurveyResponseController>(a => a.AnswerNext(5));
        }

        /// <summary>
        /// #6
        /// </summary>
        [TestMethod]
        public void TestAnswerNextPostMapping()
        {
            "~/SurveyResponse/AnswerNext/5".ShouldMapTo<SurveyResponseController>(a => a.AnswerNext(5, new QuestionAnswerParameter(), string.Empty), true);
        }

        /// <summary>
        /// #7
        /// </summary>
        [TestMethod]
        public void TestFinalizePendingMapping()
        {
            "~/SurveyResponse/FinalizePending/5".ShouldMapTo<SurveyResponseController>(a => a.FinalizePending(5));
        }

        /// <summary>
        /// #8
        /// </summary>
        [TestMethod]
        public void TestDeletePendingGetMapping()
        {
            "~/SurveyResponse/DeletePending/5".ShouldMapTo<SurveyResponseController>(a => a.DeletePending(5, false), true);
        }

        /// <summary>
        /// #9
        /// </summary>
        [TestMethod]
        public void TestDeletePendingPostMapping()
        {
            "~/SurveyResponse/DeletePending/5".ShouldMapTo<SurveyResponseController>(a => a.DeletePending(5, false, false), true);
        }

        /// <summary>
        /// #10
        /// </summary>
        [TestMethod]
        public void TestCreateGetMapping()
        {
            "~/SurveyResponse/Create/5".ShouldMapTo<SurveyResponseController>(a => a.Create(5));
        }

        /// <summary>
        /// #11
        /// </summary>
        [TestMethod]
        public void TestCreatePostMapping()
        {
            "~/SurveyResponse/Create/5".ShouldMapTo<SurveyResponseController>(a => a.Create(5, new SurveyResponse(),new QuestionAnswerParameter[0] ), true);
        }

        /// <summary>
        /// #12
        /// </summary>
        [TestMethod]
        public void TestResultsMapping()
        {
            "~/SurveyResponse/Results/5".ShouldMapTo<SurveyResponseController>(a => a.Results(5));
        }
        #endregion Mapping Tests
    }
}
