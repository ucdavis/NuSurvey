using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core.Extensions;
using NuSurvey.Web.Controllers;
using NuSurvey.Web.Models;

namespace NuSurvey.Tests.ControllerTests.CategoryControllerTests
{
    public partial class CategoryControllerTests
    {

        #region Mapping Tests
        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestReOrderGetMapping()
        {
            "~/Category/ReOrder/5".ShouldMapTo<CategoryController>(a => a.ReOrder(5));
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestReOrderPostMapping()
        {
            "~/Category/ReOrder/5".ShouldMapTo<CategoryController>(a => a.ReOrder(5, new int[0]), true);
        }

        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestCreateGetMapping()
        {
            "~/Category/Create/5".ShouldMapTo<CategoryController>(a => a.Create(5));
        }

        /// <summary>
        /// #4
        /// </summary>
        [TestMethod]
        public void TestCreatePostMapping()
        {
            "~/Category/Create/5".ShouldMapTo<CategoryController>(a => a.Create(5, new Category()), true);
        }
        #endregion Mapping Tests

    }
}
