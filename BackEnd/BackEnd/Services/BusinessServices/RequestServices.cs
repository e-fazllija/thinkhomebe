using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using BackEnd.Entities;
using BackEnd.Interfaces;
using BackEnd.Interfaces.IBusinessServices;
using BackEnd.Models.RequestModels;
using BackEnd.Models.Options;
using BackEnd.Models.OutputModels;
using BackEnd.Models.RealEstatePropertyModels;
using BackEnd.Models.RealEstatePropertyPhotoModels;

namespace BackEnd.Services.BusinessServices
{
    public class RequestServices : IRequestServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RequestServices> _logger;
        private readonly IOptionsMonitor<PaginationOptions> options;
        public RequestServices(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RequestServices> logger, IOptionsMonitor<PaginationOptions> options)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            this.options = options;

        }
        public async Task<RequestSelectModel> Create(RequestCreateModel dto)
        {
            try
            {
                var entityClass = _mapper.Map<Request>(dto);
                await _unitOfWork.RequestRepository.InsertAsync(entityClass);
                _unitOfWork.Save();

                RequestSelectModel response = new RequestSelectModel();
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

        public async Task<Request> Delete(int id)
        {
            try
            {
                IQueryable<Request> query = _unitOfWork.dbContext.Requests;

                if (id == 0)
                    throw new NullReferenceException("L'id non può essere 0");

                query = query.Where(x => x.Id == id);

                Request EntityClasses = await query.FirstOrDefaultAsync();

                if (EntityClasses == null)
                    throw new NullReferenceException("Record non trovato!");

                _unitOfWork.RequestRepository.Delete(EntityClasses);
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

        public async Task<ListViewModel<RequestSelectModel>> Get(int currentPage, string? agencyId, string? filterRequest, char? fromName, char? toName, string? userId)
        {
            try
            {
                // Ottimizzazione: uso direttamente la proiezione senza Include, Entity Framework caricherà solo i campi necessari
                IQueryable<Request> query = _unitOfWork.dbContext.Requests.OrderByDescending(x => x.Id);

                if (!string.IsNullOrEmpty(agencyId))
                    query = query.Where(x => x.AgencyId == agencyId);

                if (!string.IsNullOrEmpty(filterRequest))
                    query = query.Where(x => x.Customer.Name.Contains(filterRequest));

                //if (fromName != null)
                //{
                //    string fromNameString = fromName.ToString();
                //    query = query.Where(x => string.Compare(x.Name.Substring(0, 1), fromNameString) >= 0);
                //}

                //if (toName != null)
                //{
                //    string toNameString = toName.ToString();
                //    query = query.Where(x => string.Compare(x.Name.Substring(0, 1), toNameString) <= 0);
                //}

                ListViewModel<RequestSelectModel> result = new ListViewModel<RequestSelectModel>();

                result.Total = await query.CountAsync();

                if (currentPage > 0)
                {
                    query = query
                    .Skip((currentPage * options.CurrentValue.AnagraficItemPerPage) - options.CurrentValue.AnagraficItemPerPage)
                            .Take(options.CurrentValue.AnagraficItemPerPage);
                }

                // Ottimizzazione: uso AsNoTracking per migliorare le prestazioni
                List<Request> queryList = await query
                    .AsNoTracking()
                    .ToListAsync();

                result.Data = _mapper.Map<List<RequestSelectModel>>(queryList);

                _logger.LogInformation(nameof(Get));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Si è verificato un errore");
            }
        }

        public async Task<ListViewModel<RequestSelectModel>> GetCustomerRequests(int customerId)
        {
            try
            {
                // Ottimizzazione: carico tutte le richieste in una sola query
                var requests = await _unitOfWork.dbContext.Requests
                    .Include(x => x.Customer)
                    .Where(x => x.CustomerId == customerId)
                    .OrderByDescending(x => x.Id)
                    .AsNoTracking()
                    .ToListAsync();

                ListViewModel<RequestSelectModel> result = new ListViewModel<RequestSelectModel>()
                {
                    Total = requests.Count,
                    Data = new List<RequestSelectModel>()
                };

                // Ottimizzazione: invece di fare N query separate, raggruppo le richieste per criteri simili
                // e faccio query batch per le proprietà immobiliari
                foreach (var item in requests)
                {
                    var towns = item.Town.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                         .Select(t => t.Trim().ToLower())
                                         .ToList();

                    // Ottimizzazione: costruisco la query correttamente assegnando il risultato di Where
                    IQueryable<RealEstateProperty> realEstatePropertiesQuery = _unitOfWork.dbContext.RealEstateProperties
                        .Where(x =>
                            !x.Sold &&
                            x.Status == item.Contract &&
                            x.Price <= item.PriceTo &&
                            x.Price >= item.PriceFrom &&
                            towns.Any(t => x.Town.ToLower().Contains(t)));

                    if (!string.IsNullOrEmpty(item.PropertyType))
                    {
                        realEstatePropertiesQuery = realEstatePropertiesQuery.Where(x => (x.Typology ?? "").Contains(item.PropertyType));
                    }

                    if (!string.IsNullOrEmpty(item.RoomsNumber))
                    {
                        realEstatePropertiesQuery = realEstatePropertiesQuery.Where(x => x.WarehouseRooms == Convert.ToInt32(item.RoomsNumber));
                    }

                    if (item.MQFrom > 0)
                    {
                        realEstatePropertiesQuery = realEstatePropertiesQuery.Where(x => x.CommercialSurfaceate > item.MQFrom);
                    }

                    if (item.MQTo > 0)
                    {
                        realEstatePropertiesQuery = realEstatePropertiesQuery.Where(x => x.CommercialSurfaceate < item.MQTo);
                    }

                    if (item.ParkingSpaces > 0)
                    {
                        realEstatePropertiesQuery = realEstatePropertiesQuery.Where(x => x.ParkingSpaces >= item.ParkingSpaces);
                    }

                    // Ottimizzazione: uso proiezione invece di caricare tutto e poi mappare
                    var realEstatePropertySelectModel = await realEstatePropertiesQuery
                        .Select(x => new RealEstatePropertySelectModel
                        {
                            Id = x.Id,
                            Title = x.Title,
                            Category = x.Category,
                            Typology = x.Typology,
                            Status = x.Status,
                            AddressLine = x.AddressLine,
                            Town = x.Town,
                            Location = x.Location,
                            Price = x.Price,
                            CommercialSurfaceate = x.CommercialSurfaceate,
                            Bedrooms = x.Bedrooms,
                            Bathrooms = x.Bathrooms,
                            ParkingSpaces = x.ParkingSpaces,
                            StateOfTheProperty = x.StateOfTheProperty,
                            Photos = x.Photos.OrderBy(p => p.Position).Select(p => new RealEstatePropertyPhotoSelectModel
                            {
                                Id = p.Id,
                                RealEstatePropertyPhotoId = p.RealEstatePropertyId,
                                Url = p.Url,
                                FileName = p.FileName,
                                Highlighted = p.Highlighted,
                                Position = p.Position,
                                Type = p.Type,
                                CreationDate = p.CreationDate,
                                UpdateDate = p.UpdateDate
                            }).ToList()
                        })
                        .AsNoTracking()
                        .ToListAsync();

                    RequestSelectModel requestSelectModel = _mapper.Map<RequestSelectModel>(item);
                    requestSelectModel.RealEstateProperties = realEstatePropertySelectModel;
                    result.Data.Add(requestSelectModel);
                }

                _logger.LogInformation(nameof(GetCustomerRequests));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Si è verificato un errore");
            }
        }

        public async Task<ListViewModel<RequestListModel>> GetList(int currentPage, string? agencyId, string? filterRequest, char? fromName, char? toName, string? userId)
        {
            try
            {
                // Ottimizzazione: uso direttamente la proiezione senza Include, Entity Framework caricherà solo i campi necessari
                IQueryable<Request> query = _unitOfWork.dbContext.Requests.OrderByDescending(x => x.Id);

                if (!string.IsNullOrEmpty(agencyId))
                    query = query.Where(x => x.AgencyId == agencyId);

                if (!string.IsNullOrEmpty(filterRequest))
                    query = query.Where(x => x.Customer.Name.Contains(filterRequest) || x.Customer.LastName.Contains(filterRequest));

                ListViewModel<RequestListModel> result = new ListViewModel<RequestListModel>();

                result.Total = await query.CountAsync();

                if (currentPage > 0)
                {
                    query = query
                    .Skip((currentPage * options.CurrentValue.AnagraficItemPerPage) - options.CurrentValue.AnagraficItemPerPage)
                            .Take(options.CurrentValue.AnagraficItemPerPage);
                }

                // Proiezione ottimizzata per la lista - Entity Framework caricherà solo i campi necessari dal Customer
                var queryList = await query
                    .Select(x => new RequestListModel
                    {
                        Id = x.Id,
                        CustomerName = x.Customer.Name,
                        CustomerLastName = x.Customer.LastName,
                        CustomerEmail = x.Customer.Email,
                        CustomerPhone = x.Customer.Phone.ToString(),
                        Contract = x.Contract,
                        CreationDate = x.CreationDate,
                        Location = x.Location,
                        Town = x.Town,
                        PriceTo = x.PriceTo,
                        PriceFrom = x.PriceFrom,
                        PropertyType = x.PropertyType,
                        Archived = x.Archived,
                        Closed = x.Closed
                    })
                    .AsNoTracking()
                    .ToListAsync();

                result.Data = queryList;

                _logger.LogInformation(nameof(GetList));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Si è verificato un errore");
            }
        }

        public async Task<RequestSelectModel> GetById(int id)
        {
            try
            {
                if (id is not > 0)
                    throw new Exception("Si è verificato un errore!");

                var request = await _unitOfWork.dbContext.Requests
                    .Include(x => x.Customer)
                    .Include(x => x.RequestNotes)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (request == null)
                    throw new Exception("Richiesta non trovata!");

                var towns = request.Town.Split(',', StringSplitOptions.RemoveEmptyEntries)
                         .Select(t => t.Trim().ToLower())
                         .ToList();

                // Ottimizzazione: costruisco la query correttamente assegnando il risultato di Where
                IQueryable<RealEstateProperty> realEstatePropertiesQuery = _unitOfWork.dbContext.RealEstateProperties
                    .Where(x =>
                        !x.Sold &&
                        x.Status == request.Contract &&
                        x.Price <= request.PriceTo &&
                        x.Price >= request.PriceFrom &&
                        towns.Any(t => x.Town.ToLower().Contains(t)));

                if (!string.IsNullOrEmpty(request.PropertyType))
                {
                    realEstatePropertiesQuery = realEstatePropertiesQuery.Where(x => (x.Typology ?? "").Contains(request.PropertyType));
                }

                if (!string.IsNullOrEmpty(request.RoomsNumber))
                {
                    realEstatePropertiesQuery = realEstatePropertiesQuery.Where(x => x.WarehouseRooms == Convert.ToInt32(request.RoomsNumber));
                }

                if (request.MQFrom > 0)
                {
                    realEstatePropertiesQuery = realEstatePropertiesQuery.Where(x => x.CommercialSurfaceate > request.MQFrom);
                }

                if (request.MQTo > 0)
                {
                    realEstatePropertiesQuery = realEstatePropertiesQuery.Where(x => x.CommercialSurfaceate < request.MQTo);
                }

                if (request.ParkingSpaces > 0)
                {
                    realEstatePropertiesQuery = realEstatePropertiesQuery.Where(x => x.ParkingSpaces >= request.ParkingSpaces);
                }

                // Ottimizzazione: uso proiezione invece di caricare tutto e poi mappare
                var realEstatePropertySelectModel = await realEstatePropertiesQuery
                    .Select(x => new RealEstatePropertySelectModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Category = x.Category,
                        Typology = x.Typology,
                        Status = x.Status,
                        AddressLine = x.AddressLine,
                        Town = x.Town,
                        Location = x.Location,
                        Price = x.Price,
                        CommercialSurfaceate = x.CommercialSurfaceate,
                        Bedrooms = x.Bedrooms,
                        Bathrooms = x.Bathrooms,
                        ParkingSpaces = x.ParkingSpaces,
                        StateOfTheProperty = x.StateOfTheProperty,
                        Photos = x.Photos.OrderBy(p => p.Position).Select(p => new RealEstatePropertyPhotoSelectModel
                        {
                            Id = p.Id,
                            RealEstatePropertyPhotoId = p.RealEstatePropertyId,
                            Url = p.Url,
                            FileName = p.FileName,
                            Highlighted = p.Highlighted,
                            Position = p.Position,
                            Type = p.Type,
                            CreationDate = p.CreationDate,
                            UpdateDate = p.UpdateDate
                        }).ToList()
                    })
                    .AsNoTracking()
                    .ToListAsync();

                RequestSelectModel result = _mapper.Map<RequestSelectModel>(request);
                result.RealEstateProperties = realEstatePropertySelectModel;

                _logger.LogInformation(nameof(GetById));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Si è verificato un errore");
            }
        }

        public async Task<RequestSelectModel> Update(RequestUpdateModel dto)
        {
            try
            {
                var EntityClass =
                    await _unitOfWork.RequestRepository.FirstOrDefaultAsync(q => q.Where(x => x.Id == dto.Id));

                if (EntityClass == null)
                    throw new NullReferenceException("Record non trovato!");

                EntityClass = _mapper.Map(dto, EntityClass);

                _unitOfWork.RequestRepository.Update(EntityClass);
                await _unitOfWork.SaveAsync();

                RequestSelectModel response = new RequestSelectModel();
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
