using System;
using System.Collections.Generic;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace NuSurvey.Core.Domain
{
    /// <summary>
    /// The CategoryMax Score is used to save the maximum score possible by adding up all the question's highest response score.
    /// This value is used to calculate the percentage of the survey response's score
    /// If anything is changed for a Category's questions or responses, this needs to be recalculated.
    /// </summary>
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
