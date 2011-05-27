using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core;
using NuSurvey.Tests.Core.Extensions;
using NuSurvey.Tests.Core.Helpers;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace NuSurvey.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		Survey
    /// LookupFieldName:	Name
    /// </summary>
    [TestClass]
    public class SurveyRepositoryTests : AbstractRepositoryTests<Survey, int, SurveyMap>
    {
        /// <summary>
        /// Gets or sets the Survey repository.
        /// </summary>
        /// <value>The Survey repository.</value>
        public IRepository<Survey> SurveyRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="SurveyRepositoryTests"/> class.
        /// </summary>
        public SurveyRepositoryTests()
        {
            SurveyRepository = new Repository<Survey>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Survey GetValid(int? counter)
        {
            return CreateValidEntities.Survey(counter);
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Survey> GetQuery(int numberAtEnd)
        {
            return SurveyRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Survey entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Survey entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Name);
                    break;
                case ARTAction.Restore:
                    entity.Name = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Name;
                    entity.Name = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            SurveyRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            SurveyRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	               

        #region Name Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Name with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithNullValueDoesNotSave()
        {
            Survey survey = null;
            try
            {
                #region Arrange
                survey = GetValid(9);
                survey.Name = null;
                #endregion Arrange

                #region Act
                SurveyRepository.DbContext.BeginTransaction();
                SurveyRepository.EnsurePersistent(survey);
                SurveyRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(survey);
                var results = survey.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: The Name field is required.");
                Assert.IsTrue(survey.IsTransient());
                Assert.IsFalse(survey.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Name with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithEmptyStringDoesNotSave()
        {
            Survey survey = null;
            try
            {
                #region Arrange
                survey = GetValid(9);
                survey.Name = string.Empty;
                #endregion Arrange

                #region Act
                SurveyRepository.DbContext.BeginTransaction();
                SurveyRepository.EnsurePersistent(survey);
                SurveyRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(survey);
                var results = survey.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: The Name field is required.");
                Assert.IsTrue(survey.IsTransient());
                Assert.IsFalse(survey.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Name with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithSpacesOnlyDoesNotSave()
        {
            Survey survey = null;
            try
            {
                #region Arrange
                survey = GetValid(9);
                survey.Name = " ";
                #endregion Arrange

                #region Act
                SurveyRepository.DbContext.BeginTransaction();
                SurveyRepository.EnsurePersistent(survey);
                SurveyRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(survey);
                var results = survey.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: The Name field is required.");
                Assert.IsTrue(survey.IsTransient());
                Assert.IsFalse(survey.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Name with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithTooLongValueDoesNotSave()
        {
            Survey survey = null;
            try
            {
                #region Arrange
                survey = GetValid(9);
                survey.Name = "x".RepeatTimes((100 + 1));
                #endregion Arrange

                #region Act
                SurveyRepository.DbContext.BeginTransaction();
                SurveyRepository.EnsurePersistent(survey);
                SurveyRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(survey);
                Assert.AreEqual(100 + 1, survey.Name.Length);
                var results = survey.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: The field Name must be a string with a maximum length of 100.");
                Assert.IsTrue(survey.IsTransient());
                Assert.IsFalse(survey.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Name with one character saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithOneCharacterSaves()
        {
            #region Arrange
            var survey = GetValid(9);
            survey.Name = "x";
            #endregion Arrange

            #region Act
            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(survey);
            SurveyRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(survey.IsTransient());
            Assert.IsTrue(survey.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var survey = GetValid(9);
            survey.Name = "x".RepeatTimes(100);
            #endregion Arrange

            #region Act
            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(survey);
            SurveyRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(100, survey.Name.Length);
            Assert.IsFalse(survey.IsTransient());
            Assert.IsTrue(survey.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests

        #region ShortName Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the ShortName with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestShortNameWithTooLongValueDoesNotSave()
        {
            Survey survey = null;
            try
            {
                #region Arrange
                survey = GetValid(9);
                survey.ShortName = "x".RepeatTimes((10 + 1));
                #endregion Arrange

                #region Act
                SurveyRepository.DbContext.BeginTransaction();
                SurveyRepository.EnsurePersistent(survey);
                SurveyRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(survey);
                Assert.AreEqual(10 + 1, survey.ShortName.Length);
                var results = survey.ValidationResults().AsMessageList();
                results.AssertErrorsAre("ShortName: The field ShortName must be a string with a maximum length of 10.");
                Assert.IsTrue(survey.IsTransient());
                Assert.IsFalse(survey.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the ShortName with null value saves.
        /// </summary>
        [TestMethod]
        public void TestShortNameWithNullValueSaves()
        {
            #region Arrange
            var survey = GetValid(9);
            survey.ShortName = null;
            #endregion Arrange

            #region Act
            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(survey);
            SurveyRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(survey.IsTransient());
            Assert.IsTrue(survey.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShortName with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestShortNameWithEmptyStringSaves()
        {
            #region Arrange
            var survey = GetValid(9);
            survey.ShortName = string.Empty;
            #endregion Arrange

            #region Act
            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(survey);
            SurveyRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(survey.IsTransient());
            Assert.IsTrue(survey.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShortName with one space saves.
        /// </summary>
        [TestMethod]
        public void TestShortNameWithOneSpaceSaves()
        {
            #region Arrange
            var survey = GetValid(9);
            survey.ShortName = " ";
            #endregion Arrange

            #region Act
            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(survey);
            SurveyRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(survey.IsTransient());
            Assert.IsTrue(survey.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShortName with one character saves.
        /// </summary>
        [TestMethod]
        public void TestShortNameWithOneCharacterSaves()
        {
            #region Arrange
            var survey = GetValid(9);
            survey.ShortName = "x";
            #endregion Arrange

            #region Act
            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(survey);
            SurveyRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(survey.IsTransient());
            Assert.IsTrue(survey.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShortName with long value saves.
        /// </summary>
        [TestMethod]
        public void TestShortNameWithLongValueSaves()
        {
            #region Arrange
            var survey = GetValid(9);
            survey.ShortName = "x".RepeatTimes(10);
            #endregion Arrange

            #region Act
            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(survey);
            SurveyRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(10, survey.ShortName.Length);
            Assert.IsFalse(survey.IsTransient());
            Assert.IsTrue(survey.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion ShortName Tests

        #region QuizType Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the QuizType with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuizTypeWithNullValueDoesNotSave()
        {
            Survey survey = null;
            try
            {
                #region Arrange
                survey = GetValid(9);
                survey.QuizType = null;
                #endregion Arrange

                #region Act
                SurveyRepository.DbContext.BeginTransaction();
                SurveyRepository.EnsurePersistent(survey);
                SurveyRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(survey);
                var results = survey.ValidationResults().AsMessageList();
                results.AssertErrorsAre("QuizType: The QuizType field is required.");
                Assert.IsTrue(survey.IsTransient());
                Assert.IsFalse(survey.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the QuizType with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuizTypeWithEmptyStringDoesNotSave()
        {
            Survey survey = null;
            try
            {
                #region Arrange
                survey = GetValid(9);
                survey.QuizType = string.Empty;
                #endregion Arrange

                #region Act
                SurveyRepository.DbContext.BeginTransaction();
                SurveyRepository.EnsurePersistent(survey);
                SurveyRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(survey);
                var results = survey.ValidationResults().AsMessageList();
                results.AssertErrorsAre("QuizType: The QuizType field is required.");
                Assert.IsTrue(survey.IsTransient());
                Assert.IsFalse(survey.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the QuizType with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuizTypeWithSpacesOnlyDoesNotSave()
        {
            Survey survey = null;
            try
            {
                #region Arrange
                survey = GetValid(9);
                survey.QuizType = " ";
                #endregion Arrange

                #region Act
                SurveyRepository.DbContext.BeginTransaction();
                SurveyRepository.EnsurePersistent(survey);
                SurveyRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(survey);
                var results = survey.ValidationResults().AsMessageList();
                results.AssertErrorsAre("QuizType: The QuizType field is required.");
                Assert.IsTrue(survey.IsTransient());
                Assert.IsFalse(survey.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the QuizType with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuizTypeWithTooLongValueDoesNotSave()
        {
            Survey survey = null;
            try
            {
                #region Arrange
                survey = GetValid(9);
                survey.QuizType = "x".RepeatTimes((100 + 1));
                #endregion Arrange

                #region Act
                SurveyRepository.DbContext.BeginTransaction();
                SurveyRepository.EnsurePersistent(survey);
                SurveyRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(survey);
                Assert.AreEqual(100 + 1, survey.QuizType.Length);
                var results = survey.ValidationResults().AsMessageList();
                results.AssertErrorsAre("QuizType: The field QuizType must be a string with a maximum length of 100.");
                Assert.IsTrue(survey.IsTransient());
                Assert.IsFalse(survey.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the QuizType with one character saves.
        /// </summary>
        [TestMethod]
        public void TestQuizTypeWithOneCharacterSaves()
        {
            #region Arrange
            var survey = GetValid(9);
            survey.QuizType = "x";
            #endregion Arrange

            #region Act
            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(survey);
            SurveyRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(survey.IsTransient());
            Assert.IsTrue(survey.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the QuizType with long value saves.
        /// </summary>
        [TestMethod]
        public void TestQuizTypeWithLongValueSaves()
        {
            #region Arrange
            var survey = GetValid(9);
            survey.QuizType = "x".RepeatTimes(100);
            #endregion Arrange

            #region Act
            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(survey);
            SurveyRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(100, survey.QuizType.Length);
            Assert.IsFalse(survey.IsTransient());
            Assert.IsTrue(survey.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion QuizType Tests

        #region IsActive Tests

        /// <summary>
        /// Tests the IsActive is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsFalseSaves()
        {
            #region Arrange

            Survey survey = GetValid(9);
            survey.IsActive = false;

            #endregion Arrange

            #region Act

            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(survey);
            SurveyRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(survey.IsActive);
            Assert.IsFalse(survey.IsTransient());
            Assert.IsTrue(survey.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the IsActive is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsTrueSaves()
        {
            #region Arrange

            var survey = GetValid(9);
            survey.IsActive = true;

            #endregion Arrange

            #region Act

            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(survey);
            SurveyRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(survey.IsActive);
            Assert.IsFalse(survey.IsTransient());
            Assert.IsTrue(survey.IsValid());

            #endregion Assert
        }

        #endregion IsActive Tests
        
        #region Categories Tests
        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCategoriesWithAValueOfNullDoesNotSave()
        {
            Survey record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Categories = null;
                #endregion Arrange

                #region Act
                SurveyRepository.DbContext.BeginTransaction();
                SurveyRepository.EnsurePersistent(record);
                SurveyRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.Categories, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Categories: The Categories field is required.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestCategoriesWithANewValueDoesNotSave()
        {
            Survey record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Categories.Add(CreateValidEntities.Category(1));
                #endregion Arrange

                #region Act
                SurveyRepository.DbContext.BeginTransaction();
                SurveyRepository.EnsurePersistent(record);
                SurveyRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(record);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: NuSurvey.Core.Domain.Category, Entity: NuSurvey.Core.Domain.Category", ex.Message);
                throw;
            }
        }

        #endregion Invalid Tests
        #region Valid Tests

        [TestMethod]
        public void TestCategoriesWithPopulatedExistingListWillSave()
        {
            #region Arrange
            Survey record = GetValid(9);
            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(record);
            SurveyRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<Category>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Category(i + 1));
                relatedRecords[i].Survey = record;
                Repository.OfType<Category>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.Categories.Add(relatedRecord);
            }
            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(record);
            SurveyRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Categories);
            Assert.AreEqual(addedCount, record.Categories.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestCategoriesWithEmptyListWillSave()
        {
            #region Arrange
            Survey record = GetValid(9);
            #endregion Arrange

            #region Act
            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(record);
            SurveyRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Categories);
            Assert.AreEqual(0, record.Categories.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests


        /// <summary>
        /// Does NOT Remove it
        /// </summary>
        [TestMethod]
        public void TestSurveyDoesNotCascadesUpdateRemoveCategory()
        {
            #region Arrange
            var count = Repository.OfType<Category>().Queryable.Count();
            Survey record = GetValid(9);
            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(record);
            SurveyRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<Category>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Category(i + 1));
                relatedRecords[i].Survey = record;
                Repository.OfType<Category>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Categories.Add(relatedRecord);
            }
            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(record);
            SurveyRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Categories[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = SurveyRepository.GetNullableById(saveId);
            record.Categories.RemoveAt(1);
            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(record);
            SurveyRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount), Repository.OfType<Category>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<Category>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            #endregion Assert
        }


        #endregion Cascade Tests

        #endregion ReportColumns Tests

        #region Questions Tests
        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionsWithAValueOfNullDoesNotSave()
        {
            Survey record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Questions = null;
                #endregion Arrange

                #region Act
                SurveyRepository.DbContext.BeginTransaction();
                SurveyRepository.EnsurePersistent(record);
                SurveyRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.Questions, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Questions: The Questions field is required.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestQuestionsWithANewValueDoesNotSave()
        {
            Survey record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Questions.Add(CreateValidEntities.Question(1));
                #endregion Arrange

                #region Act
                SurveyRepository.DbContext.BeginTransaction();
                SurveyRepository.EnsurePersistent(record);
                SurveyRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(record);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: NuSurvey.Core.Domain.Question, Entity: NuSurvey.Core.Domain.Question", ex.Message);
                throw;
            }
        }

        #endregion Invalid Tests
        #region Valid Tests

        [TestMethod]
        public void TestQuestionsWithPopulatedExistingListWillSave()
        {
            #region Arrange
            Survey record = GetValid(9);
            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(record);
            SurveyRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<Question>();
            var category = CreateValidEntities.Category(9);
            category.Survey = record;
            Repository.OfType<Category>().EnsurePersistent(category);
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Question(i + 1));
                relatedRecords[i].Survey = record;
                relatedRecords[i].Category = category;
                Repository.OfType<Question>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.Questions.Add(relatedRecord);
            }
            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(record);
            SurveyRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Questions);
            Assert.AreEqual(addedCount, record.Questions.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestQuestionsWithEmptyListWillSave()
        {
            #region Arrange
            Survey record = GetValid(9);
            #endregion Arrange

            #region Act
            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(record);
            SurveyRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Questions);
            Assert.AreEqual(0, record.Questions.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests


        /// <summary>
        /// Does NOT Remove it
        /// </summary>
        [TestMethod]
        public void TestSurveyDoesNotCascadesUpdateRemoveQuestion()
        {
            #region Arrange
            var count = Repository.OfType<Question>().Queryable.Count();
            Survey record = GetValid(9);
            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(record);
            SurveyRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<Question>();
            var category = CreateValidEntities.Category(9);
            category.Survey = record;
            Repository.OfType<Category>().EnsurePersistent(category);
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Question(i + 1));
                relatedRecords[i].Survey = record;
                relatedRecords[i].Category = category;
                Repository.OfType<Question>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Questions.Add(relatedRecord);
            }
            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(record);
            SurveyRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Questions[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = SurveyRepository.GetNullableById(saveId);
            record.Questions.RemoveAt(1);
            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(record);
            SurveyRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount), Repository.OfType<Question>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<Question>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            #endregion Assert
        }



        #endregion Cascade Tests

        #endregion Category Tests

        #region SurveyResponses Tests
        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestSurveyResponsesWithAValueOfNullDoesNotSave()
        {
            Survey record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.SurveyResponses = null;
                #endregion Arrange

                #region Act
                SurveyRepository.DbContext.BeginTransaction();
                SurveyRepository.EnsurePersistent(record);
                SurveyRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.SurveyResponses, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("SurveyResponses: The SurveyResponses field is required.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestSurveyResponsesWithANewValueDoesNotSave()
        {
            Survey record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.SurveyResponses.Add(CreateValidEntities.SurveyResponse(1));
                #endregion Arrange

                #region Act
                SurveyRepository.DbContext.BeginTransaction();
                SurveyRepository.EnsurePersistent(record);
                SurveyRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(record);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: NuSurvey.Core.Domain.SurveyResponse, Entity: NuSurvey.Core.Domain.SurveyResponse", ex.Message);
                throw;
            }
        }

        #endregion Invalid Tests
        #region Valid Tests


        [TestMethod]
        public void TestSurveyResponsesWithPopulatedExistingListWillSave()
        {
            #region Arrange
            Survey record = GetValid(9);
            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(record);
            SurveyRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<SurveyResponse>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.SurveyResponse(i + 1));
                relatedRecords[i].Survey = record;
                Repository.OfType<SurveyResponse>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.SurveyResponses.Add(relatedRecord);
            }
            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(record);
            SurveyRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.SurveyResponses);
            Assert.AreEqual(addedCount, record.SurveyResponses.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestSurveyResponsesWithEmptyListWillSave()
        {
            #region Arrange
            Survey record = GetValid(9);
            #endregion Arrange

            #region Act
            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(record);
            SurveyRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.SurveyResponses);
            Assert.AreEqual(0, record.SurveyResponses.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests


   
        /// <summary>
        /// Does NOT Remove it
        /// </summary>
        [TestMethod]
        public void TestSurveyDoesNotCascadesUpdateRemoveSurveyResponse()
        {
            #region Arrange
            var count = Repository.OfType<SurveyResponse>().Queryable.Count();
            Survey record = GetValid(9);
            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(record);
            SurveyRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<SurveyResponse>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.SurveyResponse(i + 1));
                relatedRecords[i].Survey = record;
                Repository.OfType<SurveyResponse>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.SurveyResponses.Add(relatedRecord);
            }
            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(record);
            SurveyRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.SurveyResponses[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = SurveyRepository.GetNullableById(saveId);
            record.SurveyResponses.RemoveAt(1);
            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(record);
            SurveyRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount), Repository.OfType<SurveyResponse>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<SurveyResponse>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            #endregion Assert
        }
		


        #endregion Cascade Tests

        #endregion SurveyResponses Tests

        #region Constructor Tests

        [TestMethod]
        public void TestConstructorWithoutParameterSetsExpectedValues()
        {
            #region Arrange
            var record = new Survey();
            #endregion Arrange

            #region Assert
            Assert.IsNotNull(record);
            Assert.IsFalse(record.IsActive);
            Assert.IsNotNull(record.Questions);
            Assert.IsNotNull(record.SurveyResponses);
            Assert.IsNotNull(record.Categories);
            #endregion Assert		
        }
        #endregion Constructor Tests
        
        #region Reflection of Database.

        /// <summary>
        /// Tests all fields in the database have been tested.
        /// If this fails and no other tests, it means that a field has been added which has not been tested above.
        /// </summary>
        [TestMethod]
        public void TestAllFieldsInTheDatabaseHaveBeenTested()
        {
            #region Arrange
            var expectedFields = new List<NameAndType>();
            expectedFields.Add(new NameAndType("Categories", "System.Collections.Generic.IList`1[NuSurvey.Core.Domain.Category]", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("IsActive", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DisplayNameAttribute(\"Active\")]"
            }));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)100)]"
            }));
            expectedFields.Add(new NameAndType("Questions", "System.Collections.Generic.IList`1[NuSurvey.Core.Domain.Question]", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("QuizType", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]",
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)100)]",              
                 "[System.ComponentModel.DisplayNameAttribute(\"Quiz Type\")]"
            }));
            expectedFields.Add(new NameAndType("ShortName", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)10)]",
                 "[System.ComponentModel.DisplayNameAttribute(\"Short Name\")]"
            }));
            expectedFields.Add(new NameAndType("SurveyResponses", "System.Collections.Generic.IList`1[NuSurvey.Core.Domain.SurveyResponse]", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Survey));

        }

        #endregion Reflection of Database.	
		
		
    }
}