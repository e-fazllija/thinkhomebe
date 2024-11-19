using BackEnd.Data;
using BackEnd.Entities;
using BackEnd.Interfaces.IRepositories;


namespace BackEnd.Services.Repositories
{
    public class AgentRepository : GenericRepository<Agent>, IAgentRepository
    {
        public AgentRepository(AppDbContext context) : base(context){}
    }
}
