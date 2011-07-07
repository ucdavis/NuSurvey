using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core.Extensions;
using NuSurvey.Web.Controllers;

namespace NuSurvey.Tests.ControllerTests.SurveyControllerTests
{

    public partial class SurveyControllerTests
    {
        #region Mapping Tests

        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/Survey/Index/".ShouldMapTo<SurveyController>(a => a.Index());
        }

        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestIndex2Mapping()
        {
            "~/Survey/".ShouldMapTo<SurveyController>(a => a.Index());
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestDetailsMapping()
        {
            "~/Survey/Details/5".ShouldMapTo<SurveyController>(a => a.Details(5, null, null));
        }

        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestPendingDetailsMapping()
        {
            "~/Survey/PendingDetails/5".ShouldMapTo<SurveyController>(a => a.PendingDetails(5));
        }

        /// <summary>
        /// #4
        /// </summary>
        [TestMethod]
        public void TestCreateGetMapping()
        {
            "~/Survey/Create".ShouldMapTo<SurveyController>(a => a.Create());
        }

        /// <summary>
        /// #5
        /// </summary>
        [TestMethod]
        public void TestCreatePostMapping()
        {
            "~/Survey/Create".ShouldMapTo<SurveyController>(a => a.Create(new Survey()), true);
        }

        /// <summary>
        /// #6
        /// </summary>
        [TestMethod]
        public void TestEditGetMapping()
        {
            "~/Survey/Edit/5".ShouldMapTo<SurveyController>(a => a.Edit(5));
        }

        /// <summary>
        /// #7
        /// </summary>
        [TestMethod]
        public void TestEditPostMapping()
        {
            "~/Survey/Edit/5".ShouldMapTo<SurveyController>(a => a.Edit(5, new Survey()), true);
        }

        /// <summary>
        /// #8
        /// </summary>
        [TestMethod]
        public void TestReviewMapping()
        {
            "~/Survey/Review/5".ShouldMapTo<SurveyController>(a => a.Review());
        }

        /// <summary>
        /// #9
        /// </summary>
        [TestMethod]
        public void TestYourDetailsMapping()
        {
            "~/Survey/YourDetails/5".ShouldMapTo<SurveyController>(a => a.YourDetails(5, null, null));
        }
        #endregion Mapping Tests
    }
}
