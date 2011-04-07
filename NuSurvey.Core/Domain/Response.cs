using FluentNHibernate.Mapping;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace NuSurvey.Core.Domain
{
    public class Response : DomainObject
    {
        [Required]
        public virtual string Value { get; set; }

        public virtual int Score { get; set; }
        public virtual int Order { get; set; }
        public virtual bool IsActive { get; set; }
        [NotNull]
        public virtual Question Question { get; set; }
    }

    public class ResponseMap : ClassMap<Response>
    {
        public ResponseMap()
        {
            Map(x => x.Id);
            Map(x => x.Value);
            Map(x => x.Score);
            Map(x => x.Order);
            Map(x => x.IsActive);

            References(x => x.Question);
        }
    }
}

