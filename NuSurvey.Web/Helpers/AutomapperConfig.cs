using AutoMapper;
using NuSurvey.Core.Domain;

namespace NuSurvey.Web.Helpers
{
    public class AutomapperConfig
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg => cfg.AddProfile<ViewModelProfile>());
        }
    }

    public class ViewModelProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Category, Category>()
                .ForMember(x => x.Rank, x => x.Ignore())
                .ForMember(x => x.LastUpdate, x => x.Ignore())
                .ForMember(x => x.CreateDate, x => x.Ignore())
                .ForMember(x => x.Survey, x => x.Ignore());

            CreateMap<CategoryGoal, CategoryGoal>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Category, x => x.Ignore());

            CreateMap<Question, Question>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Order, x => x.Ignore())
                .ForMember(x => x.Responses, x=> x.Ignore())
                .ForMember(x => x.Photos, x=> x.Ignore())
                .ForMember(x => x.PrimaryPhoto, x=> x.Ignore())
                .ForMember(x => x.Survey, x => x.Ignore());                 

            CreateMap<Survey, Survey>()
                .ForMember(x => x.Questions, x => x.Ignore())
                .ForMember(x => x.SurveyResponses, x => x.Ignore())
                .ForMember(x => x.Categories, x => x.Ignore());

            CreateMap<Response, Response>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Question, x => x.Ignore());


            //CreateMap<Session, Session>()
            //    .ForMember(x => x.Id, x => x.Ignore())
            //    .ForMember(x => x.Seminar, x => x.Ignore());

            //CreateMap<Commodity, Commodity>()
            //    .ForMember(x => x.Id, x => x.Ignore());

            //CreateMap<PersonEditModel, Person>()
            //    .ForMember(x => x.Id, x => x.Ignore())
            //    .ForMember(x => x.FirstName, x => x.MapFrom(a => a.Person.FirstName))
            //    .ForMember(x => x.MI, x => x.MapFrom(a => a.Person.MI))
            //    .ForMember(x => x.LastName, x => x.MapFrom(a => a.Person.LastName))
            //    .ForMember(x => x.Salutation, x => x.MapFrom(a => a.Person.Salutation))
            //    .ForMember(x => x.BadgeName, x => x.MapFrom(a => a.Person.BadgeName))
            //    .ForMember(x => x.Phone, x => x.MapFrom(a => a.Person.Phone))
            //    .ForMember(x => x.CellPhone, x => x.MapFrom(a => a.Person.CellPhone))
            //    .ForMember(x => x.Fax, x => x.MapFrom(a => a.Person.Fax))
            //    .ForMember(x => x.Addresses, x => x.Ignore())
            //    .ForMember(x => x.Contacts, x => x.Ignore());

            //CreateMap<Address, Address>()
            //    .ForMember(x => x.Id, x => x.Ignore())
            //    .ForMember(x => x.AddressType, x => x.Ignore())
            //    .ForMember(x => x.Person, x => x.Ignore());

            //CreateMap<Contact, Contact>()
            //    .ForMember(x => x.Id, x => x.Ignore())
            //    .ForMember(x => x.ContactType, x => x.Ignore())
            //    .ForMember(x => x.Person, x => x.Ignore());

            //CreateMap<Firm, Firm>()
            //    .ForMember(x => x.Id, x => x.Ignore())
            //    .ForMember(x => x.Review, x => x.Ignore());
        }

    }
}