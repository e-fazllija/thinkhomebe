using BackEnd.Entities;
using BackEnd.Models.AgentModels;

namespace BackEnd.Profiles
{
    public class AgentProfile : AutoMapper.Profile
    {
        public AgentProfile()
        {
            CreateMap<Agent, AgentCreateModel>();
            CreateMap<Agent, AgentUpdateModel>();
            CreateMap<Agent, AgentSelectModel>();
            CreateMap<AgentSelectModel, AgentUpdateModel>();
            CreateMap<AgentUpdateModel, AgentSelectModel>();

            CreateMap<AgentCreateModel, Agent>();
            CreateMap<AgentUpdateModel, Agent>();
            CreateMap<AgentSelectModel, Agent>();

        }
    }
}
