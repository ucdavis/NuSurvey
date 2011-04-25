using System;
using System.Collections.Generic;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace NuSurvey.Core.Domain
{
    public class Answer : DomainObject
    {

        public virtual int Score { get; set; }

        public virtual int? OpenEndedAnswer { get; set; }

        [Required]
        public virtual SurveyResponse SurveyResponse { get; set; }
        [Required]
        public virtual Category Category { get; set; }
        [Required]
        public virtual Question Question { get; set; }
        /// <summary>
        /// This is only required for non-open ended questions that are for categories that are used for calculations
        /// </summary>
        public virtual Response Response { get; set; }
    }


    public class AnswerMap : ClassMap<Answer>
    {
        public AnswerMap()
        {
            Id(x => x.Id);
            Map(x => x.Score);
            Map(x => x.OpenEndedAnswer);

            References(x => x.SurveyResponse);
            References(x => x.Category);
            References(x => x.Question);
            References(x => x.Response);
        }
    }
}


