using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Web.Controllers;

namespace NuSurvey.Tests.ControllerTests.PrintControllerTests
{
    public partial class PrintControllerTests
    {
        #region Mapping SurveyResponse
        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestResultMapping()
        {
            "~/Print/Result/5".ShouldMapTo<PrintController>(a => a.Result(5));
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestResultsMapping()
        {
            "~/Print/Results/5".ShouldMapTo<PrintController>(a => a.Results(5, null, null));
        }

        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestPickResultsMapping()
        {
            "~/Print/PickResults/5".ShouldMapTo<PrintController>(a => a.PickResults(5, null));
        }
        #endregion Mapping Tests
    }
}
