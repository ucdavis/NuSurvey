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
    /// Entity Name:		CategoryGoal
    /// LookupFieldName:	Name
    /// </summary>
    [TestClass]
    public class CategoryGoalRepositoryTests : AbstractRepositoryTests<CategoryGoal, int, CategoryGoalMap>
    {
        /// <summary>
        /// Gets or sets the CategoryGoal repository.
        /// </summary>
        /// <value>The CategoryGoal repository.</value>
        public IRepository<CategoryGoal> CategoryGoalRepository { get; set; }
        public IRepository<Category> CategoryRepository { get; set; }
        public IRepository<Survey> SurveyRepository { get; set; }
        
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryGoalRepositoryTests"/> class.
        /// </summary>
        public CategoryGoalRepositoryTests()
        {
            CategoryGoalRepository = new Repository<CategoryGoal>();
            CategoryRepository = new Repository<Category>();
            SurveyRepository = new Repository<Survey>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override CategoryGoal GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.CategoryGoal(counter);
            rtValue.Category = CategoryRepository.Queryable.First();

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<CategoryGoal> GetQuery(int numberAtEnd)
        {
            return CategoryGoalRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(CategoryGoal entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(CategoryGoal entity, ARTAction action)
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
            CategoryRepository.DbContext.BeginTransaction();
            RepositoryLoad.LoadSurveys(Repository, 1);
            RepositoryLoad.LoadCategories(Repository, 3);
            CategoryRepository.DbContext.CommitTransaction();

            CategoryGoalRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            CategoryGoalRepository.DbContext.CommitTransaction();
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
            CategoryGoal categoryGoal = null;
            try
            {
                #region Arrange
                categoryGoal = GetValid(9);
                categoryGoal.Name = null;
                #endregion Arrange

                #region Act
                CategoryGoalRepository.DbContext.BeginTransaction();
                CategoryGoalRepository.EnsurePersistent(categoryGoal);
                CategoryGoalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(categoryGoal);
                var results = categoryGoal.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("{0}: The Goal field is required.", "Name"));
                Assert.IsTrue(categoryGoal.IsTransient());
                Assert.IsFalse(categoryGoal.IsValid());
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
            CategoryGoal categoryGoal = null;
            try
            {
                #region Arrange
                categoryGoal = GetValid(9);
                categoryGoal.Name = string.Empty;
                #endregion Arrange

                #region Act
                CategoryGoalRepository.DbContext.BeginTransaction();
                CategoryGoalRepository.EnsurePersistent(categoryGoal);
                CategoryGoalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(categoryGoal);
                var results = categoryGoal.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("{0}: The Goal field is required.", "Name"));
                Assert.IsTrue(categoryGoal.IsTransient());
                Assert.IsFalse(categoryGoal.IsValid());
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
            CategoryGoal categoryGoal = null;
            try
            {
                #region Arrange
                categoryGoal = GetValid(9);
                categoryGoal.Name = " ";
                #endregion Arrange

                #region Act
                CategoryGoalRepository.DbContext.BeginTransaction();
                CategoryGoalRepository.EnsurePersistent(categoryGoal);
                CategoryGoalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(categoryGoal);
                var results = categoryGoal.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("{0}: The Goal field is required.", "Name"));
                Assert.IsTrue(categoryGoal.IsTransient());
                Assert.IsFalse(categoryGoal.IsValid());
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
            CategoryGoal categoryGoal = null;
            try
            {
                #region Arrange
                categoryGoal = GetValid(9);
                categoryGoal.Name = "x".RepeatTimes((200 + 1));
                #endregion Arrange

                #region Act
                CategoryGoalRepository.DbContext.BeginTransaction();
                CategoryGoalRepository.EnsurePersistent(categoryGoal);
                CategoryGoalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(categoryGoal);
                Assert.AreEqual(200 + 1, categoryGoal.Name.Length);
                var results = categoryGoal.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("{0}: The field Goal must be a string with a maximum length of {1}.", "Name", "200"));
                Assert.IsTrue(categoryGoal.IsTransient());
                Assert.IsFalse(categoryGoal.IsValid());
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
            var categoryGoal = GetValid(9);
            categoryGoal.Name = "x";
            #endregion Arrange

            #region Act
            CategoryGoalRepository.DbContext.BeginTransaction();
            CategoryGoalRepository.EnsurePersistent(categoryGoal);
            CategoryGoalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(categoryGoal.IsTransient());
            Assert.IsTrue(categoryGoal.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var categoryGoal = GetValid(9);
            categoryGoal.Name = "x".RepeatTimes(200);
            #endregion Arrange

            #region Act
            CategoryGoalRepository.DbContext.BeginTransaction();
            CategoryGoalRepository.EnsurePersistent(categoryGoal);
            CategoryGoalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(200, categoryGoal.Name.Length);
            Assert.IsFalse(categoryGoal.IsTransient());
            Assert.IsTrue(categoryGoal.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests

        #region IsActive Tests

        /// <summary>
        /// Tests the IsActive is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsFalseSaves()
        {
            #region Arrange
            CategoryGoal categoryGoal = GetValid(9);
            categoryGoal.IsActive = false;
            #endregion Arrange

            #region Act
            CategoryGoalRepository.DbContext.BeginTransaction();
            CategoryGoalRepository.EnsurePersistent(categoryGoal);
            CategoryGoalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(categoryGoal.IsActive);
            Assert.IsFalse(categoryGoal.IsTransient());
            Assert.IsTrue(categoryGoal.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the IsActive is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsTrueSaves()
        {
            #region Arrange
            var categoryGoal = GetValid(9);
            categoryGoal.IsActive = true;
            #endregion Arrange

            #region Act
            CategoryGoalRepository.DbContext.BeginTransaction();
            CategoryGoalRepository.EnsurePersistent(categoryGoal);
            CategoryGoalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(categoryGoal.IsActive);
            Assert.IsFalse(categoryGoal.IsTransient());
            Assert.IsTrue(categoryGoal.IsValid());
            #endregion Assert
        }

        #endregion IsActive Tests

        #region Category Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the Category with A value of null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCategoryWithAValueOfNullDoesNotSave()
        {
            CategoryGoal categoryGoal = null;
            try
            {
                #region Arrange
                categoryGoal = GetValid(9);
                categoryGoal.Category = null;
                #endregion Arrange

                #region Act
                CategoryGoalRepository.DbContext.BeginTransaction();
                CategoryGoalRepository.EnsurePersistent(categoryGoal);
                CategoryGoalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(categoryGoal);
                Assert.AreEqual(categoryGoal.Category, null);
                var results = categoryGoal.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Category: The Category field is required.");
                Assert.IsTrue(categoryGoal.IsTransient());
                Assert.IsFalse(categoryGoal.IsValid());
                throw;
            }	
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestCategoryWithANewValueNotSave()
        {
            CategoryGoal categoryGoal = null;
            try
            {
                #region Arrange
                categoryGoal = GetValid(9);
                categoryGoal.Category = new Category(new Survey());
                #endregion Arrange

                #region Act
                CategoryGoalRepository.DbContext.BeginTransaction();
                CategoryGoalRepository.EnsurePersistent(categoryGoal);
                CategoryGoalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(categoryGoal);
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
            var record = CreateValidEntities.CategoryGoal(9);
            record.Category = Repository.OfType<Category>().Queryable.Where(a => a.Id == 2).Single();
            #endregion Arrange

            #region Act
            CategoryGoalRepository.DbContext.BeginTransaction();
            CategoryGoalRepository.EnsurePersistent(record);
            CategoryGoalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(2, record.Category.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }
        #endregion Valid Tests

        #region Cascade Tests

        [TestMethod]
        public void TestDeleteCategoryDoesNotCascade()
        {
            #region Arrange
            var record = CategoryGoalRepository.GetNullableById(2);
            Assert.IsNotNull(record);
            Assert.AreEqual(1, record.Category.Id);
            #endregion Arrange

            #region Act
            CategoryGoalRepository.DbContext.BeginTransaction();
            CategoryGoalRepository.Remove(record);
            CategoryGoalRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.IsNull(CategoryGoalRepository.GetNullableById(2));
            Assert.IsNotNull(Repository.OfType<Category>().GetNullableById(1));
            #endregion Assert		
        }
        #endregion Cascade Tests
        #endregion Category Tests

        #region Constructor Tests

        [TestMethod]
        public void TestConstructorWithNoParameter()
        {
            #region Arrange
            var record = new CategoryGoal();
            #endregion Arrange

            #region Assert
            Assert.IsTrue(record.IsActive);
            Assert.IsNull(record.Name);
            Assert.IsNull(record.Category);
            #endregion Assert		
        }

        [TestMethod]
        public void TestConstructorWithParameter()
        {
            #region Arrange
            var record = new CategoryGoal(Repository.OfType<Category>().GetNullableById(3));
            #endregion Arrange

            #region Assert
            Assert.IsTrue(record.IsActive);
            Assert.IsNull(record.Name);
            Assert.IsNotNull(record.Category);
            Assert.AreEqual(3, record.Category.Id);
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
            expectedFields.Add(new NameAndType("Category", "NuSurvey.Core.Domain.Category", new List<string>
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
                 "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Active\")]"
            }));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.DataTypeAttribute((System.ComponentModel.DataAnnotations.DataType)9)]", 
                 "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Goal\")]",
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]",
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)200)]"                 
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(CategoryGoal));

        }

        #endregion Reflection of Database.	
		
		
    }
}