using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core.Helpers;
using NuSurvey.Web.Controllers;
using NuSurvey.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;

namespace NuSurvey.Tests.InterfaceTests.ScoreServiceTests.ScoreServiceCalculateScoresTests
{
    [TestClass]
    public class ScoreServiceCalculateScoresTests
    {
        public IScoreService ScoreService;
        public IRepository<CategoryTotalMaxScore> CategoryTotalMaxScoreRepository { get; set; }

        public ScoreServiceCalculateScoresTests()
        {
            ScoreService = new ScoreService();
            CategoryTotalMaxScoreRepository = MockRepository.GenerateStub<IRepository<CategoryTotalMaxScore>>();
            SetupTotalMaxScores();
        }

        [TestMethod]
        public void TestDescription()
        {
            #region Arrange
            Assert.Inconclusive("Do ScoreService Tests");

            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert
        }

        #region Helper Methods
        public void SetupTotalMaxScores()
        {
            var categoryTotalMaxScores = new List<CategoryTotalMaxScore>();
            for (int i = 0; i < 6; i++)
            {
                categoryTotalMaxScores.Add(CreateValidEntities.CategoryTotalMaxScore(i+1));
                categoryTotalMaxScores[i].TotalMaxScore = (i + 1) + 10;
            }

            new FakeCategoryTotalMaxScore(0, CategoryTotalMaxScoreRepository, categoryTotalMaxScores);
        }
        #endregion Helper Methods
    }
}
