using System;
using FluentNHibernate.Mapping;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;

namespace NuSurvey.Core.Domain
{
    public class CategoryMaxScore : DomainObject
    {
        public virtual DateTime Date { get; set; }
        public virtual int MaxScore { get; set; }
        [NotNull]
        public virtual Category Category { get; set; }
    }

    public class CategoryMaxScoreMap : ClassMap<CategoryMaxScore>
    {
        public CategoryMaxScoreMap()
        {
            Map(x => x.Id);
            Map(x => x.Date);
            Map(x => x.MaxScore);

            References(x => x.Category);
        }
    }
}
