using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

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

        [Display(Name = "Bypass Score")]
        public virtual bool BypassScore { get; set; }
        [StringLength(50)]
        public virtual string OpenEndedStringAnswer { get; set; }
    }


    public class AnswerMap : ClassMap<Answer>
    {
        public AnswerMap()
        {
            Id(x => x.Id);
            Map(x => x.Score);
            Map(x => x.OpenEndedAnswer);
            Map(x => x.BypassScore);
            Map(x => x.OpenEndedStringAnswer);

            References(x => x.SurveyResponse);
            References(x => x.Category);
            References(x => x.Question);
            References(x => x.Response);
        }
    }
}


