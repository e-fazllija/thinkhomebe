using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using BackEnd.Entities;
using BackEnd.Interfaces;
using BackEnd.Interfaces.IBusinessServices;
using BackEnd.Models.Options;
using BackEnd.Models.OutputModels;
using BackEnd.Models.RealEstatePropertyModels;
using BackEnd.Models.CustomerModels;
using Microsoft.AspNetCore.Identity;
using BackEnd.Models.UserModel;

namespace BackEnd.Services.BusinessServices
{
    public class RealEstatePropertyServices : IRealEstatePropertyServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RealEstatePropertyServices> _logger;
        private readonly IOptionsMonitor<PaginationOptions> options;
        private readonly IStorageServices _storageServices;
        private readonly UserManager<ApplicationUser> userManager;
        public RealEstatePropertyServices(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<RealEstatePropertyServices> logger,
            IOptionsMonitor<PaginationOptions> options,
            IStorageServices storageServices,
            UserManager<ApplicationUser> userManager
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            this.options = options;
            _storageServices = storageServices;
            this.userManager = userManager;
        }
        public async Task<RealEstatePropertySelectModel> Create(RealEstatePropertyCreateModel dto)
        {
            try
            {
                var entityClass = _mapper.Map<RealEstateProperty>(dto);
                var propertyAdded = await _unitOfWork.RealEstatePropertyRepository.InsertAsync(entityClass);
                _unitOfWork.Save();

                if (dto.Files?.Count > 0)
                {
                    foreach (var file in dto.Files)
                    {
                        Stream stream = file.OpenReadStream();
                        string fileName = $"RealEstatePropertyPhotos/{propertyAdded.Entity.Id}/{file.FileName.Replace(" ", "-")}";
                        string fileUrl = await _storageServices.UploadFile(stream, fileName);

                        RealEstatePropertyPhoto photo = new RealEstatePropertyPhoto()
                        {
                            RealEstatePropertyId = propertyAdded.Entity.Id,
                            FileName = fileName,
                            Url = fileUrl,
                            Type = 1
                        };

                        await _unitOfWork.RealEstatePropertyPhotoRepository.InsertAsync(photo);
                        _unitOfWork.Save();
                    }

                }

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

        public async Task InsertFiles(UploadFilesModel dto)
        {
            try
            {
                foreach (var file in dto.Files)
                {
                    Stream stream = file.OpenReadStream();
                    string fileName = $"RealEstatePropertyPhotos/{dto.PropertyId}/{file.FileName.Replace(" ", "-")}";
                    string fileUrl = await _storageServices.UploadFile(stream, fileName);

                    RealEstatePropertyPhoto photo = new RealEstatePropertyPhoto()
                    {
                        RealEstatePropertyId = dto.PropertyId,
                        FileName = fileName,
                        Url = fileUrl,
                        Type = 1
                    };

                    await _unitOfWork.RealEstatePropertyPhotoRepository.InsertAsync(photo);
                    _unitOfWork.Save();
                }

                _logger.LogInformation(nameof(Create));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Si è verificato un errore in fase creazione");
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                IQueryable<RealEstateProperty> query = _unitOfWork.dbContext.RealEstateProperties.Include(x => x.Photos);

                if (id == 0)
                    throw new NullReferenceException("L'id non può essere 0");

                query = query.Where(x => x.Id == id);

                RealEstateProperty EntityClasses = await query.FirstOrDefaultAsync();

                if (EntityClasses == null)
                    throw new NullReferenceException("Record non trovato!");

                _unitOfWork.RealEstatePropertyRepository.Delete(EntityClasses);
                await _unitOfWork.SaveAsync();

                foreach (var photo in EntityClasses.Photos)
                {
                    await _storageServices.DeleteFile(photo.FileName);
                }

                _unitOfWork.dbContext.RealEstatePropertyPhotos.RemoveRange(EntityClasses.Photos);

                _logger.LogInformation(nameof(Delete));
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

        public async Task<ListViewModel<RealEstatePropertySelectModel>> Get(int currentPage, string? filterRequest, string? status, string? typologie, char? fromName, char? toName)
        {
            try
            {
                IQueryable<RealEstateProperty> query = _unitOfWork.dbContext.RealEstateProperties.Include(x => x.Photos).OrderByDescending(x => x.Id);

                if (!string.IsNullOrEmpty(filterRequest))
                    query = query.Where(x => x.AddressLine.Contains(filterRequest));

                if (!string.IsNullOrEmpty(status))
                    query = query.Where(x => x.Status.Contains(status));
                
                if (!string.IsNullOrEmpty(typologie))
                    query = query.Where(x => x.Typology!.Contains(typologie));

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

        public async Task<RealEstatePropertyCreateViewModel> GetToInsert()
        {
            try
            {
                IQueryable<Customer> customerQuery = _unitOfWork.dbContext.Customers;
                var usersList = await userManager.GetUsersInRoleAsync("Agent");

                List<ApplicationUser> users = usersList.ToList();

                RealEstatePropertyCreateViewModel result = new RealEstatePropertyCreateViewModel();
                List<Customer> customers = await customerQuery.ToListAsync();
                result.Customers = _mapper.Map<List<CustomerSelectModel>>(customers);
                result.Agents = _mapper.Map<List<UserSelectModel>>(users);

                _logger.LogInformation(nameof(GetToInsert));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Si è verificato un errore");
            }
        }

        public async Task SetHighlighted(int realEstatePropertyId)
        {
            try
            {
                IQueryable<RealEstateProperty> query = _unitOfWork.dbContext.RealEstateProperties.Where(x => x.Id == realEstatePropertyId);
                IQueryable<RealEstateProperty> queryHighlighted = _unitOfWork.dbContext.RealEstateProperties.Where(x => x.Highlighted == true);

                RealEstateProperty propertyHighlighted = await query.FirstAsync();
                propertyHighlighted.Highlighted = false;
                _unitOfWork.dbContext.RealEstateProperties.Update(propertyHighlighted);
                await _unitOfWork.SaveAsync();

                RealEstateProperty property = await query.FirstAsync();
                property.Highlighted = true;
                _unitOfWork.dbContext.RealEstateProperties.Update(property);
                await _unitOfWork.SaveAsync();

                _logger.LogInformation(nameof(SetHighlighted));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Si è verificato un errore");
            }
        }

        public async Task SetInHome(int realEstatePropertyId)
        {
            try
            {
                IQueryable<RealEstateProperty> query = _unitOfWork.dbContext.RealEstateProperties.Where(x => x.Id == realEstatePropertyId);

                RealEstateProperty property = await query.FirstAsync();
                property.InHome = true;
                _unitOfWork.dbContext.RealEstateProperties.Update(property);
                await _unitOfWork.SaveAsync();

                _logger.LogInformation(nameof(SetInHome));
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

                var query = await _unitOfWork.dbContext.RealEstateProperties.Include(x => x.Photos).Include(x => x.Agent)
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
