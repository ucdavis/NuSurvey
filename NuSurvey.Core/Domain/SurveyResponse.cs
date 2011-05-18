using System;
using System.Collections.Generic;
using System.ComponentModel;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;
using System.ComponentModel.DataAnnotations;


namespace NuSurvey.Core.Domain
{
    public class SurveyResponse : DomainObject
    {
        #region Constructor
        public SurveyResponse()
        {
            SetDefaults();
        }
        public SurveyResponse(Survey survey)
        {
            SetDefaults();
            Survey = survey;
        }

        private void SetDefaults()
        {
            Answers = new List<Answer>();
            DateTaken = DateTime.Now;
        }
        #endregion Constructor

        [Required]
        [StringLength(10)]
        [DisplayName("Id #")]
        public virtual string StudentId { get; set; }

        [DisplayName("Date Taken")]
        public virtual DateTime DateTaken { get; set; }

        public virtual Category PositiveCategory { get; set; }
        public virtual Category NegativeCategory1 { get; set; }
        public virtual Category NegativeCategory2 { get; set; }
        [Required]
        public virtual Survey Survey { get; set; }

        [Required]
        [DisplayName("User Id")]
        public virtual string UserId { get; set; }
        
        public virtual IList<Answer> Answers { get; set; }

        public virtual void AddAnswers(Answer answer)
        {
            answer.SurveyResponse = this;
            Answers.Add(answer);
        }
    }

    public class SurveyResponseMap : ClassMap<SurveyResponse>
    {
        public SurveyResponseMap()
        {
            Id(x => x.Id);
            Map(x => x.StudentId);
            Map(x => x.DateTaken);
            Map(x => x.UserId);

            References(x => x.PositiveCategory).Column("PositiveCategoryId");
            References(x => x.NegativeCategory1).Column("NegativeCategoryId1");
            References(x => x.NegativeCategory2).Column("NegativeCategoryId2");
            References(x => x.Survey);

            HasMany(x => x.Answers).Cascade.All();
        }
    }
}
