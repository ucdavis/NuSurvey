using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core;
using NuSurvey.Tests.Core.Helpers;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace NuSurvey.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		Answer
    /// LookupFieldName:	Score
    /// </summary>
    [TestClass]
    public class AnswerRepositoryTests : AbstractRepositoryTests<Answer, int, AnswerMap>
    {
        /// <summary>
        /// Gets or sets the Answer repository.
        /// </summary>
        /// <value>The Answer repository.</value>
        public IRepository<Answer> AnswerRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="AnswerRepositoryTests"/> class.
        /// </summary>
        public AnswerRepositoryTests()
        {
            AnswerRepository = new Repository<Answer>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Answer GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.Answer(counter);
            rtValue.Question = Repository.OfType<Question>().Queryable.First();
            rtValue.Response = Repository.OfType<Response>().Queryable.First();
            rtValue.Category = Repository.OfType<Category>().Queryable.First();
            rtValue.SurveyResponse = Repository.OfType<SurveyResponse>().Queryable.First();

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Answer> GetQuery(int numberAtEnd)
        {
            return AnswerRepository.Queryable.Where(a => a.Score == numberAtEnd);
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Answer entity, int counter)
        {
            Assert.AreEqual(counter, entity.Score);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Answer entity, ARTAction action)
        {
            const int updateValue = 998;
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Score);
                    break;
                case ARTAction.Restore:
                    entity.Score = IntRestoreValue.HasValue ? IntRestoreValue.Value:0;
                    break;
                case ARTAction.Update:
                    IntRestoreValue = entity.Score;
                    entity.Score = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<Survey>().DbContext.BeginTransaction();
            RepositoryLoad.LoadSurveys(Repository, 1);
            RepositoryLoad.LoadCategories(Repository, 3);
            RepositoryLoad.LoadQuestions(Repository, 3);
            RepositoryLoad.LoadResponses(Repository, 3);
            RepositoryLoad.LoadSurveyResponses(Repository, 3);
            Repository.OfType<Survey>().DbContext.CommitTransaction();
            AnswerRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            AnswerRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region Score Tests    

        /// <summary>
        /// Tests the Score with max int value saves.
        /// </summary>
        [TestMethod]
        public void TestScoreWithMaxIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Score = int.MaxValue;
            #endregion Arrange

            #region Act
            AnswerRepository.DbContext.BeginTransaction();
            AnswerRepository.EnsurePersistent(record);
            AnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MaxValue, record.Score);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Score with min int value saves.
        /// </summary>
        [TestMethod]
        public void TestScoreWithMinIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Score = int.MinValue;
            #endregion Arrange

            #region Act
            AnswerRepository.DbContext.BeginTransaction();
            AnswerRepository.EnsurePersistent(record);
            AnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MinValue, record.Score);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestScoreWithZeroValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Score = 0;
            #endregion Arrange

            #region Act
            AnswerRepository.DbContext.BeginTransaction();
            AnswerRepository.EnsurePersistent(record);
            AnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, record.Score);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        #endregion Score Tests

        #region OpenEndedAnswer Tests

        /// <summary>
        /// Tests the OpenEndedAnswer with null value saves.
        /// </summary>
        [TestMethod]
        public void TestOpenEndedAnswerWithNullValueSaves()
        {
            #region Arrange
            Answer record = GetValid(9);
            record.OpenEndedAnswer = null;
            #endregion Arrange

            #region Act
            AnswerRepository.DbContext.BeginTransaction();
            AnswerRepository.EnsurePersistent(record);
            AnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNull(record.OpenEndedAnswer);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        /// <summary>
        /// Tests the OpenEndedAnswer with max int value saves.
        /// </summary>
        [TestMethod]
        public void TestOpenEndedAnswerWithMaxIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.OpenEndedAnswer = int.MaxValue;
            #endregion Arrange

            #region Act
            AnswerRepository.DbContext.BeginTransaction();
            AnswerRepository.EnsurePersistent(record);
            AnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MaxValue, record.OpenEndedAnswer);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the OpenEndedAnswer with min int value saves.
        /// </summary>
        [TestMethod]
        public void TestOpenEndedAnswerWithMinIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.OpenEndedAnswer = int.MinValue;
            #endregion Arrange

            #region Act
            AnswerRepository.DbContext.BeginTransaction();
            AnswerRepository.EnsurePersistent(record);
            AnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MinValue, record.OpenEndedAnswer);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        #endregion OpenEndedAnswer Tests

        #region SurveyResponse Tests
        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestSurveyResponseWithNullValueDoesNotSave()
        {
            Answer record = null;
            var madeItThisFar = false;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.SurveyResponse = null;
                #endregion Arrange

                #region Act
                madeItThisFar = true;
                AnswerRepository.DbContext.BeginTransaction();
                AnswerRepository.EnsurePersistent(record);
                AnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsTrue(madeItThisFar);
                Assert.IsNotNull(record);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("{0}: The {0} field is required.", "SurveyResponse"));
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestSurveyResponseWithNewValueDoesNotSave()
        {
            var madeItThisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.SurveyResponse = CreateValidEntities.SurveyResponse(1);
                #endregion Arrange

                #region Act
                madeItThisFar = true;
                AnswerRepository.DbContext.BeginTransaction();
                AnswerRepository.EnsurePersistent(record);
                AnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(madeItThisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: NuSurvey.Core.Domain.SurveyResponse, Entity: NuSurvey.Core.Domain.SurveyResponse", ex.Message);
                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests

        [TestMethod]
        public void TestSurveyResponseWithExistingValueSaves()
        {
            #region Arrange
            var record = GetValid(99);            
            record.SurveyResponse = Repository.OfType<SurveyResponse>().GetNullableById(2);
            Assert.IsNotNull(record.SurveyResponse);
            #endregion Arrange

            #region Act
            AnswerRepository.DbContext.BeginTransaction();
            AnswerRepository.EnsurePersistent(record);
            AnswerRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(2, record.SurveyResponse.Id);
            #endregion Assert
        }
        #endregion Valid Tests

        #region Cascade Tests

        [TestMethod]
        public void TestDeleteAnswerDoesNotCascadeToSurveyResponse()
        {
            #region Arrange
            var record = GetValid(99);
            var relatedRecord = Repository.OfType<SurveyResponse>().GetNullableById(2);
            Assert.IsNotNull(relatedRecord);
            record.SurveyResponse = relatedRecord;
            
            AnswerRepository.DbContext.BeginTransaction();
            AnswerRepository.EnsurePersistent(record);
            AnswerRepository.DbContext.CommitChanges();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            NHibernateSessionManager.Instance.GetSession().Evict(record);

            record = AnswerRepository.GetNullableById(saveId);
            Assert.IsNotNull(record);
            #endregion Arrange

            #region Act
            AnswerRepository.DbContext.BeginTransaction();
            AnswerRepository.Remove(record);
            AnswerRepository.DbContext.CommitChanges();
            NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.IsNull(AnswerRepository.GetNullableById(saveId));
            Assert.IsNotNull(Repository.OfType<SurveyResponse>().GetNullableById(2));
            #endregion Assert
        }
        #endregion Cascade Tests
        #endregion SurveyResponse Tests 

        #region Category Tests
        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCategoryWithNullValueDoesNotSave()
        {
            Answer record = null;
            var madeItThisFar = false;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Category = null;
                #endregion Arrange

                #region Act
                madeItThisFar = true;
                AnswerRepository.DbContext.BeginTransaction();
                AnswerRepository.EnsurePersistent(record);
                AnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsTrue(madeItThisFar);
                Assert.IsNotNull(record);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("{0}: The {0} field is required.", "Category"));
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestCategoryWithNewValueDoesNotSave()
        {
            var madeItThisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.Category = CreateValidEntities.Category(1);
                #endregion Arrange

                #region Act
                madeItThisFar = true;
                AnswerRepository.DbContext.BeginTransaction();
                AnswerRepository.EnsurePersistent(record);
                AnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(madeItThisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: NuSurvey.Core.Domain.Category, Entity: NuSurvey.Core.Domain.Category", ex.Message);
                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests

        [TestMethod]
        public void TestCategoryWithExistingValueSaves()
        {
            #region Arrange
            var record = GetValid(99);            
            record.Category = Repository.OfType<Category>().GetNullableById(2);
            Assert.IsNotNull(record.Category);
            #endregion Arrange

            #region Act
            AnswerRepository.DbContext.BeginTransaction();
            AnswerRepository.EnsurePersistent(record);
            AnswerRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(2, record.Category.Id);
            #endregion Assert
        }
        #endregion Valid Tests

        #region Cascade Tests

        [TestMethod]
        public void TestDeleteAnswerDoesNotCascadeToCategory()
        {
            #region Arrange
            var record = GetValid(99);
            var relatedRecord = Repository.OfType<Category>().GetNullableById(2);
            Assert.IsNotNull(relatedRecord);
            record.Category = relatedRecord;
            
            AnswerRepository.DbContext.BeginTransaction();
            AnswerRepository.EnsurePersistent(record);
            AnswerRepository.DbContext.CommitChanges();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            NHibernateSessionManager.Instance.GetSession().Evict(record);

            record = AnswerRepository.GetNullableById(saveId);
            Assert.IsNotNull(record);
            #endregion Arrange

            #region Act
            AnswerRepository.DbContext.BeginTransaction();
            AnswerRepository.Remove(record);
            AnswerRepository.DbContext.CommitChanges();
            NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.IsNull(AnswerRepository.GetNullableById(saveId));
            Assert.IsNotNull(Repository.OfType<Category>().GetNullableById(2));
            #endregion Assert
        }
        #endregion Cascade Tests
        #endregion Category Tests 

        #region Question Tests
        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionWithNullValueDoesNotSave()
        {
            Answer record = null;
            var madeItThisFar = false;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Question = null;
                #endregion Arrange

                #region Act
                madeItThisFar = true;
                AnswerRepository.DbContext.BeginTransaction();
                AnswerRepository.EnsurePersistent(record);
                AnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsTrue(madeItThisFar);
                Assert.IsNotNull(record);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("{0}: The {0} field is required.", "Question"));
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestQuestionWithNewValueDoesNotSave()
        {
            var madeItThisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.Question = CreateValidEntities.Question(1);
                #endregion Arrange

                #region Act
                madeItThisFar = true;
                AnswerRepository.DbContext.BeginTransaction();
                AnswerRepository.EnsurePersistent(record);
                AnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(madeItThisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: NuSurvey.Core.Domain.Question, Entity: NuSurvey.Core.Domain.Question", ex.Message);
                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests

        [TestMethod]
        public void TestQuestionWithExistingValueSaves()
        {
            #region Arrange
            var record = GetValid(99);            
            record.Question = Repository.OfType<Question>().GetNullableById(2);
            Assert.IsNotNull(record.Question);
            #endregion Arrange

            #region Act
            AnswerRepository.DbContext.BeginTransaction();
            AnswerRepository.EnsurePersistent(record);
            AnswerRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(2, record.Question.Id);
            #endregion Assert
        }
        #endregion Valid Tests

        #region Cascade Tests

        [TestMethod]
        public void TestDeleteAnswerDoesNotCascadeToQuestion()
        {
            #region Arrange
            var record = GetValid(99);
            var relatedRecord = Repository.OfType<Question>().GetNullableById(2);
            Assert.IsNotNull(relatedRecord);
            record.Question = relatedRecord;
            
            AnswerRepository.DbContext.BeginTransaction();
            AnswerRepository.EnsurePersistent(record);
            AnswerRepository.DbContext.CommitChanges();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            NHibernateSessionManager.Instance.GetSession().Evict(record);

            record = AnswerRepository.GetNullableById(saveId);
            Assert.IsNotNull(record);
            #endregion Arrange

            #region Act
            AnswerRepository.DbContext.BeginTransaction();
            AnswerRepository.Remove(record);
            AnswerRepository.DbContext.CommitChanges();
            NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.IsNull(AnswerRepository.GetNullableById(saveId));
            Assert.IsNotNull(Repository.OfType<Question>().GetNullableById(2));
            #endregion Assert
        }
        #endregion Cascade Tests
        #endregion Question Tests 

        #region Response Tests
        #region Invalid Tests

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestResponseWithNewValueDoesNotSave()
        {
            var madeItThisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.Response = CreateValidEntities.Response(1);
                #endregion Arrange

                #region Act
                madeItThisFar = true;
                AnswerRepository.DbContext.BeginTransaction();
                AnswerRepository.EnsurePersistent(record);
                AnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(madeItThisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: NuSurvey.Core.Domain.Response, Entity: NuSurvey.Core.Domain.Response", ex.Message);
                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests

        [TestMethod]
        public void TestResponseWithNullValueSaves()
        {
            #region Arrange
            var record = GetValid(99);
            record.Response = null;
            #endregion Arrange

            #region Act
            AnswerRepository.DbContext.BeginTransaction();
            AnswerRepository.EnsurePersistent(record);
            AnswerRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestResponseWithExistingValueSaves()
        {
            #region Arrange
            var record = GetValid(99);            
            record.Response = Repository.OfType<Response>().GetNullableById(2);
            Assert.IsNotNull(record.Response);
            #endregion Arrange

            #region Act
            AnswerRepository.DbContext.BeginTransaction();
            AnswerRepository.EnsurePersistent(record);
            AnswerRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(2, record.Response.Id);
            #endregion Assert
        }
        #endregion Valid Tests

        #region Cascade Tests

        [TestMethod]
        public void TestDeleteAnswerDoesNotCascadeToResponse()
        {
            #region Arrange
            var record = GetValid(99);
            var relatedRecord = Repository.OfType<Response>().GetNullableById(2);
            Assert.IsNotNull(relatedRecord);
            record.Response = relatedRecord;
            
            AnswerRepository.DbContext.BeginTransaction();
            AnswerRepository.EnsurePersistent(record);
            AnswerRepository.DbContext.CommitChanges();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            NHibernateSessionManager.Instance.GetSession().Evict(record);

            record = AnswerRepository.GetNullableById(saveId);
            Assert.IsNotNull(record);
            #endregion Arrange

            #region Act
            AnswerRepository.DbContext.BeginTransaction();
            AnswerRepository.Remove(record);
            AnswerRepository.DbContext.CommitChanges();
            NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.IsNull(AnswerRepository.GetNullableById(saveId));
            Assert.IsNotNull(Repository.OfType<Response>().GetNullableById(2));
            #endregion Assert
        }
        #endregion Cascade Tests
        #endregion Response Tests 

        
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
            expectedFields.Add(new NameAndType("Category", "NuSurvey.Core.Domain.Category", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("OpenEndedAnswer", "System.Nullable`1[System.Int32]", new List<string>()));
            expectedFields.Add(new NameAndType("Question", "NuSurvey.Core.Domain.Question", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Response", "NuSurvey.Core.Domain.Response", new List<string>()));
            expectedFields.Add(new NameAndType("Score", "System.Int32", new List<string>()));
            expectedFields.Add(new NameAndType("SurveyResponse", "NuSurvey.Core.Domain.SurveyResponse", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Answer));

        }

        #endregion Reflection of Database.	
		
		
    }
}