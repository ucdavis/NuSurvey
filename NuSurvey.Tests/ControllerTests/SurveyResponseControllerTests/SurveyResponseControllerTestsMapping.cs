using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;
using NuSurvey.Web;
using NuSurvey.Web.Controllers;
using NuSurvey.Web.Controllers.Filters;
using NuSurvey.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Web.Helpers;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;
using NuSurvey.Tests.Core.Extensions;

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
            "~/SurveyResponse/Details/5".ShouldMapTo<SurveyResponseController>(a => a.Details(5));
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
            "~/SurveyResponse/AnswerNext/5".ShouldMapTo<SurveyResponseController>(a => a.AnswerNext(5, new QuestionAnswerParameter()), true);
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
        #endregion Mapping Tests
    }
}
