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
    /// Entity Name:		Category
    /// LookupFieldName:	Name
    /// </summary>
    [TestClass]
    public class CategoryRepositoryTests : AbstractRepositoryTests<Category, int, CategoryMap>
    {
        /// <summary>
        /// Gets or sets the Category repository.
        /// </summary>
        /// <value>The Category repository.</value>
        public IRepository<Category> CategoryRepository { get; set; }
        public IRepository<Survey> SurveyRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryRepositoryTests"/> class.
        /// </summary>
        public CategoryRepositoryTests()
        {
            CategoryRepository = new Repository<Category>();
            SurveyRepository = new Repository<Survey>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Category GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.Category(counter);
            rtValue.Survey = SurveyRepository.Queryable.First();

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Category> GetQuery(int numberAtEnd)
        {
            return CategoryRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Category entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Category entity, ARTAction action)
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
            RepositoryLoad.LoadSurveys(SurveyRepository, 3); 
            SurveyRepository.DbContext.CommitTransaction();

            CategoryRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            CategoryRepository.DbContext.CommitTransaction();
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
            Category category = null;
            try
            {
                #region Arrange
                category = GetValid(9);
                category.Name = null;
                #endregion Arrange

                #region Act
                CategoryRepository.DbContext.BeginTransaction();
                CategoryRepository.EnsurePersistent(category);
                CategoryRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(category);
                var results = category.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("{0}: The {0} field is required.", "Name"));
                Assert.IsTrue(category.IsTransient());
                Assert.IsFalse(category.IsValid());
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
            Category category = null;
            try
            {
                #region Arrange
                category = GetValid(9);
                category.Name = string.Empty;
                #endregion Arrange

                #region Act
                CategoryRepository.DbContext.BeginTransaction();
                CategoryRepository.EnsurePersistent(category);
                CategoryRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(category);
                var results = category.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("{0}: The {0} field is required.", "Name"));
                Assert.IsTrue(category.IsTransient());
                Assert.IsFalse(category.IsValid());
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
            Category category = null;
            try
            {
                #region Arrange
                category = GetValid(9);
                category.Name = " ";
                #endregion Arrange

                #region Act
                CategoryRepository.DbContext.BeginTransaction();
                CategoryRepository.EnsurePersistent(category);
                CategoryRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(category);
                var results = category.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("{0}: The {0} field is required.", "Name"));
                Assert.IsTrue(category.IsTransient());
                Assert.IsFalse(category.IsValid());
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
            Category category = null;
            try
            {
                #region Arrange
                category = GetValid(9);
                category.Name = "x".RepeatTimes((100 + 1));
                #endregion Arrange

                #region Act
                CategoryRepository.DbContext.BeginTransaction();
                CategoryRepository.EnsurePersistent(category);
                CategoryRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(category);
                Assert.AreEqual(100 + 1, category.Name.Length);
                var results = category.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("{0}: The field {0} must be a string with a maximum length of {1}.", "Name", "100"));
                Assert.IsTrue(category.IsTransient());
                Assert.IsFalse(category.IsValid());
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
            var category = GetValid(9);
            category.Name = "x";
            #endregion Arrange

            #region Act
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(category);
            CategoryRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(category.IsTransient());
            Assert.IsTrue(category.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var category = GetValid(9);
            category.Name = "x".RepeatTimes(100);
            #endregion Arrange

            #region Act
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(category);
            CategoryRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(100, category.Name.Length);
            Assert.IsFalse(category.IsTransient());
            Assert.IsTrue(category.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests

        #region Rank Tests

        /// <summary>
        /// Tests the Rank with max int value saves.
        /// </summary>
        [TestMethod]
        public void TestRankWithMaxIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Rank = int.MaxValue;
            #endregion Arrange

            #region Act
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(record);
            CategoryRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MaxValue, record.Rank);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Rank with min int value saves.
        /// </summary>
        [TestMethod]
        public void TestRankWithMinIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Rank = int.MinValue;
            #endregion Arrange

            #region Act
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(record);
            CategoryRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MinValue, record.Rank);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        #endregion Rank Tests
        
        #region Affirmation Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Affirmation with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAffirmationWithNullValueDoesNotSave()
        {
            Category category = null;
            try
            {
                #region Arrange
                category = GetValid(9);
                category.Affirmation = null;
                #endregion Arrange

                #region Act
                CategoryRepository.DbContext.BeginTransaction();
                CategoryRepository.EnsurePersistent(category);
                CategoryRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(category);
                var results = category.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("{0}: The {0} field is required.", "Affirmation"));
                Assert.IsTrue(category.IsTransient());
                Assert.IsFalse(category.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Affirmation with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAffirmationWithEmptyStringDoesNotSave()
        {
            Category category = null;
            try
            {
                #region Arrange
                category = GetValid(9);
                category.Affirmation = string.Empty;
                #endregion Arrange

                #region Act
                CategoryRepository.DbContext.BeginTransaction();
                CategoryRepository.EnsurePersistent(category);
                CategoryRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(category);
                var results = category.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("{0}: The {0} field is required.", "Affirmation"));
                Assert.IsTrue(category.IsTransient());
                Assert.IsFalse(category.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Affirmation with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAffirmationWithSpacesOnlyDoesNotSave()
        {
            Category category = null;
            try
            {
                #region Arrange
                category = GetValid(9);
                category.Affirmation = " ";
                #endregion Arrange

                #region Act
                CategoryRepository.DbContext.BeginTransaction();
                CategoryRepository.EnsurePersistent(category);
                CategoryRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(category);
                var results = category.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("{0}: The {0} field is required.", "Affirmation"));
                Assert.IsTrue(category.IsTransient());
                Assert.IsFalse(category.IsValid());
                throw;
            }
        }


        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Affirmation with one character saves.
        /// </summary>
        [TestMethod]
        public void TestAffirmationWithOneCharacterSaves()
        {
            #region Arrange
            var category = GetValid(9);
            category.Affirmation = "x";
            #endregion Arrange

            #region Act
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(category);
            CategoryRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(category.IsTransient());
            Assert.IsTrue(category.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Affirmation with long value saves.
        /// </summary>
        [TestMethod]
        public void TestAffirmationWithLongValueSaves()
        {
            #region Arrange
            var category = GetValid(9);
            category.Affirmation = "x".RepeatTimes(1000);
            #endregion Arrange

            #region Act
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(category);
            CategoryRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(1000, category.Affirmation.Length);
            Assert.IsFalse(category.IsTransient());
            Assert.IsTrue(category.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Affirmation Tests

        #region Encouragement Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Encouragement with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEncouragementWithNullValueDoesNotSave()
        {
            Category category = null;
            try
            {
                #region Arrange
                category = GetValid(9);
                category.Encouragement = null;
                #endregion Arrange

                #region Act
                CategoryRepository.DbContext.BeginTransaction();
                CategoryRepository.EnsurePersistent(category);
                CategoryRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(category);
                var results = category.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("{0}: The {0} field is required.", "Encouragement"));
                Assert.IsTrue(category.IsTransient());
                Assert.IsFalse(category.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Encouragement with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEncouragementWithEmptyStringDoesNotSave()
        {
            Category category = null;
            try
            {
                #region Arrange
                category = GetValid(9);
                category.Encouragement = string.Empty;
                #endregion Arrange

                #region Act
                CategoryRepository.DbContext.BeginTransaction();
                CategoryRepository.EnsurePersistent(category);
                CategoryRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(category);
                var results = category.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("{0}: The {0} field is required.", "Encouragement"));
                Assert.IsTrue(category.IsTransient());
                Assert.IsFalse(category.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Encouragement with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEncouragementWithSpacesOnlyDoesNotSave()
        {
            Category category = null;
            try
            {
                #region Arrange
                category = GetValid(9);
                category.Encouragement = " ";
                #endregion Arrange

                #region Act
                CategoryRepository.DbContext.BeginTransaction();
                CategoryRepository.EnsurePersistent(category);
                CategoryRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(category);
                var results = category.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("{0}: The {0} field is required.", "Encouragement"));
                Assert.IsTrue(category.IsTransient());
                Assert.IsFalse(category.IsValid());
                throw;
            }
        }

        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Encouragement with one character saves.
        /// </summary>
        [TestMethod]
        public void TestEncouragementWithOneCharacterSaves()
        {
            #region Arrange
            var category = GetValid(9);
            category.Encouragement = "x";
            #endregion Arrange

            #region Act
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(category);
            CategoryRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(category.IsTransient());
            Assert.IsTrue(category.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Encouragement with long value saves.
        /// </summary>
        [TestMethod]
        public void TestEncouragementWithLongValueSaves()
        {
            #region Arrange
            var category = GetValid(9);
            category.Encouragement = "x".RepeatTimes(1000);
            #endregion Arrange

            #region Act
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(category);
            CategoryRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(1000, category.Encouragement.Length);
            Assert.IsFalse(category.IsTransient());
            Assert.IsTrue(category.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Encouragement Tests

        #region IsActive Tests

        /// <summary>
        /// Tests the IsActive is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsFalseSaves()
        {
            #region Arrange
            Category category = GetValid(9);
            category.IsActive = false;
            #endregion Arrange

            #region Act
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(category);
            CategoryRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(category.IsActive);
            Assert.IsFalse(category.IsTransient());
            Assert.IsTrue(category.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the IsActive is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsTrueSaves()
        {
            #region Arrange
            var category = GetValid(9);
            category.IsActive = true;
            #endregion Arrange

            #region Act
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(category);
            CategoryRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(category.IsActive);
            Assert.IsFalse(category.IsTransient());
            Assert.IsTrue(category.IsValid());
            #endregion Assert
        }

        #endregion IsActive Tests

        #region DoNotUseForCalculations Tests

        /// <summary>
        /// Tests the DoNotUseForCalculations is false saves.
        /// </summary>
        [TestMethod]
        public void TestDoNotUseForCalculationsIsFalseSaves()
        {
            #region Arrange
            Category category = GetValid(9);
            category.DoNotUseForCalculations = false;
            #endregion Arrange

            #region Act
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(category);
            CategoryRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(category.DoNotUseForCalculations);
            Assert.IsFalse(category.IsTransient());
            Assert.IsTrue(category.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the DoNotUseForCalculations is true saves.
        /// </summary>
        [TestMethod]
        public void TestDoNotUseForCalculationsIsTrueSaves()
        {
            #region Arrange
            var category = GetValid(9);
            category.DoNotUseForCalculations = true;
            #endregion Arrange

            #region Act
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(category);
            CategoryRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(category.DoNotUseForCalculations);
            Assert.IsFalse(category.IsTransient());
            Assert.IsTrue(category.IsValid());
            #endregion Assert
        }

        #endregion DoNotUseForCalculations Tests

        #region IsCurrentVersion Tests

        /// <summary>
        /// Tests the IsCurrentVersion is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsCurrentVersionIsFalseSaves()
        {
            #region Arrange
            Category category = GetValid(9);
            category.IsCurrentVersion = false;
            #endregion Arrange

            #region Act
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(category);
            CategoryRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(category.IsCurrentVersion);
            Assert.IsFalse(category.IsTransient());
            Assert.IsTrue(category.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the IsCurrentVersion is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsCurrentVersionIsTrueSaves()
        {
            #region Arrange
            var category = GetValid(9);
            category.IsCurrentVersion = true;
            #endregion Arrange

            #region Act
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(category);
            CategoryRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(category.IsCurrentVersion);
            Assert.IsFalse(category.IsTransient());
            Assert.IsTrue(category.IsValid());
            #endregion Assert
        }

        #endregion IsCurrentVersion Tests
        

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
            expectedFields.Add(new NameAndType("Affirmation", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)1000)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("DoNotUseForCalculations", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Encouragement", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)1000)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("IsActive", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("IsCurrentVersion", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)100)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Rank", "System.Int32", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Category));

        }

        #endregion Reflection of Database.	
		
		
    }
}