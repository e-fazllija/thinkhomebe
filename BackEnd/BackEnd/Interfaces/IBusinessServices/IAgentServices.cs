using BackEnd.Entities;
using BackEnd.Models.AgentModels;
using BackEnd.Models.OutputModels;

namespace BackEnd.Interfaces.IBusinessServices
{
    public interface IAgentServices
    {
        Task<AgentSelectModel> Create(AgentCreateModel dto);
        Task<ListViewModel<AgentSelectModel>> Get(int currentPage, string? filterRequest, char? fromName, char? toName);
        Task<AgentSelectModel> Update(AgentUpdateModel dto);
        Task<AgentSelectModel> GetById(int id);
        Task<Agent> Delete(int id);
    }
}
