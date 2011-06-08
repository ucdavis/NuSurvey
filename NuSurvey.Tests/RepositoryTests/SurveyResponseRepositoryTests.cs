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
    /// Entity Name:		SurveyResponse
    /// LookupFieldName:	UserId
    /// </summary>
    [TestClass]
    public class SurveyResponseRepositoryTests : AbstractRepositoryTests<SurveyResponse, int, SurveyResponseMap>
    {
        /// <summary>
        /// Gets or sets the SurveyResponse repository.
        /// </summary>
        /// <value>The SurveyResponse repository.</value>
        public IRepository<SurveyResponse> SurveyResponseRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="SurveyResponseRepositoryTests"/> class.
        /// </summary>
        public SurveyResponseRepositoryTests()
        {
            SurveyResponseRepository = new Repository<SurveyResponse>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override SurveyResponse GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.SurveyResponse(counter);
            rtValue.Survey = Repository.OfType<Survey>().Queryable.First();

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<SurveyResponse> GetQuery(int numberAtEnd)
        {
            return SurveyResponseRepository.Queryable.Where(a => a.UserId.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(SurveyResponse entity, int counter)
        {
            Assert.AreEqual("UserId" + counter, entity.UserId);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(SurveyResponse entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.UserId);
                    break;
                case ARTAction.Restore:
                    entity.UserId = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.UserId;
                    entity.UserId = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<Survey>().DbContext.BeginTransaction();
            RepositoryLoad.LoadSurveys(Repository, 3);
            RepositoryLoad.LoadCategories(Repository, 3);
            RepositoryLoad.LoadQuestions(Repository, 1);
            RepositoryLoad.LoadResponses(Repository, 1);
            Repository.OfType<Survey>().DbContext.CommitTransaction();

            SurveyResponseRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            SurveyResponseRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region StudentId Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the StudentId with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestStudentIdWithNullValueDoesNotSave()
        {
            SurveyResponse surveyResponse = null;
            try
            {
                #region Arrange
                surveyResponse = GetValid(9);
                surveyResponse.StudentId = null;
                #endregion Arrange

                #region Act
                SurveyResponseRepository.DbContext.BeginTransaction();
                SurveyResponseRepository.EnsurePersistent(surveyResponse);
                SurveyResponseRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(surveyResponse);
                var results = surveyResponse.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("{0}: The {0} field is required.", "StudentId"));
                Assert.IsTrue(surveyResponse.IsTransient());
                Assert.IsFalse(surveyResponse.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the StudentId with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestStudentIdWithEmptyStringDoesNotSave()
        {
            SurveyResponse surveyResponse = null;
            try
            {
                #region Arrange
                surveyResponse = GetValid(9);
                surveyResponse.StudentId = string.Empty;
                #endregion Arrange

                #region Act
                SurveyResponseRepository.DbContext.BeginTransaction();
                SurveyResponseRepository.EnsurePersistent(surveyResponse);
                SurveyResponseRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(surveyResponse);
                var results = surveyResponse.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("{0}: The {0} field is required.", "StudentId"));
                Assert.IsTrue(surveyResponse.IsTransient());
                Assert.IsFalse(surveyResponse.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the StudentId with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestStudentIdWithSpacesOnlyDoesNotSave()
        {
            SurveyResponse surveyResponse = null;
            try
            {
                #region Arrange
                surveyResponse = GetValid(9);
                surveyResponse.StudentId = " ";
                #endregion Arrange

                #region Act
                SurveyResponseRepository.DbContext.BeginTransaction();
                SurveyResponseRepository.EnsurePersistent(surveyResponse);
                SurveyResponseRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(surveyResponse);
                var results = surveyResponse.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("{0}: The {0} field is required.", "StudentId"));
                Assert.IsTrue(surveyResponse.IsTransient());
                Assert.IsFalse(surveyResponse.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the StudentId with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestStudentIdWithTooLongValueDoesNotSave()
        {
            SurveyResponse surveyResponse = null;
            try
            {
                #region Arrange
                surveyResponse = GetValid(9);
                surveyResponse.StudentId = "x".RepeatTimes((10 + 1));
                #endregion Arrange

                #region Act
                SurveyResponseRepository.DbContext.BeginTransaction();
                SurveyResponseRepository.EnsurePersistent(surveyResponse);
                SurveyResponseRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(surveyResponse);
                Assert.AreEqual(10 + 1, surveyResponse.StudentId.Length);
                var results = surveyResponse.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("{0}: The field {0} must be a string with a maximum length of {1}.", "StudentId", "10"));
                Assert.IsTrue(surveyResponse.IsTransient());
                Assert.IsFalse(surveyResponse.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the StudentId with one character saves.
        /// </summary>
        [TestMethod]
        public void TestStudentIdWithOneCharacterSaves()
        {
            #region Arrange
            var surveyResponse = GetValid(9);
            surveyResponse.StudentId = "x";
            #endregion Arrange

            #region Act
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(surveyResponse);
            SurveyResponseRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(surveyResponse.IsTransient());
            Assert.IsTrue(surveyResponse.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the StudentId with long value saves.
        /// </summary>
        [TestMethod]
        public void TestStudentIdWithLongValueSaves()
        {
            #region Arrange
            var surveyResponse = GetValid(9);
            surveyResponse.StudentId = "x".RepeatTimes(10);
            #endregion Arrange

            #region Act
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(surveyResponse);
            SurveyResponseRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(10, surveyResponse.StudentId.Length);
            Assert.IsFalse(surveyResponse.IsTransient());
            Assert.IsTrue(surveyResponse.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion StudentId Tests

        #region DateTaken Tests

        /// <summary>
        /// Tests the DateTaken with past date will save.
        /// </summary>
        [TestMethod]
        public void TestDateTakenWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(-10);
            SurveyResponse record = GetValid(99);
            record.DateTaken = compareDate;
            #endregion Arrange

            #region Act
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(record);
            SurveyResponseRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateTaken);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the DateTaken with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestDateTakenWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now;
            var record = GetValid(99);
            record.DateTaken = compareDate;
            #endregion Arrange

            #region Act
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(record);
            SurveyResponseRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateTaken);
            #endregion Assert
        }

        /// <summary>
        /// Tests the DateTaken with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestDateTakenWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(15);
            var record = GetValid(99);
            record.DateTaken = compareDate;
            #endregion Arrange

            #region Act
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(record);
            SurveyResponseRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateTaken);
            #endregion Assert
        }
        #endregion DateTaken Tests
        
        #region UserId Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the UserId with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestUserIdWithNullValueDoesNotSave()
        {
            SurveyResponse surveyResponse = null;
            try
            {
                #region Arrange
                surveyResponse = GetValid(9);
                surveyResponse.UserId = null;
                #endregion Arrange

                #region Act
                SurveyResponseRepository.DbContext.BeginTransaction();
                SurveyResponseRepository.EnsurePersistent(surveyResponse);
                SurveyResponseRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(surveyResponse);
                var results = surveyResponse.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("{0}: The {0} field is required.", "UserId"));
                Assert.IsTrue(surveyResponse.IsTransient());
                Assert.IsFalse(surveyResponse.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the UserId with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestUserIdWithEmptyStringDoesNotSave()
        {
            SurveyResponse surveyResponse = null;
            try
            {
                #region Arrange
                surveyResponse = GetValid(9);
                surveyResponse.UserId = string.Empty;
                #endregion Arrange

                #region Act
                SurveyResponseRepository.DbContext.BeginTransaction();
                SurveyResponseRepository.EnsurePersistent(surveyResponse);
                SurveyResponseRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(surveyResponse);
                var results = surveyResponse.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("{0}: The {0} field is required.", "UserId"));
                Assert.IsTrue(surveyResponse.IsTransient());
                Assert.IsFalse(surveyResponse.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the UserId with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestUserIdWithSpacesOnlyDoesNotSave()
        {
            SurveyResponse surveyResponse = null;
            try
            {
                #region Arrange
                surveyResponse = GetValid(9);
                surveyResponse.UserId = " ";
                #endregion Arrange

                #region Act
                SurveyResponseRepository.DbContext.BeginTransaction();
                SurveyResponseRepository.EnsurePersistent(surveyResponse);
                SurveyResponseRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(surveyResponse);
                var results = surveyResponse.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("{0}: The {0} field is required.", "UserId"));
                Assert.IsTrue(surveyResponse.IsTransient());
                Assert.IsFalse(surveyResponse.IsValid());
                throw;
            }
        }

        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the UserId with one character saves.
        /// </summary>
        [TestMethod]
        public void TestUserIdWithOneCharacterSaves()
        {
            #region Arrange
            var surveyResponse = GetValid(9);
            surveyResponse.UserId = "x";
            #endregion Arrange

            #region Act
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(surveyResponse);
            SurveyResponseRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(surveyResponse.IsTransient());
            Assert.IsTrue(surveyResponse.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the UserId with long value saves.
        /// </summary>
        [TestMethod]
        public void TestUserIdWithLongValueSaves()
        {
            #region Arrange
            var surveyResponse = GetValid(9);
            surveyResponse.UserId = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(surveyResponse);
            SurveyResponseRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, surveyResponse.UserId.Length);
            Assert.IsFalse(surveyResponse.IsTransient());
            Assert.IsTrue(surveyResponse.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion UserId Tests

        #region Survey Tests
        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestSurveyWithNullValueDoesNotSave()
        {
            SurveyResponse record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Survey = null;
                #endregion Arrange

                #region Act
                SurveyResponseRepository.DbContext.BeginTransaction();
                SurveyResponseRepository.EnsurePersistent(record);
                SurveyResponseRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("{0}: The {0} field is required.", "Survey"));
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestSurveyWithNewValueDoesNotSave()
        {
            SurveyResponse record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Survey = CreateValidEntities.Survey(1);
                #endregion Arrange

                #region Act
                SurveyResponseRepository.DbContext.BeginTransaction();
                SurveyResponseRepository.EnsurePersistent(record);
                SurveyResponseRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(record);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: NuSurvey.Core.Domain.Survey, Entity: NuSurvey.Core.Domain.Survey", ex.Message);
                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests

        [TestMethod]
        public void TestSurveyWithExistingValueSaves()
        {
            #region Arrange
            SurveyResponse record = GetValid(99);
            record.Survey = Repository.OfType<Survey>().GetNullableById(2);
            Assert.IsNotNull(record.Survey);
            #endregion Arrange

            #region Act
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(record);
            SurveyResponseRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(2, record.Survey.Id);
            #endregion Assert
        }
        #endregion Valid Tests

        #region Cascade Tests

        [TestMethod]
        public void TestDeleteQuestionDoesNotCascadeToSurvey()
        {
            #region Arrange
            SurveyResponse record = GetValid(99);
            var survey = Repository.OfType<Survey>().GetNullableById(2);
            record.Survey = survey;
            Assert.IsNotNull(record.Survey);

            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(record);
            SurveyResponseRepository.DbContext.CommitChanges();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(survey);
            NHibernateSessionManager.Instance.GetSession().Evict(record);

            record = SurveyResponseRepository.GetNullableById(saveId);
            Assert.IsNotNull(record);
            #endregion Arrange

            #region Act
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.Remove(record);
            SurveyResponseRepository.DbContext.CommitChanges();
            NHibernateSessionManager.Instance.GetSession().Evict(survey);
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.IsNull(SurveyResponseRepository.GetNullableById(saveId));
            Assert.IsNotNull(Repository.OfType<Survey>().GetNullableById(2));
            #endregion Assert
        }
        #endregion Cascade Tests
        #endregion Survey Tests

        #region PositiveCategory Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the PositiveCategory with A value of new does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestPositiveCategoryWithAValueOfNewDoesNotSave()
        {
            SurveyResponse surveyResponse = null;
            try
            {
                #region Arrange
                surveyResponse = GetValid(9);
                surveyResponse.PositiveCategory = CreateValidEntities.Category(1);
                #endregion Arrange

                #region Act
                SurveyResponseRepository.DbContext.BeginTransaction();
                SurveyResponseRepository.EnsurePersistent(surveyResponse);
                SurveyResponseRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(surveyResponse);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: NuSurvey.Core.Domain.Category, Entity: NuSurvey.Core.Domain.Category", ex.Message);
                throw;
            }	
        }
        #endregion Invalid Tests
        #region Valid Tests

        [TestMethod]
        public void TestPositiveCategoryWithExistingValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.PositiveCategory = Repository.OfType<Category>().GetNullableById(2);
            Assert.IsNotNull(record.PositiveCategory);
            #endregion Arrange

            #region Act
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(record);
            SurveyResponseRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.PositiveCategory);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }
        #endregion Valid Tests

        #region Cascade Tests

        [TestMethod]
        public void TestDeleteSurveyResponseDoesNotCascadeToCategoryViaPositivcategory()
        {
            #region Arrange
            var record = GetValid(9);
            var category = Repository.OfType<Category>().GetNullableById(2);
            Assert.IsNotNull(category);
            record.PositiveCategory = category;            
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(record);
            SurveyResponseRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(category);
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            record = SurveyResponseRepository.GetNullableById(saveId);
            Assert.IsNotNull(record);
            #endregion Arrange

            #region Act
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.Remove(record);
            SurveyResponseRepository.DbContext.CommitTransaction();            
            #endregion Act

            #region Assert
            Assert.IsNull(SurveyResponseRepository.GetNullableById(saveId));
            Assert.IsNotNull(Repository.OfType<Category>().GetNullableById(2));
            #endregion Assert		
        }
        #endregion Cascade Tests
        #endregion PositiveCategory Tests

        #region NegativeCategory1 Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the NegativeCategory1 with A value of new does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestNegativeCategory1WithAValueOfNewDoesNotSave()
        {
            SurveyResponse surveyResponse = null;
            try
            {
                #region Arrange
                surveyResponse = GetValid(9);
                surveyResponse.NegativeCategory1 = CreateValidEntities.Category(1);
                #endregion Arrange

                #region Act
                SurveyResponseRepository.DbContext.BeginTransaction();
                SurveyResponseRepository.EnsurePersistent(surveyResponse);
                SurveyResponseRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(surveyResponse);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: NuSurvey.Core.Domain.Category, Entity: NuSurvey.Core.Domain.Category", ex.Message);
                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests

        [TestMethod]
        public void TestNegativeCategory1WithExistingValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.NegativeCategory1 = Repository.OfType<Category>().GetNullableById(2);
            Assert.IsNotNull(record.NegativeCategory1);
            #endregion Arrange

            #region Act
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(record);
            SurveyResponseRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.NegativeCategory1);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests

        #region Cascade Tests

        [TestMethod]
        public void TestDeleteSurveyResponseDoesNotCascadeToCategoryViaNegativeCategory1()
        {
            #region Arrange
            var record = GetValid(9);
            var category = Repository.OfType<Category>().GetNullableById(2);
            Assert.IsNotNull(category);
            record.NegativeCategory1 = category;
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(record);
            SurveyResponseRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(category);
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            record = SurveyResponseRepository.GetNullableById(saveId);
            Assert.IsNotNull(record);
            #endregion Arrange

            #region Act
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.Remove(record);
            SurveyResponseRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNull(SurveyResponseRepository.GetNullableById(saveId));
            Assert.IsNotNull(Repository.OfType<Category>().GetNullableById(2));
            #endregion Assert
        }
        #endregion Cascade Tests
        #endregion NegativeCategory1 Tests

        #region NegativeCategory2 Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the NegativeCategory2 with A value of new does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestNegativeCategory2WithAValueOfNewDoesNotSave()
        {
            SurveyResponse surveyResponse = null;
            try
            {
                #region Arrange
                surveyResponse = GetValid(9);
                surveyResponse.NegativeCategory2 = CreateValidEntities.Category(1);
                #endregion Arrange

                #region Act
                SurveyResponseRepository.DbContext.BeginTransaction();
                SurveyResponseRepository.EnsurePersistent(surveyResponse);
                SurveyResponseRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(surveyResponse);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: NuSurvey.Core.Domain.Category, Entity: NuSurvey.Core.Domain.Category", ex.Message);
                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests

        [TestMethod]
        public void TestNegativeCategory2WithExistingValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.NegativeCategory2 = Repository.OfType<Category>().GetNullableById(2);
            Assert.IsNotNull(record.NegativeCategory2);
            #endregion Arrange

            #region Act
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(record);
            SurveyResponseRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.NegativeCategory2);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests

        #region Cascade Tests

        [TestMethod]
        public void TestDeleteSurveyResponseDoesNotCascadeToCategoryViaNegativeCategory2()
        {
            #region Arrange
            var record = GetValid(9);
            var category = Repository.OfType<Category>().GetNullableById(2);
            Assert.IsNotNull(category);
            record.NegativeCategory2 = category;
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(record);
            SurveyResponseRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(category);
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            record = SurveyResponseRepository.GetNullableById(saveId);
            Assert.IsNotNull(record);
            #endregion Arrange

            #region Act
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.Remove(record);
            SurveyResponseRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNull(SurveyResponseRepository.GetNullableById(saveId));
            Assert.IsNotNull(Repository.OfType<Category>().GetNullableById(2));
            #endregion Assert
        }
        #endregion Cascade Tests
        #endregion NegativeCategory2 Tests

        #region Answers Tests
        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAnswersWithAValueOfNullDoesNotSave()
        {
            SurveyResponse record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Answers = null;
                #endregion Arrange

                #region Act
                SurveyResponseRepository.DbContext.BeginTransaction();
                SurveyResponseRepository.EnsurePersistent(record);
                SurveyResponseRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.Answers, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Answers: The Answers field is required.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        #endregion Invalid Tests
        #region Valid Tests
        [TestMethod]
        public void TestAnswersWithPopulatedListWillSave()
        {

            #region Arrange
            SurveyResponse record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddAnswers(CreateValidEntities.Answer(i+1));
                record.Answers[i].Category = Repository.OfType<Category>().GetNullableById(1);
                record.Answers[i].Question = Repository.OfType<Question>().GetNullableById(1);
                record.Answers[i].Response = Repository.OfType<Response>().GetNullableById(1);
            }
            #endregion Arrange

            #region Act
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(record);
            SurveyResponseRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Answers);
            Assert.AreEqual(addedCount, record.Answers.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswersWithPopulatedExistingListWillSave()
        {
            #region Arrange
            SurveyResponse record = GetValid(9);
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(record);
            SurveyResponseRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<Answer>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Answer(i + 1));
                relatedRecords[i].SurveyResponse = record;
                relatedRecords[i].Category = Repository.OfType<Category>().GetNullableById(1);
                relatedRecords[i].Question = Repository.OfType<Question>().GetNullableById(1);
                relatedRecords[i].Response = Repository.OfType<Response>().GetNullableById(1);
                Repository.OfType<Answer>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.Answers.Add(relatedRecord);
            }
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(record);
            SurveyResponseRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Answers);
            Assert.AreEqual(addedCount, record.Answers.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswersWithEmptyListWillSave()
        {
            #region Arrange
            SurveyResponse record = GetValid(9);
            #endregion Arrange

            #region Act
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(record);
            SurveyResponseRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Answers);
            Assert.AreEqual(0, record.Answers.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests


        [TestMethod]
        public void TestSurveyResponseCascadesSaveToAnswer()
        {
            #region Arrange
            var count = Repository.OfType<Answer>().Queryable.Count();
            SurveyResponse record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddAnswers(CreateValidEntities.Answer(i+1));
                record.Answers[i].Category = Repository.OfType<Category>().GetNullableById(1);
                record.Answers[i].Question = Repository.OfType<Question>().GetNullableById(1);
                record.Answers[i].Response = Repository.OfType<Response>().GetNullableById(1);
            }

            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(record);
            SurveyResponseRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = SurveyResponseRepository.GetNullableById(saveId);
            #endregion Act

            #region Assert
            Assert.IsNotNull(record);
            Assert.AreEqual(addedCount, record.Answers.Count);
            Assert.AreEqual(count + addedCount, Repository.OfType<Answer>().Queryable.Count());
            #endregion Assert
        }


        [TestMethod]
        public void TestSurveyResponseCascadesUpdateToAnswer1()
        {
            #region Arrange
            var count = Repository.OfType<Answer>().Queryable.Count();
            SurveyResponse record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddAnswers(CreateValidEntities.Answer(i + 1));
                record.Answers[i].Category = Repository.OfType<Category>().GetNullableById(1);
                record.Answers[i].Question = Repository.OfType<Question>().GetNullableById(1);
                record.Answers[i].Response = Repository.OfType<Response>().GetNullableById(1);
            }

            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(record);
            SurveyResponseRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Answers[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = SurveyResponseRepository.GetNullableById(saveId);
            record.Answers[1].Score = 456;
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(record);
            SurveyResponseRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<Answer>().Queryable.Count());
            var relatedRecord = Repository.OfType<Answer>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord);
            Assert.AreEqual(456, relatedRecord.Score);
            #endregion Assert
        }

        [TestMethod]
        public void TestSurveyResponseCascadesUpdateToAnswer2()
        {
            #region Arrange
            var count = Repository.OfType<Answer>().Queryable.Count();
            SurveyResponse record = GetValid(9);
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(record);
            SurveyResponseRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<Answer>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Answer(i + 1));
                relatedRecords[i].SurveyResponse = record;
                relatedRecords[i].Category = Repository.OfType<Category>().GetNullableById(1);
                relatedRecords[i].Question = Repository.OfType<Question>().GetNullableById(1);
                relatedRecords[i].Response = Repository.OfType<Response>().GetNullableById(1);
                Repository.OfType<Answer>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Answers.Add(relatedRecord);
            }
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(record);
            SurveyResponseRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Answers[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Arrange

            #region Act
            record = SurveyResponseRepository.GetNullableById(saveId);
            record.Answers[1].Score = 123;
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(record);
            SurveyResponseRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<Answer>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<Answer>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            Assert.AreEqual(123, relatedRecord2.Score);
            #endregion Assert
        }

        /// <summary>
        /// Does Remove it 
        /// </summary>
        [TestMethod]
        public void TestSurveyResponseCascadesUpdateRemoveAnswer()
        {
            #region Arrange
            var count = Repository.OfType<Answer>().Queryable.Count();
            SurveyResponse record = GetValid(9);
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(record);
            SurveyResponseRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<Answer>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Answer(i + 1));
                relatedRecords[i].SurveyResponse = record;
                relatedRecords[i].Category = Repository.OfType<Category>().GetNullableById(1);
                relatedRecords[i].Question = Repository.OfType<Question>().GetNullableById(1);
                relatedRecords[i].Response = Repository.OfType<Response>().GetNullableById(1);
                Repository.OfType<Answer>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Answers.Add(relatedRecord);
            }
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(record);
            SurveyResponseRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Answers[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = SurveyResponseRepository.GetNullableById(saveId);
            record.Answers.RemoveAt(1);
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(record);
            SurveyResponseRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount - 1), Repository.OfType<Answer>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<Answer>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }


        [TestMethod]
        public void TestSurveyResponseCascadesDeleteToAnswer()
        {
            #region Arrange
            var count = Repository.OfType<Answer>().Queryable.Count();
            SurveyResponse record = GetValid(9);
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(record);
            SurveyResponseRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<Answer>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Answer(i + 1));
                relatedRecords[i].SurveyResponse = record;
                relatedRecords[i].Category = Repository.OfType<Category>().GetNullableById(1);
                relatedRecords[i].Question = Repository.OfType<Question>().GetNullableById(1);
                relatedRecords[i].Response = Repository.OfType<Response>().GetNullableById(1);
                Repository.OfType<Answer>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Answers.Add(relatedRecord);
            }
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(record);
            SurveyResponseRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Answers[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = SurveyResponseRepository.GetNullableById(saveId);
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.Remove(record);
            SurveyResponseRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count, Repository.OfType<Answer>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<Answer>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }
		


        #endregion Cascade Tests

        #endregion Answers Tests

        #region IsPending Tests

        /// <summary>
        /// Tests the IsPending is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsPendingIsFalseSaves()
        {
            #region Arrange
            SurveyResponse surveyResponse = GetValid(9);
            surveyResponse.IsPending = false;
            #endregion Arrange

            #region Act
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(surveyResponse);
            SurveyResponseRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(surveyResponse.IsPending);
            Assert.IsFalse(surveyResponse.IsTransient());
            Assert.IsTrue(surveyResponse.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the IsPending is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsPendingIsTrueSaves()
        {
            #region Arrange
            var surveyResponse = GetValid(9);
            surveyResponse.IsPending = true;
            #endregion Arrange

            #region Act
            SurveyResponseRepository.DbContext.BeginTransaction();
            SurveyResponseRepository.EnsurePersistent(surveyResponse);
            SurveyResponseRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(surveyResponse.IsPending);
            Assert.IsFalse(surveyResponse.IsTransient());
            Assert.IsTrue(surveyResponse.IsValid());
            #endregion Assert
        }

        #endregion IsPending Tests        
    
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
            expectedFields.Add(new NameAndType("Answers", "System.Collections.Generic.IList`1[NuSurvey.Core.Domain.Answer]", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("DateTaken", "System.DateTime", new List<string>
            {
                "[System.ComponentModel.DisplayNameAttribute(\"Date Taken\")]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("IsPending", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("NegativeCategory1", "NuSurvey.Core.Domain.Category", new List<string>()));
            expectedFields.Add(new NameAndType("NegativeCategory2", "NuSurvey.Core.Domain.Category", new List<string>()));
            expectedFields.Add(new NameAndType("PositiveCategory", "NuSurvey.Core.Domain.Category", new List<string>()));
            expectedFields.Add(new NameAndType("StudentId", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.RequiredAttribute()]",
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)10)]",
                "[System.ComponentModel.DisplayNameAttribute(\"Name\")]"
            }));
            expectedFields.Add(new NameAndType("Survey", "NuSurvey.Core.Domain.Survey", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("UserId", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]",
                 "[System.ComponentModel.DisplayNameAttribute(\"User Id\")]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(SurveyResponse));

        }

        #endregion Reflection of Database.	
			
    }
}