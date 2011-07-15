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

        #region Archive When Question Is Changed
        
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
        #endregion Archive When Question Is Changed

        #region Archive When Category Is Changed

        [TestMethod]
        public void TestArchiveServiceWhenCategoryIsChangedAll()
        {
            #region Arrange
            var categoryies = GetCategoryies();          

            new FakeCategories(0, CategoryRepository, categoryies);
            var updatedCategory = CreateValidEntities.Category(9);
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, updatedCategory);
            #endregion Act

            #region Assert

            #region New Values
            CategoryRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Category>.Is.Anything), options => options.Repeat.Times(2));
            var args1 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[0][0];
            Assert.IsNotNull(args1);
            Assert.AreEqual(result, args1);
            Assert.AreEqual(0, args1.Id);
            Assert.AreEqual(3, args1.Questions.Count);
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
            Assert.AreEqual("Name9", args1.Name);
            Assert.AreEqual("Affirmation9", args1.Affirmation);
            Assert.AreEqual("Encouragement9", args1.Encouragement);
            Assert.AreEqual(categoryies[0].DoNotUseForCalculations, args1.DoNotUseForCalculations);
            Assert.AreEqual(categoryies[0].Id, args1.PreviousVersion.Id);
            Assert.AreEqual(categoryies[0].Rank, args1.Rank);
            Assert.AreEqual(DateTime.Now.Date, args1.CreateDate.Date);
            Assert.AreEqual(args1.CreateDate, args1.LastUpdate);
            Assert.AreEqual(2, args1.Survey.Id);
            Assert.IsTrue(args1.IsCurrentVersion);
            #endregion New Values

            #region Old Values
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
                Assert.AreEqual(string.Format("Name{0}", i + 1), args2.CategoryGoals[i].Name);
                Assert.AreEqual(i + 1, args2.CategoryGoals[i].Id);

                Assert.IsNotNull(args2.Questions[i]);
                Assert.AreEqual(string.Format("Name{0}", i + 1), args2.Questions[i].Name);
                Assert.AreEqual(i + 1, args2.Questions[i].Id);
                for (int j = 0; j < 3; j++)
                {
                    Assert.IsNotNull(args2.Questions[i].Responses[j]);
                    Assert.AreEqual(string.Format("Value{0}", repCount), args2.Questions[i].Responses[j].Value);
                    Assert.AreEqual(repCount++, args2.Questions[i].Responses[j].Id);
                }
            }
            
            #endregion Old Values

            #endregion Assert
        }

        [TestMethod]
        public void TestArchiveServiceWhenCategoryIsChanged1()
        {
            #region Arrange
            var categoryies = GetCategoryies();
            categoryies[0].Name = "OldName";
            new FakeCategories(0, CategoryRepository, categoryies);
            var updatedCategory = CreateValidEntities.Category(9);
            updatedCategory.Name = "NewName";
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, updatedCategory);
            #endregion Act

            #region Assert

            #region New Values
            CategoryRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Category>.Is.Anything), options => options.Repeat.Times(2));
            var args1 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[0][0];
            Assert.IsNotNull(args1);
            Assert.AreEqual(result, args1);
            Assert.AreEqual("NewName", args1.Name);
            #endregion New Values

            #region Old Values
            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.AreEqual("OldName", args2.Name);
            #endregion Old Values

            #endregion Assert
        }

        [TestMethod]
        public void TestArchiveServiceWhenCategoryIsChanged2()
        {
            #region Arrange
            var categoryies = GetCategoryies();
            categoryies[0].Rank = 5;
            new FakeCategories(0, CategoryRepository, categoryies);
            var updatedCategory = CreateValidEntities.Category(9);
            updatedCategory.Rank = 7; //Rank doesn't change
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, updatedCategory);
            #endregion Act

            #region Assert

            #region New Values
            CategoryRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Category>.Is.Anything), options => options.Repeat.Times(2));
            var args1 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[0][0];
            Assert.IsNotNull(args1);
            Assert.AreEqual(result, args1);
            Assert.AreEqual(5, args1.Rank);
            #endregion New Values

            #region Old Values
            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.AreEqual(5, args2.Rank);
            #endregion Old Values

            #endregion Assert
        }

        [TestMethod]
        public void TestArchiveServiceWhenCategoryIsChanged3()
        {
            #region Arrange
            var categoryies = GetCategoryies();
            categoryies[0].Affirmation = "OldAffirmation";
            new FakeCategories(0, CategoryRepository, categoryies);
            var updatedCategory = CreateValidEntities.Category(9);
            updatedCategory.Affirmation = "NewAffirmation";
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, updatedCategory);
            #endregion Act

            #region Assert

            #region New Values
            CategoryRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Category>.Is.Anything), options => options.Repeat.Times(2));
            var args1 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[0][0];
            Assert.IsNotNull(args1);
            Assert.AreEqual(result, args1);
            Assert.AreEqual("NewAffirmation", args1.Affirmation);
            #endregion New Values

            #region Old Values
            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.AreEqual("OldAffirmation", args2.Affirmation);
            #endregion Old Values

            #endregion Assert
        }

        [TestMethod]
        public void TestArchiveServiceWhenCategoryIsChanged4()
        {
            #region Arrange
            var categoryies = GetCategoryies();
            categoryies[0].Encouragement = "OldEncouragement";
            new FakeCategories(0, CategoryRepository, categoryies);
            var updatedCategory = CreateValidEntities.Category(9);
            updatedCategory.Encouragement = "NewEncouragement";
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, updatedCategory);
            #endregion Act

            #region Assert

            #region New Values
            CategoryRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Category>.Is.Anything), options => options.Repeat.Times(2));
            var args1 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[0][0];
            Assert.IsNotNull(args1);
            Assert.AreEqual(result, args1);
            Assert.AreEqual("NewEncouragement", args1.Encouragement);
            #endregion New Values

            #region Old Values
            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.AreEqual("OldEncouragement", args2.Encouragement);
            #endregion Old Values

            #endregion Assert
        }

        [TestMethod]
        public void TestArchiveServiceWhenCategoryIsChanged5()
        {
            #region Arrange
            var categoryies = GetCategoryies();
            categoryies[0].IsActive = true;
            new FakeCategories(0, CategoryRepository, categoryies);
            var updatedCategory = CreateValidEntities.Category(9);
            updatedCategory.IsActive = true;
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, updatedCategory);
            #endregion Act

            #region Assert

            #region New Values
            CategoryRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Category>.Is.Anything), options => options.Repeat.Times(2));
            var args1 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[0][0];
            Assert.IsNotNull(args1);
            Assert.AreEqual(result, args1);
            Assert.AreEqual(true, args1.IsActive);
            #endregion New Values

            #region Old Values
            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.AreEqual(true, args2.IsActive);
            #endregion Old Values

            #endregion Assert
        }

        [TestMethod]
        public void TestArchiveServiceWhenCategoryIsChanged6()
        {
            #region Arrange
            var categoryies = GetCategoryies();
            categoryies[0].IsActive = false;
            new FakeCategories(0, CategoryRepository, categoryies);
            var updatedCategory = CreateValidEntities.Category(9);
            updatedCategory.IsActive = true;
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, updatedCategory);
            #endregion Act

            #region Assert

            #region New Values
            CategoryRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Category>.Is.Anything), options => options.Repeat.Times(2));
            var args1 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[0][0];
            Assert.IsNotNull(args1);
            Assert.AreEqual(result, args1);
            Assert.AreEqual(true, args1.IsActive);
            #endregion New Values

            #region Old Values
            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.AreEqual(false, args2.IsActive);
            #endregion Old Values

            #endregion Assert
        }
        [TestMethod]
        public void TestArchiveServiceWhenCategoryIsChanged7()
        {
            #region Arrange
            var categoryies = GetCategoryies();
            categoryies[0].IsActive = true;
            new FakeCategories(0, CategoryRepository, categoryies);
            var updatedCategory = CreateValidEntities.Category(9);
            updatedCategory.IsActive = false;
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, updatedCategory);
            #endregion Act

            #region Assert

            #region New Values
            CategoryRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Category>.Is.Anything), options => options.Repeat.Times(2));
            var args1 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[0][0];
            Assert.IsNotNull(args1);
            Assert.AreEqual(result, args1);
            Assert.AreEqual(false, args1.IsActive);
            #endregion New Values

            #region Old Values
            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.AreEqual(true, args2.IsActive);
            #endregion Old Values

            #endregion Assert
        }

        [TestMethod]
        public void TestArchiveServiceWhenCategoryIsChanged8()
        {
            #region Arrange
            var categoryies = GetCategoryies();
            categoryies[0].IsActive = false;
            new FakeCategories(0, CategoryRepository, categoryies);
            var updatedCategory = CreateValidEntities.Category(9);
            updatedCategory.IsActive = false;
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, updatedCategory);
            #endregion Act

            #region Assert

            #region New Values
            CategoryRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Category>.Is.Anything), options => options.Repeat.Times(2));
            var args1 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[0][0];
            Assert.IsNotNull(args1);
            Assert.AreEqual(result, args1);
            Assert.AreEqual(false, args1.IsActive);
            #endregion New Values

            #region Old Values
            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.AreEqual(false, args2.IsActive);
            #endregion Old Values

            #endregion Assert
        }

        [TestMethod]
        public void TestArchiveServiceWhenCategoryIsChanged9()
        {
            #region Arrange
            var categoryies = GetCategoryies();
            categoryies[0].DoNotUseForCalculations = true;
            new FakeCategories(0, CategoryRepository, categoryies);
            var updatedCategory = CreateValidEntities.Category(9);
            updatedCategory.DoNotUseForCalculations = true;
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, updatedCategory);
            #endregion Act

            #region Assert

            #region New Values
            CategoryRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Category>.Is.Anything), options => options.Repeat.Times(2));
            var args1 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[0][0];
            Assert.IsNotNull(args1);
            Assert.AreEqual(result, args1);
            Assert.AreEqual(true, args1.DoNotUseForCalculations);
            #endregion New Values

            #region Old Values
            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.AreEqual(true, args2.DoNotUseForCalculations);
            #endregion Old Values

            #endregion Assert
        }

        [TestMethod]
        public void TestArchiveServiceWhenCategoryIsChanged10()
        {
            #region Arrange
            var categoryies = GetCategoryies();
            categoryies[0].DoNotUseForCalculations = false;
            new FakeCategories(0, CategoryRepository, categoryies);
            var updatedCategory = CreateValidEntities.Category(9);
            updatedCategory.DoNotUseForCalculations = true;
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, updatedCategory);
            #endregion Act

            #region Assert

            #region New Values
            CategoryRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Category>.Is.Anything), options => options.Repeat.Times(2));
            var args1 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[0][0];
            Assert.IsNotNull(args1);
            Assert.AreEqual(result, args1);
            Assert.AreEqual(true, args1.DoNotUseForCalculations);
            #endregion New Values

            #region Old Values
            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.AreEqual(false, args2.DoNotUseForCalculations);
            #endregion Old Values

            #endregion Assert
        }
        [TestMethod]
        public void TestArchiveServiceWhenCategoryIsChanged11()
        {
            #region Arrange
            var categoryies = GetCategoryies();
            categoryies[0].DoNotUseForCalculations = true;
            new FakeCategories(0, CategoryRepository, categoryies);
            var updatedCategory = CreateValidEntities.Category(9);
            updatedCategory.DoNotUseForCalculations = false;
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, updatedCategory);
            #endregion Act

            #region Assert

            #region New Values
            CategoryRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Category>.Is.Anything), options => options.Repeat.Times(2));
            var args1 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[0][0];
            Assert.IsNotNull(args1);
            Assert.AreEqual(result, args1);
            Assert.AreEqual(false, args1.DoNotUseForCalculations);
            #endregion New Values

            #region Old Values
            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.AreEqual(true, args2.DoNotUseForCalculations);
            #endregion Old Values

            #endregion Assert
        }

        [TestMethod]
        public void TestArchiveServiceWhenCategoryIsChanged12()
        {
            #region Arrange
            var categoryies = GetCategoryies();
            categoryies[0].DoNotUseForCalculations = false;
            new FakeCategories(0, CategoryRepository, categoryies);
            var updatedCategory = CreateValidEntities.Category(9);
            updatedCategory.DoNotUseForCalculations = false;
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, updatedCategory);
            #endregion Act

            #region Assert

            #region New Values
            CategoryRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Category>.Is.Anything), options => options.Repeat.Times(2));
            var args1 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[0][0];
            Assert.IsNotNull(args1);
            Assert.AreEqual(result, args1);
            Assert.AreEqual(false, args1.DoNotUseForCalculations);
            #endregion New Values

            #region Old Values
            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.AreEqual(false, args2.DoNotUseForCalculations);
            #endregion Old Values

            #endregion Assert
        }

        [TestMethod]
        public void TestArchiveServiceWhenCategoryIsChanged13()
        {
            #region Arrange
            var categoryies = GetCategoryies();
            categoryies[0].IsCurrentVersion = true;
            new FakeCategories(0, CategoryRepository, categoryies);
            var updatedCategory = CreateValidEntities.Category(9);
            updatedCategory.IsCurrentVersion = false;
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, updatedCategory);
            #endregion Act

            #region Assert

            #region New Values
            CategoryRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Category>.Is.Anything), options => options.Repeat.Times(2));
            var args1 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[0][0];
            Assert.IsNotNull(args1);
            Assert.AreEqual(result, args1);
            Assert.AreEqual(true, args1.IsCurrentVersion);
            #endregion New Values

            #region Old Values
            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.AreEqual(false, args2.IsCurrentVersion);
            #endregion Old Values

            #endregion Assert
        }

        [TestMethod]
        public void TestArchiveServiceWhenCategoryIsChanged14()
        {
            #region Arrange
            var categoryies = GetCategoryies();
            categoryies[0].IsCurrentVersion = false;
            new FakeCategories(0, CategoryRepository, categoryies);
            var updatedCategory = CreateValidEntities.Category(9);
            updatedCategory.IsCurrentVersion = false;
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, updatedCategory);
            #endregion Act

            #region Assert

            #region New Values
            CategoryRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Category>.Is.Anything), options => options.Repeat.Times(2));
            var args1 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[0][0];
            Assert.IsNotNull(args1);
            Assert.AreEqual(result, args1);
            Assert.AreEqual(true, args1.IsCurrentVersion);
            #endregion New Values

            #region Old Values
            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.AreEqual(false, args2.IsCurrentVersion);
            #endregion Old Values

            #endregion Assert
        }

        [TestMethod]
        public void TestArchiveServiceWhenCategoryIsChanged15()
        {
            #region Arrange
            var categoryies = GetCategoryies();
            categoryies[0].IsCurrentVersion = true;
            new FakeCategories(0, CategoryRepository, categoryies);
            var updatedCategory = CreateValidEntities.Category(9);
            updatedCategory.IsCurrentVersion = true;
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, updatedCategory);
            #endregion Act

            #region Assert

            #region New Values
            CategoryRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Category>.Is.Anything), options => options.Repeat.Times(2));
            var args1 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[0][0];
            Assert.IsNotNull(args1);
            Assert.AreEqual(result, args1);
            Assert.AreEqual(true, args1.IsCurrentVersion);
            #endregion New Values

            #region Old Values
            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.AreEqual(false, args2.IsCurrentVersion);
            #endregion Old Values

            #endregion Assert
        }

        [TestMethod]
        public void TestArchiveServiceWhenCategoryIsChanged16()
        {
            #region Arrange
            var categoryies = GetCategoryies();
            categoryies[0].LastUpdate = new DateTime(2001,01,01);
            new FakeCategories(0, CategoryRepository, categoryies);
            var updatedCategory = CreateValidEntities.Category(9);
            updatedCategory.LastUpdate = new DateTime(2005,02,03);
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, updatedCategory);
            #endregion Act

            #region Assert

            #region New Values
            CategoryRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Category>.Is.Anything), options => options.Repeat.Times(2));
            var args1 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[0][0];
            Assert.IsNotNull(args1);
            Assert.AreEqual(result, args1);
            Assert.AreEqual(DateTime.Now.Date, args1.LastUpdate.Date);
            #endregion New Values

            #region Old Values
            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.AreEqual(new DateTime(2001, 01, 01), args2.LastUpdate);
            #endregion Old Values

            #endregion Assert
        }

        [TestMethod]
        public void TestArchiveServiceWhenCategoryIsChanged17()
        {
            #region Arrange
            var categoryies = GetCategoryies();
            categoryies[0].CreateDate = new DateTime(2001, 01, 01);
            new FakeCategories(0, CategoryRepository, categoryies);
            var updatedCategory = CreateValidEntities.Category(9);
            updatedCategory.CreateDate = new DateTime(2005, 02, 03);
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, updatedCategory);
            #endregion Act

            #region Assert

            #region New Values
            CategoryRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Category>.Is.Anything), options => options.Repeat.Times(2));
            var args1 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[0][0];
            Assert.IsNotNull(args1);
            Assert.AreEqual(result, args1);
            Assert.AreEqual(DateTime.Now.Date, args1.CreateDate.Date);
            #endregion New Values

            #region Old Values
            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.AreEqual(new DateTime(2001, 01, 01), args2.CreateDate);
            #endregion Old Values

            #endregion Assert
        }
        [TestMethod]
        public void TestArchiveServiceWhenCategoryIsChanged18()
        {
            #region Arrange
            var categoryies = GetCategoryies();
            categoryies[0].PreviousVersion = CreateValidEntities.Category(80);
            categoryies[0].PreviousVersion.SetIdTo(80);
            new FakeCategories(0, CategoryRepository, categoryies);
            var updatedCategory = CreateValidEntities.Category(9);
            updatedCategory.PreviousVersion = CreateValidEntities.Category(90);
            updatedCategory.PreviousVersion.SetIdTo(90);
            #endregion Arrange

            #region Act
            var result = ArchiveService.ArchiveCategory(Repository, 1, updatedCategory);
            #endregion Act

            #region Assert

            #region New Values
            CategoryRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Category>.Is.Anything), options => options.Repeat.Times(2));
            var args1 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[0][0];
            Assert.IsNotNull(args1);
            Assert.AreEqual(result, args1);
            Assert.AreEqual(1, args1.PreviousVersion.Id);
            #endregion New Values

            #region Old Values
            var args2 = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[1][0];
            Assert.IsNotNull(args2);
            Assert.AreEqual(80, args2.PreviousVersion.Id);
            #endregion Old Values

            #endregion Assert
        }
        #endregion Archive When Category Is Changed

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
