﻿using System;
using System.Collections.Generic;
using FluentNHibernate.Mapping;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

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
            Questions = new List<Question>();
            SurveyResponses = new List<SurveyResponse>();
            Categories = new List<Category>();
        }

        #endregion Constructor

        [Required]
        [Length(100)]
        public virtual string Name { get; set; }
        public virtual bool IsActive { get; set; }
        
        [NotNull]
        public virtual IList<Question> Questions { get; set; }
        [NotNull]
        public virtual IList<SurveyResponse> SurveyResponses { get; set; }
        [NotNull]
        public virtual IList<Category> Categories { get; set; }
    }

    public class SurveyMap : ClassMap<Survey>
    {
        public SurveyMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.IsActive);
        }
    }
}
