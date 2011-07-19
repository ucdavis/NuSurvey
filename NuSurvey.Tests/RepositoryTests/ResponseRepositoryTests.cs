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
    /// Entity Name:		Response
    /// LookupFieldName:	Value
    /// </summary>
    [TestClass]
    public class ResponseRepositoryTests : AbstractRepositoryTests<Response, int, ResponseMap>
    {
        /// <summary>
        /// Gets or sets the Response repository.
        /// </summary>
        /// <value>The Response repository.</value>
        public IRepository<Response> ResponseRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseRepositoryTests"/> class.
        /// </summary>
        public ResponseRepositoryTests()
        {
            ResponseRepository = new Repository<Response>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Response GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.Response(counter);
            rtValue.Question = Repository.OfType<Question>().Queryable.First();

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Response> GetQuery(int numberAtEnd)
        {
            return ResponseRepository.Queryable.Where(a => a.Value.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Response entity, int counter)
        {
            Assert.AreEqual("Value" + counter, entity.Value);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Response entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Value);
                    break;
                case ARTAction.Restore:
                    entity.Value = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Value;
                    entity.Value = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<Question>().DbContext.BeginTransaction();
            RepositoryLoad.LoadSurveys(Repository, 1);
            RepositoryLoad.LoadCategories(Repository, 1);
            RepositoryLoad.LoadQuestions(Repository, 3);
            Repository.OfType<Question>().DbContext.CommitTransaction();
            ResponseRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            ResponseRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region Value Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Value with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestValueWithNullValueDoesNotSave()
        {
            Response response = null;
            try
            {
                #region Arrange
                response = GetValid(9);
                response.Value = null;
                #endregion Arrange

                #region Act
                ResponseRepository.DbContext.BeginTransaction();
                ResponseRepository.EnsurePersistent(response);
                ResponseRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(response);
                var results = response.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("{0}: The Choice field is required.", "Value"));
                Assert.IsTrue(response.IsTransient());
                Assert.IsFalse(response.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Value with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestValueWithEmptyStringDoesNotSave()
        {
            Response response = null;
            try
            {
                #region Arrange
                response = GetValid(9);
                response.Value = string.Empty;
                #endregion Arrange

                #region Act
                ResponseRepository.DbContext.BeginTransaction();
                ResponseRepository.EnsurePersistent(response);
                ResponseRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(response);
                var results = response.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("{0}: The Choice field is required.", "Value"));
                Assert.IsTrue(response.IsTransient());
                Assert.IsFalse(response.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Value with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestValueWithSpacesOnlyDoesNotSave()
        {
            Response response = null;
            try
            {
                #region Arrange
                response = GetValid(9);
                response.Value = " ";
                #endregion Arrange

                #region Act
                ResponseRepository.DbContext.BeginTransaction();
                ResponseRepository.EnsurePersistent(response);
                ResponseRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(response);
                var results = response.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("{0}: The Choice field is required.", "Value"));
                Assert.IsTrue(response.IsTransient());
                Assert.IsFalse(response.IsValid());
                throw;
            }
        }

        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Value with one character saves.
        /// </summary>
        [TestMethod]
        public void TestValueWithOneCharacterSaves()
        {
            #region Arrange
            var response = GetValid(9);
            response.Value = "x";
            #endregion Arrange

            #region Act
            ResponseRepository.DbContext.BeginTransaction();
            ResponseRepository.EnsurePersistent(response);
            ResponseRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(response.IsTransient());
            Assert.IsTrue(response.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Value with long value saves.
        /// </summary>
        [TestMethod]
        public void TestValueWithLongValueSaves()
        {
            #region Arrange
            var response = GetValid(9);
            response.Value = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            ResponseRepository.DbContext.BeginTransaction();
            ResponseRepository.EnsurePersistent(response);
            ResponseRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, response.Value.Length);
            Assert.IsFalse(response.IsTransient());
            Assert.IsTrue(response.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Value Tests

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
            ResponseRepository.DbContext.BeginTransaction();
            ResponseRepository.EnsurePersistent(record);
            ResponseRepository.DbContext.CommitTransaction();
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
            ResponseRepository.DbContext.BeginTransaction();
            ResponseRepository.EnsurePersistent(record);
            ResponseRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MinValue, record.Score);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestScoreWithZeroIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Score = 0;
            #endregion Arrange

            #region Act
            ResponseRepository.DbContext.BeginTransaction();
            ResponseRepository.EnsurePersistent(record);
            ResponseRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, record.Score);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        #endregion Score Tests
    
        #region Order Tests

        /// <summary>
        /// Tests the Order with max int value saves.
        /// </summary>
        [TestMethod]
        public void TestOrderWithMaxIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Order = int.MaxValue;
            #endregion Arrange

            #region Act
            ResponseRepository.DbContext.BeginTransaction();
            ResponseRepository.EnsurePersistent(record);
            ResponseRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MaxValue, record.Order);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Order with min int value saves.
        /// </summary>
        [TestMethod]
        public void TestOrderWithMinIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Order = int.MinValue;
            #endregion Arrange

            #region Act
            ResponseRepository.DbContext.BeginTransaction();
            ResponseRepository.EnsurePersistent(record);
            ResponseRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MinValue, record.Order);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        #endregion Order Tests

        #region IsActive Tests

        /// <summary>
        /// Tests the IsActive is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsFalseSaves()
        {
            #region Arrange
            Response response = GetValid(9);
            response.IsActive = false;
            #endregion Arrange

            #region Act
            ResponseRepository.DbContext.BeginTransaction();
            ResponseRepository.EnsurePersistent(response);
            ResponseRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(response.IsActive);
            Assert.IsFalse(response.IsTransient());
            Assert.IsTrue(response.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the IsActive is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsTrueSaves()
        {
            #region Arrange
            var response = GetValid(9);
            response.IsActive = true;
            #endregion Arrange

            #region Act
            ResponseRepository.DbContext.BeginTransaction();
            ResponseRepository.EnsurePersistent(response);
            ResponseRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(response.IsActive);
            Assert.IsFalse(response.IsTransient());
            Assert.IsTrue(response.IsValid());
            #endregion Assert
        }

        #endregion IsActive Tests

        #region Question Tests Tests
        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionWithNullValueDoesNotSave()
        {
            Response record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Question = null;
                #endregion Arrange

                #region Act
                ResponseRepository.DbContext.BeginTransaction();
                ResponseRepository.EnsurePersistent(record);
                ResponseRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
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
            Response record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Question = CreateValidEntities.Question(1);
                #endregion Arrange

                #region Act
                ResponseRepository.DbContext.BeginTransaction();
                ResponseRepository.EnsurePersistent(record);
                ResponseRepository.DbContext.CommitTransaction();
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
        public void TestQuestionWithExistingValueSaves()
        {
            #region Arrange
            Response record = GetValid(99);
            record.Question = Repository.OfType<Question>().GetNullableById(2);
            Assert.IsNotNull(record.Question);
            #endregion Arrange

            #region Act
            ResponseRepository.DbContext.BeginTransaction();
            ResponseRepository.EnsurePersistent(record);
            ResponseRepository.DbContext.CommitChanges();
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
        public void TestDeleteResponseDoesNotCascadeToQuestion()
        {
            #region Arrange
            Response record = GetValid(99);
            var question = Repository.OfType<Question>().GetNullableById(2);
            record.Question = question;
            Assert.IsNotNull(record.Question);

            ResponseRepository.DbContext.BeginTransaction();
            ResponseRepository.EnsurePersistent(record);
            ResponseRepository.DbContext.CommitChanges();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(question);
            NHibernateSessionManager.Instance.GetSession().Evict(record);

            record = ResponseRepository.GetNullableById(saveId);
            Assert.IsNotNull(record);
            #endregion Arrange

            #region Act
            ResponseRepository.DbContext.BeginTransaction();
            ResponseRepository.Remove(record);
            ResponseRepository.DbContext.CommitChanges();
            NHibernateSessionManager.Instance.GetSession().Evict(question);
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.IsNull(ResponseRepository.GetNullableById(saveId));
            Assert.IsNotNull(Repository.OfType<Question>().GetNullableById(2));
            #endregion Assert
        }
        #endregion Cascade Tests
        #endregion Question Tests Tests
             
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

            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("IsActive", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Order", "System.Int32", new List<string>()));
            expectedFields.Add(new NameAndType("Question", "NuSurvey.Core.Domain.Question", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Score", "System.Int32", new List<string>()));
            expectedFields.Add(new NameAndType("Value", "System.String", new List<string>
            {                  
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Choice\")]", 
                "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
                 
            }));
            
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Response));

        }

        #endregion Reflection of Database.	
		
		
    }
}