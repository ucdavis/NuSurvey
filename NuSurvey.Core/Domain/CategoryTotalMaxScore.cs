using System;
using System.Collections.Generic;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;
using System.ComponentModel.DataAnnotations;

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
