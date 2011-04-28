using System;
using System.Collections.Generic;
using System.ComponentModel;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;
using System.ComponentModel.DataAnnotations;
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
        [DisplayName("Short Name")]
        public virtual string ShortName { get; set; }

        [DisplayName("Active")]
        public virtual bool IsActive { get; set; }

        [Required]
        public virtual IList<Question> Questions { get; set; }
        [Required]
        public virtual IList<SurveyResponse> SurveyResponses { get; set; }
        [Required]
        public virtual IList<Category> Categories { get; set; }
    }

    public class SurveyMap : ClassMap<Survey>
    {
        public SurveyMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.ShortName);
            Map(x => x.IsActive);

            HasMany(x => x.Questions);
            HasMany(x => x.SurveyResponses);
            HasMany(x => x.Categories);
        }
    }
}
