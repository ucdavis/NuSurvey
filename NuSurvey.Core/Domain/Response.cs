using System;
using System.Collections.Generic;
using System.ComponentModel;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;
using System.ComponentModel.DataAnnotations;

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

