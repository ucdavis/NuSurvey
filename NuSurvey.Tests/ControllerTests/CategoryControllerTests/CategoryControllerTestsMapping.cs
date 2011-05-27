using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
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
        #endregion Mapping Tests

    }
}
