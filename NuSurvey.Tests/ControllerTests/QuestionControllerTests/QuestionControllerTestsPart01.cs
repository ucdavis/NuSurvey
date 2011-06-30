using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core.Helpers;
using NuSurvey.Web.Controllers;

namespace NuSurvey.Tests.ControllerTests.QuestionControllerTests
{
    public partial class QuestionControllerTests
    {
        #region Details Tests

        [TestMethod]
        public void TestDetailsRedirectsWhenQuestionNotFound1()
        {
            #region Arrange
            new FakeQuestions(3, QuestionRepository);
            #endregion Arrange

            #region Act
            Controller.Details(4, null)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Question not found.", Controller.Message);
            #endregion Assert		
        }

        [TestMethod]
        public void TestDetailsRedirectsWhenQuestionNotFound2()
        {
            #region Arrange
            new FakeQuestions(3, QuestionRepository);
            #endregion Arrange

            #region Act
            Controller.Details(4, 3)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Question not found.", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestDetailsReturnsView1()
        {
            #region Arrange
            var category = CreateValidEntities.Category(9);
            var questions = new List<Question>();
            for (int i = 0; i < 3; i++)
            {
                questions.Add(CreateValidEntities.Question(i+1));
                questions[i].Category = category;
            }
            new FakeQuestions(0, QuestionRepository, questions);
            #endregion Arrange

            #region Act
            var result = Controller.Details(2, null)
                .AssertViewRendered()
                .WithViewData<QuestionDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name2", result.Question.Name);
            Assert.IsNull(result.Category);
            #endregion Assert		
        }

        [TestMethod]
        public void TestDetailsReturnsView2()
        {
            #region Arrange
            var category = CreateValidEntities.Category(9);
            var questions = new List<Question>();
            for (int i = 0; i < 3; i++)
            {
                questions.Add(CreateValidEntities.Question(i + 1));
                questions[i].Category = category;
            }
            new FakeQuestions(0, QuestionRepository, questions);
            #endregion Arrange

            #region Act
            var result = Controller.Details(2, 3)
                .AssertViewRendered()
                .WithViewData<QuestionDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name2", result.Question.Name);
            Assert.AreEqual("Name9", result.Category.Name);
            #endregion Assert
        }
        #endregion Details Tests

    }
}
