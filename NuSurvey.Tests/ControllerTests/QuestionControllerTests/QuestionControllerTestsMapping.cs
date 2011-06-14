using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core.Extensions;
using NuSurvey.Web.Controllers;

namespace NuSurvey.Tests.ControllerTests.QuestionControllerTests
{
    public partial class QuestionControllerTests
    {
        #region Mapping Tests
        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestDetailsMapping()
        {
            "~/Question/Details/5".ShouldMapTo<QuestionController>(a => a.Details(5, null));
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestCreateGetMapping()
        {
            "~/Question/Create/5".ShouldMapTo<QuestionController>(a => a.Create(5, null));
        }

        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestCreatePostMapping()
        {
            "~/Question/Create/5".ShouldMapTo<QuestionController>(a => a.Create(5, null, new Question(),new ResponsesParameter[0] ), true);
        }

        /// <summary>
        /// #4
        /// </summary>
        [TestMethod]
        public void TestEditGetMapping()
        {
            "~/Question/Edit/5".ShouldMapTo<QuestionController>(a => a.Edit(5, 3, null), true);
        }

        /// <summary>
        /// #5
        /// </summary>
        [TestMethod]
        public void TestEditPostMapping()
        {
            "~/Question/Edit/5".ShouldMapTo<QuestionController>(a => a.Edit(5, 3, null, new Question(),new ResponsesParameter[0] ), true);
        }

        /// <summary>
        /// #6
        /// </summary>
        [TestMethod]
        public void TestReOrderGetMapping()
        {
            "~/Question/ReOrder/5".ShouldMapTo<QuestionController>(a => a.ReOrder(5));
        }

        /// <summary>
        /// #7
        /// </summary>
        [TestMethod]
        public void TestReOrderPostMapping()
        {
            "~/Question/ReOrder/5".ShouldMapTo<QuestionController>(a => a.ReOrder(5, new int[0]));
        }
        #endregion Mapping Tests
    }
}
