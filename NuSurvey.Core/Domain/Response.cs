using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace NuSurvey.Core.Domain
{
    public class Response : DomainObject
    {
        [Required]
        [DisplayName("Choice")]
        public virtual string Value { get; set; }

        public virtual int Score { get; set; }
        public virtual int Order { get; set; }
        public virtual bool IsActive { get; set; }
        [Required]
        public virtual Question Question { get; set; }
    }

    public class ResponseMap : ClassMap<Response>
    {
        public ResponseMap()
        {
            Id(x => x.Id);
            Map(x => x.Value);
            Map(x => x.Score);
            Map(x => x.Order).Column("`Order`");
            Map(x => x.IsActive);

            References(x => x.Question);
        }
    }
}

