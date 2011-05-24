using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core;
using NuSurvey.Tests.Core.Extensions;
using NuSurvey.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentNHibernate.Testing;
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
            expectedFields.Add(new NameAndType("OpenEndedAnswer", "System.Int32", new List<string>()));
            expectedFields.Add(new NameAndType("Score", "System.Int32", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Answer));

        }

        #endregion Reflection of Database.	
		
		
    }
}