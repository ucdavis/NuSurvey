using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace NuSurvey.Core.Domain
{
    public class CategoryTotalMaxScore : DomainObject
    {
        public virtual int TotalMaxScore { get; set; }
    }

    public class CategoryTotalMaxScoreMap : ClassMap<CategoryTotalMaxScore>
    {
        public CategoryTotalMaxScoreMap()
        {
            Table("vw_CategoryTotalMaxScores");
            ReadOnly();
            
            Id(x => x.Id);
            Map(x => x.TotalMaxScore);
            
        }
    }
}
