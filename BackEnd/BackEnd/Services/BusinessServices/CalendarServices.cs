﻿using AutoMapper;
using BackEnd.Entities;
using BackEnd.Interfaces;
using BackEnd.Interfaces.IBusinessServices;
using BackEnd.Models.CalendarModels;
using BackEnd.Models.CustomerModels;
using BackEnd.Models.OutputModels;
using BackEnd.Models.RealEstatePropertyModels;
using BackEnd.Models.RequestModels;
using BackEnd.Models.UserModel;
using DocumentFormat.OpenXml.Spreadsheet;
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
                //dto.DataInizioEvento = dto.DataInizioEvento.AddHours(1);
                //dto.DataFineEvento = dto.DataFineEvento.AddHours(1);
                var entityClass = _mapper.Map<Calendar>(dto);
                var result = await _unitOfWork.CalendarRepository.InsertAsync(entityClass);
                _unitOfWork.Save();

                ApplicationUser user = await userManager.FindByIdAsync(entityClass.ApplicationUserId);

                if(entityClass.RequestId > 0 && entityClass.RequestId != null)
                {
                    RequestNotes note = new RequestNotes()
                    {
                        ApplicationUserId = entityClass.ApplicationUserId,
                        CalendarId = result.Entity.Id,
                        RequestId = entityClass.RequestId ?? 0,
                        Text = $"<strong>Nota di</strong>: {user.Name} {user.LastName} <br> <strong>Titolo</strong>: {entityClass.NomeEvento}"
                    };

                    await _unitOfWork.dbContext.RequestNotes.AddAsync(note);
                    _unitOfWork.Save();
                }

                if (entityClass.RealEstatePropertyId > 0 && entityClass.RealEstatePropertyId != null)
                {
                    RealEstatePropertyNotes note = new RealEstatePropertyNotes()
                    {
                        ApplicationUserId = entityClass.ApplicationUserId,
                        CalendarId = result.Entity.Id,
                        RealEstatePropertyId = entityClass.RealEstatePropertyId ?? 0,
                        Text = $"<strong>Nota di</strong>: {user.Name} {user.LastName} <br> <strong>Titolo</strong>: {entityClass.NomeEvento}"
                    };
                    await _unitOfWork.dbContext.RealEstatePropertyNotes.AddAsync(note);
                    _unitOfWork.Save();
                }

                if (entityClass.CustomerId > 0 && entityClass.CustomerId != null)
                {
                    CustomerNotes note = new CustomerNotes()
                    {
                        ApplicationUserId = entityClass.ApplicationUserId,
                        CalendarId = result.Entity.Id,
                        CustomerId = entityClass.CustomerId ?? 0,
                        Text = $"<strong>Nota di</strong>: {user.Name} {user.LastName} <br> <strong>Titolo</strong>: {entityClass.NomeEvento}"
                    };

                    await _unitOfWork.dbContext.CustomerNotes.AddAsync(note);
                    _unitOfWork.Save();
                }

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

                Calendar entityClass = await query.FirstOrDefaultAsync();

                if (entityClass == null)
                    throw new NullReferenceException("Record non trovato!");

                if (entityClass.RequestId > 0 && entityClass.RequestId != null)
                {
                    RequestNotes? note = await _unitOfWork.dbContext.RequestNotes.FirstOrDefaultAsync(x => x.CalendarId == entityClass.Id);
                    if(note != null)
                    {
                        _unitOfWork.dbContext.RequestNotes.Remove(note);
                        _unitOfWork.Save();
                    }
                }

                if (entityClass.RealEstatePropertyId > 0 && entityClass.RealEstatePropertyId != null)
                {
                    RealEstatePropertyNotes? note = await _unitOfWork.dbContext.RealEstatePropertyNotes.FirstOrDefaultAsync(x => x.CalendarId == entityClass.Id);
                    if (note != null)
                    {
                        _unitOfWork.dbContext.RealEstatePropertyNotes.Remove(note);
                        _unitOfWork.Save();
                    } 
                }

                if (entityClass.CustomerId > 0 && entityClass.CustomerId != null)
                {
                    CustomerNotes? note = await _unitOfWork.dbContext.CustomerNotes.FirstOrDefaultAsync(x => x.CalendarId == entityClass.Id);
                    if (note != null)
                    {
                        _unitOfWork.dbContext.CustomerNotes.Remove(note);
                        _unitOfWork.Save();
                    }
                }

                _unitOfWork.CalendarRepository.Delete(entityClass);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation(nameof(Delete));

                return entityClass;
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
                {
                    ApplicationUser user = await userManager.FindByIdAsync(filterRequest);
                    if (await userManager.IsInRoleAsync(user, "Admin") || await userManager.IsInRoleAsync(user, "Agency"))
                    {
                        query = query.Where(x => x.ApplicationUserId == filterRequest && x.ApplicationUser.AgencyId == filterRequest);
                    }
                    else
                    {
                        query = query.Where(x => x.ApplicationUserId == filterRequest);
                    }                    
                }
                    
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
                    .Include(x => x.ApplicationUser)
                    .AsNoTracking()
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

        private async Task HandleRequestNotes(Calendar entityClass, int? requestId)
        {
            var existingRequestNote = await _unitOfWork.dbContext.RequestNotes.FirstOrDefaultAsync(x => x.CalendarId == entityClass.Id);
            if (requestId != null && (entityClass.RequestId == null || entityClass.RequestId != requestId))
            {
                if (existingRequestNote == null)
                {
                    existingRequestNote = new RequestNotes
                    {
                        ApplicationUserId = entityClass.ApplicationUserId,
                        CalendarId = entityClass.Id,
                        RequestId = requestId.Value,
                        Text = $"<strong>Nota di</strong>: {entityClass.ApplicationUser.Name} {entityClass.ApplicationUser.LastName} <br> <strong>Titolo</strong>: {entityClass.NomeEvento}"
                    };
                    await _unitOfWork.dbContext.RequestNotes.AddAsync(existingRequestNote);
                }
                else
                {
                    existingRequestNote.RequestId = requestId.Value;
                    existingRequestNote.Text = $"<strong>Nota di</strong>: {entityClass.ApplicationUser.Name} {entityClass.ApplicationUser.LastName} <br> <strong>Titolo</strong>: {entityClass.NomeEvento}";
                    _unitOfWork.dbContext.RequestNotes.Update(existingRequestNote);
                }
            }
            else if (requestId == null && entityClass.RequestId != null && existingRequestNote != null)
            {
                _unitOfWork.dbContext.RequestNotes.Remove(existingRequestNote);
            }
        }

        private async Task HandleRealEstatePropertyNotes(Calendar entityClass, int? realEstatePropertyId)
        {
            var existingPropertyNote = await _unitOfWork.dbContext.RealEstatePropertyNotes.FirstOrDefaultAsync(x => x.CalendarId == entityClass.Id);
            if (realEstatePropertyId > 0)
            {
                if (existingPropertyNote == null)
                {
                    existingPropertyNote = new RealEstatePropertyNotes
                    {
                        ApplicationUserId = entityClass.ApplicationUserId,
                        RealEstatePropertyId = realEstatePropertyId ?? 0,
                        CalendarId = entityClass.Id,
                        Text = $"<strong>Nota di</strong>: {entityClass.ApplicationUser.Name} {entityClass.ApplicationUser.LastName} <br> <strong>Titolo</strong>: {entityClass.NomeEvento}"
                    };
                    await _unitOfWork.dbContext.RealEstatePropertyNotes.AddAsync(existingPropertyNote);
                }
                else
                {
                    existingPropertyNote.Text = $"<strong>Nota di</strong>: {entityClass.ApplicationUser.Name} {entityClass.ApplicationUser.LastName} <br> <strong>Titolo</strong>: {entityClass.NomeEvento}";
                    _unitOfWork.dbContext.RealEstatePropertyNotes.Update(existingPropertyNote);
                }
            }
            else if (existingPropertyNote != null)
            {
                _unitOfWork.dbContext.RealEstatePropertyNotes.Remove(existingPropertyNote);
            }
        }

        private async Task HandleCustomerNotes(Calendar entityClass, int? customerId)
        {
            var existingCustomerNote = await _unitOfWork.dbContext.CustomerNotes.FirstOrDefaultAsync(x => x.CalendarId == entityClass.Id);
            if (customerId > 0)
            {
                if (existingCustomerNote == null)
                {
                    existingCustomerNote = new CustomerNotes
                    {
                        ApplicationUserId = entityClass.ApplicationUserId,
                        CustomerId = customerId ?? 0,
                        CalendarId = entityClass.Id,
                        Text = $"<strong>Nota di</strong>: {entityClass.ApplicationUser.Name} {entityClass.ApplicationUser.LastName} <br> <strong>Titolo</strong>: {entityClass.NomeEvento}"
                    };
                    await _unitOfWork.dbContext.CustomerNotes.AddAsync(existingCustomerNote);
                }
                else
                {
                    existingCustomerNote.Text = $"<strong>Nota di</strong>: {entityClass.ApplicationUser.Name} {entityClass.ApplicationUser.LastName} <br> <strong>Titolo</strong>: {entityClass.NomeEvento}";
                    _unitOfWork.dbContext.CustomerNotes.Update(existingCustomerNote);
                }
            }
            else if (existingCustomerNote != null)
            {
                _unitOfWork.dbContext.CustomerNotes.Remove(existingCustomerNote);
            }
        }


        public async Task<CalendarSelectModel> Update(CalendarUpdateModel dto)
        {
            try
            {
                var entityClass =
                    await _unitOfWork.dbContext.Calendars.Include(x => x.ApplicationUser).FirstOrDefaultAsync(x => x.Id == dto.Id);

                if (entityClass == null)
                    throw new NullReferenceException("Record non trovato!");

                //dto.DataInizioEvento = dto.DataInizioEvento.AddHours(1);
                //dto.DataFineEvento = dto.DataFineEvento.AddHours(1);

                await HandleRequestNotes(entityClass, dto.RequestId);
                await HandleRealEstatePropertyNotes(entityClass, dto.RealEstatePropertyId);
                await HandleCustomerNotes(entityClass, dto.CustomerId);

                entityClass = _mapper.Map(dto, entityClass);

                _unitOfWork.CalendarRepository.Update(entityClass);
                await _unitOfWork.SaveAsync();

                CalendarSelectModel response = new CalendarSelectModel();
                _mapper.Map(entityClass, response);

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