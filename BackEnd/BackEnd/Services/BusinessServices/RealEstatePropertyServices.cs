using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using BackEnd.Entities;
using BackEnd.Interfaces;
using BackEnd.Interfaces.IBusinessServices;
using BackEnd.Models.CustomerModels;
using BackEnd.Models.Options;
using BackEnd.Models.OutputModels;
using BackEnd.Models.RealEstatePropertyModels;

namespace BackEnd.Services.BusinessServices
{
    public class RealEstatePropertyServices : IRealEstatePropertyServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RealEstatePropertyServices> _logger;
        private readonly IOptionsMonitor<PaginationOptions> options;
        private readonly IStorageServices _storageServices;
        public RealEstatePropertyServices(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            ILogger<RealEstatePropertyServices> logger, 
            IOptionsMonitor<PaginationOptions> options,
            IStorageServices storageServices)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            this.options = options;
            _storageServices = storageServices;

        }
        public async Task<RealEstatePropertySelectModel> Create(RealEstatePropertyCreateModel dto)
        {
            try
            {
                if (dto.Photos?.Count > 0)
                {

                }

                var entityClass = _mapper.Map<RealEstateProperty>(dto);
                await _unitOfWork.RealEstatePropertyRepository.InsertAsync(entityClass);
                _unitOfWork.Save();

                RealEstatePropertySelectModel response = new RealEstatePropertySelectModel();
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

        public async Task<RealEstateProperty> Delete(int id)
        {
            try
            {
                IQueryable<RealEstateProperty> query = _unitOfWork.dbContext.RealEstatePropertys;

                if (id == 0)
                    throw new NullReferenceException("L'id non può essere 0");

                query = query.Where(x => x.Id == id);

                RealEstateProperty EntityClasses = await query.FirstOrDefaultAsync();

                if (EntityClasses == null)
                    throw new NullReferenceException("Record non trovato!");

                _unitOfWork.RealEstatePropertyRepository.Delete(EntityClasses);
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

        public async Task<ListViewModel<RealEstatePropertySelectModel>> Get(int currentPage, string? filterRequest, char? fromName, char? toName)
        {
            try
            {
                IQueryable<RealEstateProperty> query = _unitOfWork.dbContext.RealEstatePropertys;

                if (!string.IsNullOrEmpty(filterRequest))
                    query = query.Where(x => x.Category.Contains(filterRequest));

                if (fromName != null)
                {
                    string fromNameString = fromName.ToString();
                    query = query.Where(x => string.Compare(x.Category.Substring(0, 1), fromNameString) >= 0);
                }

                if (toName != null)
                {
                    string toNameString = toName.ToString();
                    query = query.Where(x => string.Compare(x.Category.Substring(0, 1), toNameString) <= 0);
                }

                ListViewModel<RealEstatePropertySelectModel> result = new ListViewModel<RealEstatePropertySelectModel>();

                result.Total = await query.CountAsync();

                if (currentPage > 0)
                {
                    query = query
                    .Skip((currentPage * options.CurrentValue.RealEstatePropertyItemPerPage) - options.CurrentValue.RealEstatePropertyItemPerPage)
                            .Take(options.CurrentValue.RealEstatePropertyItemPerPage);
                }

                List<RealEstateProperty> queryList = await query
                    //.Include(x => x.RealEstatePropertyType)
                    .ToListAsync();

                result.Data = _mapper.Map<List<RealEstatePropertySelectModel>>(queryList);

                _logger.LogInformation(nameof(Get));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Si è verificato un errore");
            }
        }

        public async Task<RealEstatePropertySelectModel> GetById(int id)
        {
            try
            {
                if (id is not > 0)
                    throw new Exception("Si è verificato un errore!");

                var query = await _unitOfWork.dbContext.RealEstatePropertys
                    //.Include(x => x.RealEstatePropertyType)
                    .FirstOrDefaultAsync(x => x.Id == id);

                RealEstatePropertySelectModel result = _mapper.Map<RealEstatePropertySelectModel>(query);

                _logger.LogInformation(nameof(GetById));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Si è verificato un errore");
            }
        }

        public async Task<RealEstatePropertySelectModel> Update(RealEstatePropertyUpdateModel dto)
        {
            try
            {
                var EntityClass =
                    await _unitOfWork.RealEstatePropertyRepository.FirstOrDefaultAsync(q => q.Where(x => x.Id == dto.Id));

                if (EntityClass == null)
                    throw new NullReferenceException("Record non trovato!");

                EntityClass = _mapper.Map(dto, EntityClass);

                _unitOfWork.RealEstatePropertyRepository.Update(EntityClass);
                await _unitOfWork.SaveAsync();

                RealEstatePropertySelectModel response = new RealEstatePropertySelectModel();
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
