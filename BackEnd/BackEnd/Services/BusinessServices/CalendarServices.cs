using AutoMapper;
using BackEnd.Entities;
using BackEnd.Interfaces;
using BackEnd.Interfaces.IBusinessServices;
using BackEnd.Models.CalendarModels;
using BackEnd.Models.CustomerModels;
using BackEnd.Models.OutputModels;
using BackEnd.Models.RealEstatePropertyModels;
using BackEnd.Models.RequestModels;
using BackEnd.Models.UserModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services.BusinessServices
{
    public class CalendarServices : ICalendarServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CalendarServices> _logger;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public CalendarServices(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CalendarServices> logger, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<CalendarSelectModel> Create(CalendarCreateModel dto)
        {
            try
            {
                var entityClass = _mapper.Map<Calendar>(dto);
                await _unitOfWork.CalendarRepository.InsertAsync(entityClass);
                _unitOfWork.Save();

                CalendarSelectModel response = new CalendarSelectModel();
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

        public async Task<Calendar> Delete(int id)
        {
            try
            {
                IQueryable<Calendar> query = _unitOfWork.dbContext.Calendars;

                if (id == 0)
                    throw new NullReferenceException("L'id non può essere 0");

                query = query.Where(x => x.Id == id);

                Calendar EntityClasses = await query.FirstOrDefaultAsync();

                if (EntityClasses == null)
                    throw new NullReferenceException("Record non trovato!");

                _unitOfWork.CalendarRepository.Delete(EntityClasses);
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

        public async Task<ListViewModel<CalendarSelectModel>> Get(string? filterRequest, char? fromName, char? toName)
        {
            try
            {
                IQueryable<Calendar> query = _unitOfWork.dbContext.Calendars.OrderByDescending(x => x.DataInizioEvento);

                if (!string.IsNullOrEmpty(filterRequest))
                    query = query.Where(x => x.ApplicationUserId == filterRequest);

                if (fromName != null)
                {
                    string fromNameString = fromName.ToString();
                    query = query.Where(x => string.Compare(x.NomeEvento.Substring(0, 1), fromNameString) >= 0);
                }

                if (toName != null)
                {
                    string toNameString = toName.ToString();
                    query = query.Where(x => string.Compare(x.NomeEvento.Substring(0, 1), toNameString) <= 0);
                }

                ListViewModel<CalendarSelectModel> result = new ListViewModel<CalendarSelectModel>();

                result.Total = await query.CountAsync();

                
                List<Calendar> queryList = await query
                    //.Include(x => x.CalendarType)
                    .ToListAsync();

                result.Data = _mapper.Map<List<CalendarSelectModel>>(queryList);

                _logger.LogInformation(nameof(Get));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Si è verificato un errore");
            }
        }

        public async Task<CalendarCreateViewModel> GetToInsert(string agencyId)
        {
            try
            {
                List<Customer> customers = await _unitOfWork.dbContext.Customers.ToListAsync();
                List<RealEstateProperty> properties = await _unitOfWork.dbContext.RealEstateProperties.Where(x => x.Agent.AgencyId == agencyId).ToListAsync();
                List<Request> requests = await _unitOfWork.dbContext.Requests.ToListAsync();

                CalendarCreateViewModel result = new CalendarCreateViewModel();
                result.Customers = _mapper.Map<List<CustomerSelectModel>>(customers);
                result.RealEstateProperties = _mapper.Map<List<RealEstatePropertySelectModel>>(properties);
                result.Requests = _mapper.Map<List<RequestSelectModel>>(requests);

                _logger.LogInformation(nameof(GetToInsert));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Si è verificato un errore");
            }
        }

        public async Task<CalendarSearchModel> GetSearchItems(string agencyId)
        {
            try
            {
                ApplicationUser user = await userManager.FindByIdAsync(agencyId);
                List<UserSelectModel> agencies = new List<UserSelectModel>();
                List<UserSelectModel> agents = new List<UserSelectModel>();
                if(await userManager.IsInRoleAsync(user, "Admin"))
                {
                    var agenciesList = await userManager.GetUsersInRoleAsync("Agency");
                    agencies = _mapper.Map<List<UserSelectModel>>(agenciesList);
                }

                if(await userManager.IsInRoleAsync(user, "Agency"))
                {
                    var agentsList = await userManager.GetUsersInRoleAsync("Agent");
                    agents = _mapper.Map<List<UserSelectModel>>(agentsList);
                }

                CalendarSearchModel result = new CalendarSearchModel()
                {
                    Agencies = agencies,
                    Agents = agents
                };

                _logger.LogInformation(nameof(GetSearchItems));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Si è verificato un errore");
            }
        }

        public async Task<CalendarSelectModel> GetById(int id)
        {
            try
            {
                if (id is not > 0)
                    throw new Exception("Si è verificato un errore!");

                var query = await _unitOfWork.dbContext.Calendars
                    //.Include(x => x.CalendarType)
                    .FirstOrDefaultAsync(x => x.Id == id);

                CalendarSelectModel result = _mapper.Map<CalendarSelectModel>(query);

                _logger.LogInformation(nameof(GetById));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Si è verificato un errore");
            }
        }

        public async Task<CalendarSelectModel> Update(CalendarUpdateModel dto)
        {
            try
            {
                var EntityClass =
                    await _unitOfWork.CalendarRepository.FirstOrDefaultAsync(q => q.Where(x => x.Id == dto.Id));

                if (EntityClass == null)
                    throw new NullReferenceException("Record non trovato!");

                EntityClass = _mapper.Map(dto, EntityClass);

                _unitOfWork.CalendarRepository.Update(EntityClass);
                await _unitOfWork.SaveAsync();

                CalendarSelectModel response = new CalendarSelectModel();
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