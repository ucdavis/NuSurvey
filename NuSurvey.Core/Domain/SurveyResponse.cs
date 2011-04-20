using System;
using System.Collections.Generic;
using FluentNHibernate.Mapping;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace NuSurvey.Core.Domain
{
    public class SurveyResponse : DomainObject
    {
        #region Constructor
        public SurveyResponse()
        {
            SetDefaults();
        }

        private void SetDefaults()
        {
            Answers = new List<Answer>();
            DateTaken = DateTime.Now;
        }
        #endregion Constructor

        [Required]
        [Length(10)]
        public virtual string StudentId { get; set; }
        public virtual DateTime DateTaken { get; set; }

        public virtual Category PositiveCategory { get; set; }
        public virtual Category NegativeCategoryId1 { get; set; }
        public virtual Category NegativeCategoryId2 { get; set; }
        [NotNull]
        public virtual Survey Survey { get; set; }

        [NotNull]
        public virtual IList<Answer> Answers { get; set; }
    }

    public class SurveyResponseMap : ClassMap<SurveyResponse>
    {
        public SurveyResponseMap()
        {
            Id(x => x.Id);
            Map(x => x.StudentId);
            Map(x => x.DateTaken);

            //TODO: Ensure that these point to the correct values in the database.
            References(x => x.PositiveCategory);
            References(x => x.NegativeCategoryId1);
            References(x => x.NegativeCategoryId2);
            References(x => x.Survey);
        }
    }
}
