using AutoMapper;
using BackEnd.Entities;
using BackEnd.Models.SpecificDocumentationModels;

namespace BackEnd.Profiles
{
    public class SpecificDocumentationProfile : Profile
    {
        public SpecificDocumentationProfile()
        {
            CreateMap<SpecificDocumentation, SpecificDocumentationSelectModel>();
            CreateMap<SpecificDocumentationSelectModel, SpecificDocumentation>();
            CreateMap<SpecificDocumentationCreateModel, SpecificDocumentation>();
        }
    }
}
