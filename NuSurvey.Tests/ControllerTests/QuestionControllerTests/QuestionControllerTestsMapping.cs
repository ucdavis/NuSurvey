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
        #endregion Mapping Tests
    }
}
