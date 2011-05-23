using System;
using System.Collections.Generic;
using System.ComponentModel;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;

namespace NuSurvey.Core.Domain
{
    public class Category : DomainObject
    {
        #region Constructor
        public Category(Survey survey)
        {
            SetPreDefaults();
            Survey = survey;
            SetPostDefaults();
        }

        public Category()
        {
            SetPreDefaults();
            SetPostDefaults();
        }

        private void SetPreDefaults()
        {
            IsCurrentVersion = true;
            CategoryGoals = new List<CategoryGoal>();
            Questions = new List<Question>();

        }
        private void SetPostDefaults()
        {
            if (Rank == 0)
            {
                if (Survey != null && Survey.Categories != null && Survey.Categories.Count > 0)
                {
                    Rank = Survey.Categories.Max(a => a.Rank) + 1;
                }
                else
                {
                    Rank = 1;
                }
            }
        }
        #endregion Constructor


        [Required]
        [StringLength(100)]
        public virtual string Name { get; set; }

        public virtual int Rank { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public virtual string Affirmation { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public virtual string Encouragement { get; set; }

        [DisplayName("Active")]
        public virtual bool IsActive { get; set; }

        [DisplayName("Do Not Use For Calculations")]
        public virtual bool DoNotUseForCalculations { get; set; }

        public virtual bool IsCurrentVersion { get; set; }

        /// <summary>
        /// This should be updated for every save
        /// </summary>
        [DisplayName("Last Updated On")] 
        public virtual DateTime LastUpdate { get; set; }

        public virtual DateTime CreateDate { get; set; }

        [Required]
        public virtual Survey Survey { get; set; }

        public virtual Category PreviousVersion { get; set; }

        [Required]
        public virtual IList<CategoryGoal> CategoryGoals { get; set; }
        [Required]
        public virtual IList<Question> Questions { get; set; }

        #region Methods
        public virtual void AddCategoryGoal(CategoryGoal categoryGoal)
        {
            categoryGoal.Category = this;
            CategoryGoals.Add(categoryGoal);
        }

        public virtual void AddQuestions(Question question)
        {
            question.Category = this;
            question.Survey = Survey;
            Questions.Add(question);
        }

        #endregion Methods

    }

    public class CategoryMap : ClassMap<Category>
    {
        public CategoryMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.Rank);
            Map(x => x.Affirmation);
            Map(x => x.Encouragement);
            Map(x => x.IsActive);
            Map(x => x.LastUpdate);
            Map(x => x.CreateDate);
            Map(x => x.DoNotUseForCalculations);
            Map(x => x.IsCurrentVersion);
            References(x => x.PreviousVersion).Column("PreviousVersion").ForeignKey("id");

            HasMany(x => x.CategoryGoals).Cascade.SaveUpdate();
            HasMany(x => x.Questions).Cascade.SaveUpdate();
            References(x => x.Survey);
        }
    }
}
