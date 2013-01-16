using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;
namespace NuSurvey.Core.Domain
{
    public class Survey : DomainObject
    {
        #region Constructor
        public Survey()
        {
            SetDefaults();
        }

        private void SetDefaults()
        {
            IsActive = false;
            Questions = new List<Question>();
            SurveyResponses = new List<SurveyResponse>();
            Categories = new List<Category>();
        }

        #endregion Constructor

        [Required]
        [StringLength(100)]
        public virtual string Name { get; set; }

        [StringLength(10)]
        [Display(Name = "Short Name")]
        public virtual string ShortName { get; set; }

        [Display(Name = "Active")]
        public virtual bool IsActive { get; set; }

        [Required]
        public virtual IList<Question> Questions { get; set; }
        [Required]
        public virtual IList<SurveyResponse> SurveyResponses { get; set; }
        [Required]
        public virtual IList<Category> Categories { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Quiz Type")]
        public virtual string QuizType { get; set; }

        [StringLength(250)]
        public virtual string OwnerId { get; set; } //When a survey is duplicated, this shows who it belongs to.

        public virtual Photo Photo { get; set; }
    }

    public class SurveyMap : ClassMap<Survey>
    {
        public SurveyMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.ShortName);
            Map(x => x.IsActive);
            Map(x => x.QuizType);
            Map(x => x.OwnerId);

            References(x => x.Photo);

            HasMany(x => x.Questions);
            HasMany(x => x.SurveyResponses);
            HasMany(x => x.Categories);
        }
    }
}
