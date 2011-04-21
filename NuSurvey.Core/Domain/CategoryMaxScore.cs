using System;
using System.Collections.Generic;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace NuSurvey.Core.Domain
{
    public class CategoryMaxScore : DomainObject
    {
        public virtual DateTime Date { get; set; }
        public virtual int MaxScore { get; set; }
        [Required]
        public virtual Category Category { get; set; }
    }

    public class CategoryMaxScoreMap : ClassMap<CategoryMaxScore>
    {
        public CategoryMaxScoreMap()
        {
            Id(x => x.Id);
            Map(x => x.Date);
            Map(x => x.MaxScore);

            References(x => x.Category);
        }
    }
}
