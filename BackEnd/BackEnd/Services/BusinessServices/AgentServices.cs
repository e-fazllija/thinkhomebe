using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using BackEnd.Entities;
using BackEnd.Interfaces;
using BackEnd.Interfaces.IBusinessServices;
using BackEnd.Models.AgentModels;
using BackEnd.Models.Options;
using BackEnd.Models.OutputModels;

namespace BackEnd.Services.BusinessServices
{
    public class AgentServices : IAgentServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AgentServices> _logger;
        private readonly IOptionsMonitor<PaginationOptions> options;
        public AgentServices(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AgentServices> logger, IOptionsMonitor<PaginationOptions> options)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            this.options = options;

        }
        public async Task<AgentSelectModel> Create(AgentCreateModel dto)
        {
            try
            {
                var entityClass = _mapper.Map<Agent>(dto);
                await _unitOfWork.AgentRepository.InsertAsync(entityClass);
                _unitOfWork.Save();

                AgentSelectModel response = new AgentSelectModel();
                _mapper.Map(entityClass, response);

                _logger.LogInformation(nameof(Create));
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Si è verificato un errore in fase creazione");
            }
        }

        public async Task<Agent> Delete(int id)
        {
            try
            {
                IQueryable<Agent> query = _unitOfWork.dbContext.Agents;

                if (id == 0)
                    throw new NullReferenceException("L'id non può essere 0");

                query = query.Where(x => x.Id == id);

                Agent EntityClasses = await query.FirstOrDefaultAsync();

                if (EntityClasses == null)
                    throw new NullReferenceException("Record non trovato!");

                _unitOfWork.AgentRepository.Delete(EntityClasses);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation(nameof(Delete));

                return EntityClasses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                if (ex.InnerException.Message.Contains("DELETE statement conflicted with the REFERENCE constraint"))
                {
                    throw new Exception("Impossibile eliminare il record perché è utilizzato come chiave esterna in un'altra tabella.");
                }
                if (ex is NullReferenceException)
                {
                    throw new Exception(ex.Message);
                }
                else
                {
                    throw new Exception("Si è verificato un errore in fase di eliminazione");
                }
            }
        }

        public async Task<ListViewModel<AgentSelectModel>> Get(int currentPage, string? filterRequest, char? fromName, char? toName)
        {
            try
            {
                IQueryable<Agent> query = _unitOfWork.dbContext.Agents;

                if (!string.IsNullOrEmpty(filterRequest))
                    query = query.Where(x => x.Name.Contains(filterRequest));

                if (fromName != null)
                {
                    string fromNameString = fromName.ToString();
                    query = query.Where(x => string.Compare(x.Name.Substring(0, 1), fromNameString) >= 0);
                }

                if (toName != null)
                {
                    string toNameString = toName.ToString();
                    query = query.Where(x => string.Compare(x.Name.Substring(0, 1), toNameString) <= 0);
                }

                ListViewModel<AgentSelectModel> result = new ListViewModel<AgentSelectModel>();

                result.Total = await query.CountAsync();

                if (currentPage > 0)
                {
                    query = query
                    .Skip((currentPage * options.CurrentValue.AgentItemPerPage) - options.CurrentValue.AgentItemPerPage)
                            .Take(options.CurrentValue.AgentItemPerPage);
                }

                List<Agent> queryList = await query
                    //.Include(x => x.AgentType)
                    .ToListAsync();

                result.Data = _mapper.Map<List<AgentSelectModel>>(queryList);

                _logger.LogInformation(nameof(Get));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Si è verificato un errore");
            }
        }

        public async Task<AgentSelectModel> GetById(int id)
        {
            try
            {
                if (id is not > 0)
                    throw new Exception("Si è verificato un errore!");

                var query = await _unitOfWork.dbContext.Agents
                    //.Include(x => x.AgentType)
                    .FirstOrDefaultAsync(x => x.Id == id);

                AgentSelectModel result = _mapper.Map<AgentSelectModel>(query);

                _logger.LogInformation(nameof(GetById));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Si è verificato un errore");
            }
        }

        public async Task<AgentSelectModel> Update(AgentUpdateModel dto)
        {
            try
            {
                var EntityClass =
                    await _unitOfWork.AgentRepository.FirstOrDefaultAsync(q => q.Where(x => x.Id == dto.Id));

                if (EntityClass == null)
                    throw new NullReferenceException("Record non trovato!");

                EntityClass = _mapper.Map(dto, EntityClass);

                _unitOfWork.AgentRepository.Update(EntityClass);
                await _unitOfWork.SaveAsync();

                AgentSelectModel response = new AgentSelectModel();
                _mapper.Map(EntityClass, response);

                _logger.LogInformation(nameof(Update));

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                if (ex is NullReferenceException)
                {
                    throw new Exception(ex.Message);
                }
                else
                {
                    throw new Exception("Si è verificato un errore in fase di modifica");
                }
            }
        }
    }
}
