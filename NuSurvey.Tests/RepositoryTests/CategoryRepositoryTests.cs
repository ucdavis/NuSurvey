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
            RepositoryLoad.LoadSurveys(Repository, 3); 
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
        
        #region LastUpdate Tests

        /// <summary>
        /// Tests the LastUpdate with past date will save.
        /// </summary>
        [TestMethod]
        public void TestLastUpdateWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(-10);
            Category record = GetValid(99);
            record.LastUpdate = compareDate;
            #endregion Arrange

            #region Act
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(record);
            CategoryRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.LastUpdate);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the LastUpdate with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestLastUpdateWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now;
            var record = GetValid(99);
            record.LastUpdate = compareDate;
            #endregion Arrange

            #region Act
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(record);
            CategoryRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.LastUpdate);
            #endregion Assert
        }

        /// <summary>
        /// Tests the LastUpdate with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestLastUpdateWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(15);
            var record = GetValid(99);
            record.LastUpdate = compareDate;
            #endregion Arrange

            #region Act
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(record);
            CategoryRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.LastUpdate);
            #endregion Assert
        }
        #endregion LastUpdate Tests
        
        #region CreateDate Tests

        /// <summary>
        /// Tests the CreateDate with past date will save.
        /// </summary>
        [TestMethod]
        public void TestCreateDateWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(-10);
            Category record = GetValid(99);
            record.CreateDate = compareDate;
            #endregion Arrange

            #region Act
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(record);
            CategoryRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.CreateDate);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the CreateDate with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestCreateDateWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now;
            var record = GetValid(99);
            record.CreateDate = compareDate;
            #endregion Arrange

            #region Act
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(record);
            CategoryRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.CreateDate);
            #endregion Assert
        }

        /// <summary>
        /// Tests the CreateDate with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestCreateDateWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(15);
            var record = GetValid(99);
            record.CreateDate = compareDate;
            #endregion Arrange

            #region Act
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(record);
            CategoryRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.CreateDate);
            #endregion Assert
        }
        #endregion CreateDate Tests
        
        #region Survey Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the Survey with A value of null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestSurveyWithAValueOfNullDoesNotSave()
        {
            Category category = null;
            try
            {
                #region Arrange
                category = GetValid(9);
                category.Survey = null;
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
                Assert.AreEqual(category.Survey, null);
                var results = category.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Survey: The Survey field is required.");
                Assert.IsTrue(category.IsTransient());
                Assert.IsFalse(category.IsValid());
                throw;
            }	
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestSurveyWithANewValueDoesNotSave()
        {
            Category category = null;
            try
            {
                #region Arrange
                category = GetValid(9);
                category.Survey = CreateValidEntities.Survey(9);
                #endregion Arrange

                #region Act
                CategoryRepository.DbContext.BeginTransaction();
                CategoryRepository.EnsurePersistent(category);
                CategoryRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(category);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: NuSurvey.Core.Domain.Survey, Entity: NuSurvey.Core.Domain.Survey", ex.Message);
 
                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests

        [TestMethod]
        public void TestCategoryWithExistingSurveySaves()
        {
            #region Arrange
            var survey = Repository.OfType<Survey>().GetNullableById(2);
            Assert.IsNotNull(survey);
            var record = GetValid(9);
            record.Survey = survey;
            #endregion Arrange

            #region Act
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(record);
            CategoryRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(survey.Name, record.Survey.Name);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }
        #endregion Valid Tests
        #region Cascade Tests

        [TestMethod]
        public void TestDeleteCategoryDoesNotCascadeToSurvey()
        {
            #region Arrange
            var survey = Repository.OfType<Survey>().GetNullableById(2);
            Assert.IsNotNull(survey);
            var record = GetValid(9);
            record.Survey = survey;

            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(record);
            CategoryRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            NHibernateSessionManager.Instance.GetSession().Evict(survey);
            record = CategoryRepository.GetNullableById(saveId);
            Assert.IsNotNull(record);
            Assert.AreEqual("Name2", record.Survey.Name);
            #endregion Arrange

            #region Act
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.Remove(record);
            CategoryRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNull(CategoryRepository.GetNullableById(saveId));
            Assert.IsNotNull(Repository.OfType<Survey>().GetNullableById(2));
            #endregion Assert		
        }
        #endregion Cascade Tests
        #endregion Survey Tests

        #region Survey Tests
        #region Invalid Tests

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestPreviousVersionWithANewValueDoesNotSave()
        {
            Category category = null;
            try
            {
                #region Arrange
                category = GetValid(9);
                category.PreviousVersion = CreateValidEntities.Category(9);
                #endregion Arrange

                #region Act
                CategoryRepository.DbContext.BeginTransaction();
                CategoryRepository.EnsurePersistent(category);
                CategoryRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(category);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: NuSurvey.Core.Domain.Category, Entity: NuSurvey.Core.Domain.Category", ex.Message);

                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests

        [TestMethod]
        public void TestPreviousVersionWithNullValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.PreviousVersion = null;
            #endregion Arrange

            #region Act
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(record);
            CategoryRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert

            Assert.IsNull(record.PreviousVersion);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestCategoryWithExistingPreviousVersionSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.PreviousVersion = CategoryRepository.GetNullableById(1);
            #endregion Arrange

            #region Act
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(record);
            CategoryRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #endregion PreviousVersion Tests

        #region CategoryGoals Tests
        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCategoryGoalsWithAValueOfNullDoesNotSave()
        {
            Category record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.CategoryGoals = null;
                #endregion Arrange

                #region Act
                CategoryRepository.DbContext.BeginTransaction();
                CategoryRepository.EnsurePersistent(record);
                CategoryRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.CategoryGoals, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("CategoryGoals: The CategoryGoals field is required.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        #endregion Invalid Tests
        #region Valid Tests
        [TestMethod]
        public void TestCategoryGoalsWithPopulatedListWillSave()
        {
            #region Arrange
            Category record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddCategoryGoal(CreateValidEntities.CategoryGoal(i + 1));
            }
            #endregion Arrange

            #region Act
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(record);
            CategoryRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.CategoryGoals);
            Assert.AreEqual(addedCount, record.CategoryGoals.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestCategoryGoalsWithPopulatedExistingListWillSave()
        {
            #region Arrange
            Category record = GetValid(9);
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(record);
            CategoryRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<CategoryGoal>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.CategoryGoal(i + 1));
                relatedRecords[i].Category = record;
                Repository.OfType<CategoryGoal>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.CategoryGoals.Add(relatedRecord);
            }
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(record);
            CategoryRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.CategoryGoals);
            Assert.AreEqual(addedCount, record.CategoryGoals.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestCategoryGoalsWithEmptyListWillSave()
        {
            #region Arrange
            Category record = GetValid(9);
            #endregion Arrange

            #region Act
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(record);
            CategoryRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.CategoryGoals);
            Assert.AreEqual(0, record.CategoryGoals.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests


        [TestMethod]
        public void TestCategoryCascadesSaveToCategoryGoal()
        {
            #region Arrange
            var count = Repository.OfType<CategoryGoal>().Queryable.Count();
            Category record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddCategoryGoal(CreateValidEntities.CategoryGoal(i+1));
            }

            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(record);
            CategoryRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = CategoryRepository.GetNullableById(saveId);
            #endregion Act

            #region Assert
            Assert.IsNotNull(record);
            Assert.AreEqual(addedCount, record.CategoryGoals.Count);
            Assert.AreEqual(count + addedCount, Repository.OfType<CategoryGoal>().Queryable.Count());
            #endregion Assert
        }


        [TestMethod]
        public void TestCategoryCascadesUpdateToCategoryGoal1()
        {
            #region Arrange
            var count = Repository.OfType<CategoryGoal>().Queryable.Count();
            Category record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddCategoryGoal(CreateValidEntities.CategoryGoal(i+1));
            }

            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(record);
            CategoryRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.CategoryGoals[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = CategoryRepository.GetNullableById(saveId);
            record.CategoryGoals[1].Name = "Updated";
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(record);
            CategoryRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<CategoryGoal>().Queryable.Count());
            var relatedRecord = Repository.OfType<CategoryGoal>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord);
            Assert.AreEqual("Updated", relatedRecord.Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestCategoryCascadesUpdateToCategoryGoal2()
        {
            #region Arrange
            var count = Repository.OfType<CategoryGoal>().Queryable.Count();
            Category record = GetValid(9);
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(record);
            CategoryRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<CategoryGoal>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.CategoryGoal(i + 1));
                relatedRecords[i].Category = record;
                Repository.OfType<CategoryGoal>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.CategoryGoals.Add(relatedRecord);
            }
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(record);
            CategoryRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.CategoryGoals[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Arrange

            #region Act
            record = CategoryRepository.GetNullableById(saveId);
            record.CategoryGoals[1].Name = "Updated";
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(record);
            CategoryRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<CategoryGoal>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<CategoryGoal>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            Assert.AreEqual("Updated", relatedRecord2.Name);
            #endregion Assert
        }

        /// <summary>
        /// Does NOT Remove it
        /// </summary>
        [TestMethod]
        public void TestCategoryDoesNotCascadesUpdateRemoveCategoryGoal()
        {
            #region Arrange
            var count = Repository.OfType<CategoryGoal>().Queryable.Count();
            Category record = GetValid(9);
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(record);
            CategoryRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<CategoryGoal>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.CategoryGoal(i + 1));
                relatedRecords[i].Category = record;
                Repository.OfType<CategoryGoal>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.CategoryGoals.Add(relatedRecord);
            }
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(record);
            CategoryRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.CategoryGoals[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = CategoryRepository.GetNullableById(saveId);
            record.CategoryGoals.RemoveAt(1);
            CategoryRepository.DbContext.BeginTransaction();
            CategoryRepository.EnsurePersistent(record);
            CategoryRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount), Repository.OfType<CategoryGoal>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<CategoryGoal>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            #endregion Assert
        }


        #endregion Cascade Tests


        #endregion CategoryGoals Tests



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
            expectedFields.Add(new NameAndType("CategoryGoals", "System.Collections.Generic.IList`1[NuSurvey.Core.Domain.CategoryGoal]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("CreateDate", "System.DateTime", new List<string>()));
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
            expectedFields.Add(new NameAndType("LastUpdate", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)100)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("PreviousVesion", "System.String", new List<string>
            {
                 ""
            }));
            expectedFields.Add(new NameAndType("Rank", "System.Int32", new List<string>()));
            expectedFields.Add(new NameAndType("Survey", "System.String", new List<string>
            {
                 ""
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Category));

        }

        #endregion Reflection of Database.	
		
		
    }
}