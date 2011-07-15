using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core.Helpers;
using NuSurvey.Web.Helpers;
using NuSurvey.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;

namespace NuSurvey.Tests.InterfaceTests
{
    [TestClass]
    public class ArchiveServiceTests
    {
        public IArchiveService ArchiveService;
        public IRepository Repository { get; set; }
        public IRepository<Category> CategoryRepository;
        public IRepository<Question> QuestionRepository;
        public IRepository<Survey> SurveyRepository;

        public ArchiveServiceTests()
        {
            AutomapperConfig.Configure();
            ArchiveService = new ArchiveService();
            Repository = MockRepository.GenerateStub<IRepository>();
            CategoryRepository = MockRepository.GenerateStub<IRepository<Category>>();
            QuestionRepository = MockRepository.GenerateStub<IRepository<Question>>();
            SurveyRepository = MockRepository.GenerateStub<IRepository<Survey>>();
            Repository.Expect(a => a.OfType<Category>()).Return(CategoryRepository);
        }


        [TestMethod]
        public void TestArchiveServiceWhenQuestionChangedAll()
        {
            #region Arrange
            var questionToEdit = GetQuestionToEdit();
            var categoryies = GetCategoryies();
            categoryies[0].IsCurrentVersion = true;
            new FakeCategories(0, CategoryRepository, categoryies);
            Assert.IsTrue(categoryies[0].IsCurrentVersion);
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, questionToEdit);
            #endregion Act

            #region Assert

            AssertNewValuesAreCorrect(categoryies, result);

            #region Check old version
            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.AreEqual(1, args2.Id);
            Assert.AreEqual("Name1", args2.Name);
            Assert.AreEqual(1, args2.Rank);
            Assert.AreEqual("Affirmation1", args2.Affirmation);
            Assert.AreEqual("Encouragement1", args2.Encouragement);
            Assert.IsFalse(args2.IsActive);
            Assert.IsFalse(args2.IsCurrentVersion);
            Assert.IsFalse(args2.DoNotUseForCalculations);
            Assert.AreEqual(new DateTime(2001, 01, 01), args2.CreateDate);
            Assert.AreEqual(new DateTime(2001, 01, 01), args2.LastUpdate);
            Assert.AreEqual(2, args2.Survey.Id);
            Assert.IsNull(args2.PreviousVersion);
            var repCount = 1;
            for (int i = 0; i < 3; i++)
            {
                Assert.IsNotNull(args2.CategoryGoals[i]);
                Assert.AreEqual(string.Format("Name{0}", i+1), args2.CategoryGoals[i].Name);
                Assert.AreEqual(i+1, args2.CategoryGoals[i].Id);
                
                Assert.IsNotNull(args2.Questions[i]);
                Assert.AreEqual(string.Format("Name{0}", i+1), args2.Questions[i].Name);
                Assert.AreEqual(i+1, args2.Questions[i].Id);
                for (int j = 0; j < 3; j++)
                {
                    Assert.IsNotNull(args2.Questions[i].Responses[j]);
                    Assert.AreEqual(string.Format("Value{0}", repCount), args2.Questions[i].Responses[j].Value);
                    Assert.AreEqual(repCount++, args2.Questions[i].Responses[j].Id);
                }
            }
            

            #endregion Check old version

            #endregion Assert		
        }


        [TestMethod]
        public void TestArchiveServiceWhenQuestionChanged1()
        {
            #region Arrange
            var questionToEdit = GetQuestionToEdit();
            var categoryies = GetCategoryies();
            categoryies[0].IsCurrentVersion = false;
            new FakeCategories(0, CategoryRepository, categoryies);
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, questionToEdit);
            #endregion Act

            #region Assert
            AssertNewValuesAreCorrect(categoryies, result);

            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.IsFalse(args2.IsCurrentVersion);
            #endregion Assert
        }

        [TestMethod]
        public void TestArchiveServiceWhenQuestionChanged2()
        {
            #region Arrange
            var questionToEdit = GetQuestionToEdit();
            var categoryies = GetCategoryies();
            categoryies[0].Name = "SomeName";
            new FakeCategories(0, CategoryRepository, categoryies);
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, questionToEdit);
            #endregion Act

            #region Assert
            AssertNewValuesAreCorrect(categoryies, result);

            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.AreEqual("SomeName", args2.Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestArchiveServiceWhenQuestionChanged3()
        {
            #region Arrange
            var questionToEdit = GetQuestionToEdit();
            var categoryies = GetCategoryies();
            categoryies[0].Rank = 7;
            new FakeCategories(0, CategoryRepository, categoryies);
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, questionToEdit);
            #endregion Act

            #region Assert
            AssertNewValuesAreCorrect(categoryies, result);

            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.AreEqual(7, args2.Rank);
            #endregion Assert
        }

        [TestMethod]
        public void TestArchiveServiceWhenQuestionChanged4()
        {
            #region Arrange
            var questionToEdit = GetQuestionToEdit();
            var categoryies = GetCategoryies();
            categoryies[0].Affirmation = "SomeJob";
            new FakeCategories(0, CategoryRepository, categoryies);
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, questionToEdit);
            #endregion Act

            #region Assert
            AssertNewValuesAreCorrect(categoryies, result);

            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.AreEqual("SomeJob", args2.Affirmation);
            #endregion Assert
        }

        [TestMethod]
        public void TestArchiveServiceWhenQuestionChanged5()
        {
            #region Arrange
            var questionToEdit = GetQuestionToEdit();
            var categoryies = GetCategoryies();
            categoryies[0].Encouragement = "Keep Trying!";
            new FakeCategories(0, CategoryRepository, categoryies);
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, questionToEdit);
            #endregion Act

            #region Assert
            AssertNewValuesAreCorrect(categoryies, result);

            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.AreEqual("Keep Trying!", args2.Encouragement);
            #endregion Assert
        }

        [TestMethod]
        public void TestArchiveServiceWhenQuestionChanged6()
        {
            #region Arrange
            var questionToEdit = GetQuestionToEdit();
            var categoryies = GetCategoryies();
            categoryies[0].IsActive = true;
            new FakeCategories(0, CategoryRepository, categoryies);
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, questionToEdit);
            #endregion Act

            #region Assert
            AssertNewValuesAreCorrect(categoryies, result);

            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.IsTrue(args2.IsActive);
            #endregion Assert
        }

        [TestMethod]
        public void TestArchiveServiceWhenQuestionChanged7()
        {
            #region Arrange
            var questionToEdit = GetQuestionToEdit();
            var categoryies = GetCategoryies();
            categoryies[0].IsActive = false;
            new FakeCategories(0, CategoryRepository, categoryies);
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, questionToEdit);
            #endregion Act

            #region Assert
            AssertNewValuesAreCorrect(categoryies, result);

            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.IsFalse(args2.IsActive);
            #endregion Assert
        }

        [TestMethod]
        public void TestArchiveServiceWhenQuestionChanged8()
        {
            #region Arrange
            var questionToEdit = GetQuestionToEdit();
            var categoryies = GetCategoryies();
            categoryies[0].DoNotUseForCalculations = true;
            new FakeCategories(0, CategoryRepository, categoryies);
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, questionToEdit);
            #endregion Act

            #region Assert
            AssertNewValuesAreCorrect(categoryies, result);

            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.IsTrue(args2.DoNotUseForCalculations);
            #endregion Assert
        }

        [TestMethod]
        public void TestArchiveServiceWhenQuestionChanged9()
        {
            #region Arrange
            var questionToEdit = GetQuestionToEdit();
            var categoryies = GetCategoryies();
            categoryies[0].DoNotUseForCalculations = false;
            new FakeCategories(0, CategoryRepository, categoryies);
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, questionToEdit);
            #endregion Act

            #region Assert
            AssertNewValuesAreCorrect(categoryies, result);

            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.IsFalse(args2.DoNotUseForCalculations);
            #endregion Assert
        }

        [TestMethod]
        public void TestArchiveServiceWhenQuestionChanged10()
        {
            #region Arrange
            var questionToEdit = GetQuestionToEdit();
            var categoryies = GetCategoryies();
            categoryies[0].IsCurrentVersion = true;
            new FakeCategories(0, CategoryRepository, categoryies);
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, questionToEdit);
            #endregion Act

            #region Assert
            AssertNewValuesAreCorrect(categoryies, result);

            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.IsFalse(args2.IsCurrentVersion);
            #endregion Assert
        }

        [TestMethod]
        public void TestArchiveServiceWhenQuestionChanged11()
        {
            #region Arrange
            var questionToEdit = GetQuestionToEdit();
            var categoryies = GetCategoryies();
            categoryies[0].LastUpdate = new DateTime(2011,07,05);
            new FakeCategories(0, CategoryRepository, categoryies);
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, questionToEdit);
            #endregion Act

            #region Assert
            AssertNewValuesAreCorrect(categoryies, result);

            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.AreEqual(new DateTime(2011, 07, 05), args2.LastUpdate);
            #endregion Assert
        }

        [TestMethod]
        public void TestArchiveServiceWhenQuestionChanged12()
        {
            #region Arrange
            var questionToEdit = GetQuestionToEdit();
            var categoryies = GetCategoryies();
            categoryies[0].CreateDate = new DateTime(2011, 07, 05);
            new FakeCategories(0, CategoryRepository, categoryies);
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, questionToEdit);
            #endregion Act

            #region Assert
            AssertNewValuesAreCorrect(categoryies, result);

            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.AreEqual(new DateTime(2011, 07, 05), args2.CreateDate);
            #endregion Assert
        }
        [TestMethod]
        public void TestArchiveServiceWhenQuestionChanged13()
        {
            #region Arrange
            var questionToEdit = GetQuestionToEdit();
            var categoryies = GetCategoryies();
            categoryies[0].PreviousVersion = CreateValidEntities.Category(8);
            categoryies[0].PreviousVersion.SetIdTo(8);
            new FakeCategories(0, CategoryRepository, categoryies);
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, questionToEdit);
            #endregion Act

            #region Assert
            AssertNewValuesAreCorrect(categoryies, result);

            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.AreEqual(8, args2.PreviousVersion.Id);
            #endregion Assert
        }

        [TestMethod]
        public void TestArchiveServiceWhenQuestionChanged14()
        {
            #region Arrange
            var questionToEdit = GetQuestionToEdit();
            var categoryies = GetCategoryies();
            categoryies[0].CategoryGoals[1].Name = "My Category Goal";
            new FakeCategories(0, CategoryRepository, categoryies);
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, questionToEdit);
            #endregion Act

            #region Assert
            AssertNewValuesAreCorrect(categoryies, result);

            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.AreEqual("My Category Goal", args2.CategoryGoals[1].Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestArchiveServiceWhenQuestionChanged15()
        {
            #region Arrange
            var questionToEdit = GetQuestionToEdit();
            var categoryies = GetCategoryies();
            categoryies[0].Questions[1].Name = "My Question";
            new FakeCategories(0, CategoryRepository, categoryies);
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, questionToEdit);
            #endregion Act

            #region Assert
            AssertNewValuesAreCorrect(categoryies, result);

            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.AreEqual("My Question", args2.Questions[1].Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestArchiveServiceWhenQuestionChanged16()
        {
            #region Arrange
            var questionToEdit = GetQuestionToEdit();
            var categoryies = GetCategoryies();
            categoryies[0].Questions[1].Responses[1].Value = "My Choice";
            new FakeCategories(0, CategoryRepository, categoryies);
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, questionToEdit);
            #endregion Act

            #region Assert
            AssertNewValuesAreCorrect(categoryies, result);

            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.AreEqual("My Choice", args2.Questions[1].Responses[1].Value);
            #endregion Assert
        }
        #region Helper Methods

        public void AssertNewValuesAreCorrect(List<Category> categoryies, Category result)
        {
            CategoryRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Category>.Is.Anything), options => options.Repeat.Times(2));
            var args1 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[0][0];
            Assert.IsNotNull(args1);
            Assert.AreEqual(result, args1);
            Assert.AreEqual(0, args1.Id);
            Assert.AreEqual(2, args1.Questions.Count);
            foreach (var question in args1.Questions)
            {
                Assert.AreEqual(0, question.Id);
                Assert.AreEqual(3, question.Responses.Count);
                foreach (var response in question.Responses)
                {
                    Assert.AreEqual(0, response.Id);
                }
            }
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(categoryies[0].CategoryGoals[i].Name, args1.CategoryGoals[i].Name);
                Assert.AreEqual(0, args1.CategoryGoals[i].Id);
            }
            Assert.AreEqual(categoryies[0].IsActive, args1.IsActive);
            Assert.AreEqual(categoryies[0].Name, args1.Name);
            Assert.AreEqual(categoryies[0].Affirmation, args1.Affirmation);
            Assert.AreEqual(categoryies[0].Encouragement, args1.Encouragement);
            Assert.AreEqual(categoryies[0].DoNotUseForCalculations, args1.DoNotUseForCalculations);
            Assert.AreEqual(categoryies[0].Id, args1.PreviousVersion.Id);
            Assert.AreEqual(categoryies[0].Rank, args1.Rank);
            Assert.AreEqual(DateTime.Now.Date, args1.CreateDate.Date);
            Assert.AreEqual(args1.CreateDate, args1.LastUpdate);
            Assert.AreEqual(2, args1.Survey.Id);
            Assert.IsTrue(args1.IsCurrentVersion);
        }

        public Question GetQuestionToEdit()
        {
            var questionToEdit = CreateValidEntities.Question(99);
            questionToEdit.SetIdTo(2);
            for (int i = 0; i < 3; i++)
            {
                questionToEdit.AddResponse(CreateValidEntities.Response(90 + i));
                questionToEdit.Responses[i].SetIdTo(i + 4);
            }
            return questionToEdit;
        }

        public List<Category> GetCategoryies()
        {
            new FakeSurveys(3, SurveyRepository);

            var categoryies = new List<Category>();
            categoryies.Add(CreateValidEntities.Category(1));
            categoryies[0].Survey = SurveyRepository.GetNullableById(2);
            var responseCount = 1;
            for (int i = 0; i < 3; i++)
            {
                categoryies[0].AddCategoryGoal(CreateValidEntities.CategoryGoal(i+1));
                categoryies[0].CategoryGoals[i].SetIdTo(i + 1);
                categoryies[0].AddQuestions(CreateValidEntities.Question(i+1));
                categoryies[0].Questions[i].SetIdTo(i + 1);
                for (int j = 0; j < 3; j++)
                {
                    categoryies[0].Questions[i].AddResponse(CreateValidEntities.Response(responseCount));
                    categoryies[0].Questions[i].Responses[j].SetIdTo(responseCount++);
                }
            }
            categoryies[0].CreateDate = new DateTime(2001,01,01);
            categoryies[0].LastUpdate = new DateTime(2001, 01, 01);
            return categoryies;
        }

        #endregion Helper Methods
    }
}
