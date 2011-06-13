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
        [TestMethod]
        public void TestDetailsMapping()
        {
            "~/Question/Details/5".ShouldMapTo<QuestionController>(a => a.Details(5, null));
        }

        [TestMethod]
        public void TestCreateGetMapping()
        {
            "~/Question/Create/5".ShouldMapTo<QuestionController>(a => a.Create(5, null));
        }
        #endregion Mapping Tests
    }
}
