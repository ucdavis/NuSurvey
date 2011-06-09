using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core.Extensions;
using NuSurvey.Web.Controllers;


namespace NuSurvey.Tests.ControllerTests.CategoryGoalControllerTests
{
    public partial class CategoryGoalControllerTests
    {
        #region Mapping Tests
        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestDetailsMapping()
        {
            "~/CategoryGoal/Details/5".ShouldMapTo<CategoryGoalController>(a => a.Details(5));
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestCreateGetMapping()
        {
            "~/CategoryGoal/Create/5".ShouldMapTo<CategoryGoalController>(a => a.Create(5));
        }

        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestCreatePostMapping()
        {
            "~/CategoryGoal/Create/5".ShouldMapTo<CategoryGoalController>(a => a.Create(5, new CategoryGoal()), true);
        }
        #endregion Mapping Tests
    }
}
