using AutoMapper;
using BackEnd.Entities;
using BackEnd.Interfaces;
using BackEnd.Models.Options;
using BackEnd.Models.OutputModels;
using BackEnd.Models.RealEstatePropertyModels;
using BackEnd.Models.RealEstatePropertyPhotoModels;
using BackEnd.Models.LocationModels;
using BackEnd.Services.BusinessServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Utilities;
using System;
using BackEnd.Interfaces.IBusinessServices;

namespace BackEnd.Services
{
    public class GenericService : IGenericService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GenericService> _logger;
        private readonly IOptionsMonitor<PaginationOptions> options;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILocationServices _locationServices;
        
        public GenericService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GenericService> logger, IOptionsMonitor<PaginationOptions> options, UserManager<ApplicationUser> userManager, ILocationServices locationServices)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            this.options = options;
            this.userManager = userManager;
            _locationServices = locationServices;
        }

        public async Task<HomeDetailsModel> GetHomeDetails()
        {
            try
            {
                HomeDetailsModel result = new HomeDetailsModel();
                List<RealEstateProperty> propertiesInHome = await _unitOfWork.dbContext.RealEstateProperties.Where(x => x.InHome).Include(x => x.Photos.OrderBy(x => x.Position)).OrderByDescending(x => x.Id).ToListAsync();

                RealEstateProperty? propertyHighlighted =
                    await _unitOfWork.dbContext.RealEstateProperties.Include(x => x.Photos).FirstOrDefaultAsync(x => x.Highlighted) ?? propertiesInHome.FirstOrDefault();

                result.RealEstatePropertiesHighlighted = _mapper.Map<RealEstatePropertySelectModel>(propertyHighlighted);

                if (result.RealEstatePropertiesHighlighted.Photos.Any(x => x.Highlighted))
                {
                    List<RealEstatePropertyPhotoSelectModel> photos = new List<RealEstatePropertyPhotoSelectModel>();
                    photos.Insert(0, result.RealEstatePropertiesHighlighted.Photos.FirstOrDefault(x => x.Highlighted)!);

                    result.RealEstatePropertiesHighlighted.Photos.Remove(result.RealEstatePropertiesHighlighted.Photos.FirstOrDefault(x => x.Highlighted)!);
                    photos.AddRange(result.RealEstatePropertiesHighlighted.Photos);
                    result.RealEstatePropertiesHighlighted.Photos = photos;
                }

                foreach (RealEstateProperty property in propertiesInHome)
                {
                    if (property.Photos.Any(x => x.Highlighted))
                    {
                        List<RealEstatePropertyPhoto> photos = new List<RealEstatePropertyPhoto>();
                        photos.Insert(0, property.Photos.FirstOrDefault(x => x.Highlighted)!);

                        property.Photos.Remove(property.Photos.FirstOrDefault(x => x.Highlighted)!);
                        photos.AddRange(property.Photos);
                        property.Photos = photos;
                    }
                }

                result.RealEstatePropertiesInHome = _mapper.Map<List<RealEstatePropertySelectModel>>(propertiesInHome);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Si è verificato un errore");
            }
        }

        public async Task<AdminHomeDetailsModel> GetAdminHomeDetails(string agencyId)
        {
            try
            {
                AdminHomeDetailsModel result = new AdminHomeDetailsModel();
                IQueryable<RealEstateProperty> propertiesInHome = _unitOfWork.dbContext.RealEstateProperties.Where(x => x.Agent.AgencyId == agencyId);

                result.RealEstatePropertyHomeDetails.Total = propertiesInHome.Count();
                result.RealEstatePropertyHomeDetails.TotalSale = propertiesInHome.Where(x => x.Status == "Vendita").Count();
                result.RealEstatePropertyHomeDetails.TotalRent = propertiesInHome.Where(x => x.Status == "Affitto").Count();
                result.RealEstatePropertyHomeDetails.TotalLastMonth = propertiesInHome.Where(x => x.CreationDate >= DateTime.Now.AddMonths(-1)).Count();

                foreach (var item in propertiesInHome.Where(x => x.Status == "Vendita"))
                {
                    if (!result.RealEstatePropertyHomeDetails.DistinctByTownSale.ContainsKey(item.Town))
                    {
                        result.RealEstatePropertyHomeDetails.DistinctByTownSale[item.Town] = 1;
                    }
                    else
                    {
                        result.RealEstatePropertyHomeDetails.DistinctByTownSale[item.Town]++;
                    }

                    //By type
                    if (!result.RealEstatePropertyHomeDetails.DistinctByTypeSale.ContainsKey(item.Typology ?? item.Category))
                    {
                        result.RealEstatePropertyHomeDetails.DistinctByTypeSale[item.Typology ?? item.Category] = 1;
                    }
                    else
                    {
                        result.RealEstatePropertyHomeDetails.DistinctByTypeSale[item.Typology ?? item.Category]++;
                    }
                }

                foreach (var item in propertiesInHome.Where(x => x.Status == "Affitto"))
                {
                    if (!result.RealEstatePropertyHomeDetails.DistinctByTownRent.ContainsKey(item.Town))
                    {
                        result.RealEstatePropertyHomeDetails.DistinctByTownRent[item.Town] = 1;
                    }
                    else
                    {
                        result.RealEstatePropertyHomeDetails.DistinctByTownRent[item.Town]++;
                    }

                    //By type
                    if (!result.RealEstatePropertyHomeDetails.DistinctByTypeRent.ContainsKey(item.Typology ?? item.Category))
                    {
                        result.RealEstatePropertyHomeDetails.DistinctByTypeRent[item.Typology ?? item.Category] = 1;
                    }
                    else
                    {
                        result.RealEstatePropertyHomeDetails.DistinctByTypeRent[item.Typology ?? item.Category]++;
                    }
                }

                result.RealEstatePropertyHomeDetails.TotalCreatedPerMonth = propertiesInHome.ToList()
                    .GroupBy(obj => obj.CreationDate.Month + "/" + obj.CreationDate.Year.ToString())
                    .ToDictionary(g => g.Key, g => g.Count());

                result.RealEstatePropertyHomeDetails.MaxAnnual = result.RealEstatePropertyHomeDetails.TotalCreatedPerMonth.Values.Any() ?
                    result.RealEstatePropertyHomeDetails.TotalCreatedPerMonth.Values.Max() : 0;

                result.RealEstatePropertyHomeDetails.MinAnnual = result.RealEstatePropertyHomeDetails.TotalCreatedPerMonth.Values.Any() ? 
                    result.RealEstatePropertyHomeDetails.TotalCreatedPerMonth.Values.Min() : 0;

                IQueryable<Request> request = _unitOfWork.dbContext.Requests.Where(x => x.AgencyId == agencyId);
                result.RequestHomeDetails.Total = request.Count();
                result.RequestHomeDetails.TotalActive = request.Where(x => !x.Closed && !x.Archived).Count();
                result.RequestHomeDetails.TotalArchived = request.Where(x => x.Archived).Count();
                result.RequestHomeDetails.TotalClosed = request.Where(x => x.Closed).Count();
                result.RequestHomeDetails.TotalLastMonth = _unitOfWork.dbContext.Requests.Where(x => x.AgencyId == agencyId && x.CreationDate >= DateTime.Now.AddMonths(-1) && !x.Closed && !x.Archived).Count();
                result.RequestHomeDetails.TotalSale = request.Where(x => x.Contract == "Vendita" && !x.Closed && !x.Archived).Count();
                result.RequestHomeDetails.TotalRent = request.Where(x => x.Contract == "Affitto" && !x.Closed && !x.Archived).Count();

                result.RequestHomeDetails.TotalCreatedPerMonth = request.ToList()
                    .GroupBy(obj => obj.CreationDate.Month + "/" + obj.CreationDate.Year.ToString())
                    .ToDictionary(g => g.Key, g => g.Count());

                result.RequestHomeDetails.MaxAnnual = result.RequestHomeDetails.TotalCreatedPerMonth.Values.Count() > 0 ?
                    result.RequestHomeDetails.TotalCreatedPerMonth.Values.Max() : 0;

                result.RequestHomeDetails.MinAnnual = result.RequestHomeDetails.TotalCreatedPerMonth.Values.Count() > 0 ?
                    result.RequestHomeDetails.TotalCreatedPerMonth.Values.Min() : 0;

                foreach (var item in request.Where(x => x.Contract == "Vendita"))
                {
                    string[] towns = item.Town.Split(',');
                    foreach(var town in towns)
                    {
                        if (!result.RequestHomeDetails.DistinctByTownSale.ContainsKey(town))
                        {
                            result.RequestHomeDetails.DistinctByTownSale[town] = 1;
                        }
                        else
                        {
                            result.RequestHomeDetails.DistinctByTownSale[town]++;
                        }
                    }

                    //By type
                    if (!result.RequestHomeDetails.DistinctByTypeSale.ContainsKey(item.PropertyType))
                    {
                        result.RequestHomeDetails.DistinctByTypeSale[item.PropertyType] = 1;
                    }
                    else
                    {
                        result.RequestHomeDetails.DistinctByTypeSale[item.PropertyType]++;
                    }
                }

                foreach (var item in request.Where(x => x.Contract == "Affitto"))
                {
                    string[] towns = item.Town.Split(',');
                    foreach (var town in towns)
                    {
                        if (!result.RequestHomeDetails.DistinctByTownRent.ContainsKey(town))
                        {
                            result.RequestHomeDetails.DistinctByTownRent[town] = 1;
                        }
                        else
                        {
                            result.RequestHomeDetails.DistinctByTownRent[town]++;
                        }
                    }

                    //By type
                    if (!result.RequestHomeDetails.DistinctByTypeRent.ContainsKey(item.PropertyType))
                    {
                        result.RequestHomeDetails.DistinctByTypeRent[item.PropertyType] = 1;
                    }
                    else
                    {
                        result.RequestHomeDetails.DistinctByTypeRent[item.PropertyType]++;
                    }
                }

                result.TotalCustomers = _unitOfWork.dbContext.Customers.Where(x => x.AgencyId == agencyId).Count();
                result.TotalAgents = userManager.GetUsersInRoleAsync("Agent").Result.Where(x => x.AgencyId == agencyId).Count();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Si è verificato un errore");
            }
        }

        public async Task<DashboardResponse> GetDashboard(ApplicationUser currentUser, string role, string? agencyId)
        {
            try
            {
                string normalizedRole = role ?? string.Empty;
                bool isAdmin = normalizedRole.Equals("Admin", StringComparison.OrdinalIgnoreCase);
                bool isAgency = normalizedRole.Equals("Agenzia", StringComparison.OrdinalIgnoreCase);
                bool isAgent = normalizedRole.Equals("Agente", StringComparison.OrdinalIgnoreCase);
                DateTime now = DateTime.Now;
                DateTime monthStart = new DateTime(now.Year, now.Month, 1);
                DateTime lastMonth = now.AddMonths(-1);

                string scopeAgencyId = string.Empty;
                if (isAdmin && !string.IsNullOrWhiteSpace(agencyId))
                {
                    scopeAgencyId = agencyId;
                }
                else if (isAgency)
                {
                    scopeAgencyId = currentUser.Id;
                }
                else if (isAgent)
                {
                    scopeAgencyId = currentUser.AgencyId ?? string.Empty;
                }

                IQueryable<RealEstateProperty> properties = _unitOfWork.dbContext.RealEstateProperties.AsQueryable();
                if (!string.IsNullOrEmpty(scopeAgencyId))
                {
                    properties = properties.Where(x => x.Agent.AgencyId == scopeAgencyId);
                }
                if (isAgent)
                {
                    properties = properties.Where(x => x.AgentId == currentUser.Id);
                }

                IQueryable<Request> requests = _unitOfWork.dbContext.Requests.AsQueryable();
                if (!string.IsNullOrEmpty(scopeAgencyId))
                {
                    requests = requests.Where(x => x.AgencyId == scopeAgencyId);
                }

                DashboardResponse result = new DashboardResponse
                {
                    Role = normalizedRole,
                    ScopeAgencyId = scopeAgencyId,
                    ScopeUserId = currentUser.Id
                };

                // Immobili
                result.RealEstatePropertyHomeDetails.Total = await properties.CountAsync();
                IQueryable<RealEstateProperty> saleProperties = properties.Where(x => x.Status == "Vendita");
                IQueryable<RealEstateProperty> rentProperties = properties.Where(x => x.Status == "Affitto");

                result.RealEstatePropertyHomeDetails.TotalSale = await saleProperties.CountAsync();
                result.RealEstatePropertyHomeDetails.TotalRent = await rentProperties.CountAsync();
                result.RealEstatePropertyHomeDetails.TotalLastMonth = await properties.Where(x => x.CreationDate >= lastMonth).CountAsync();
                result.RealEstatePropertyHomeDetails.TotalCurrentMonth = await properties.Where(x => x.CreationDate >= monthStart).CountAsync();

                result.RealEstatePropertyHomeDetails.DistinctByTownSale = await saleProperties
                    .GroupBy(x => x.Town)
                    .Select(g => new { g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Key, x => x.Count);

                result.RealEstatePropertyHomeDetails.DistinctByTownRent = await rentProperties
                    .GroupBy(x => x.Town)
                    .Select(g => new { g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Key, x => x.Count);

                result.RealEstatePropertyHomeDetails.DistinctByTypeSale = await saleProperties
                    .GroupBy(x => x.Typology ?? x.Category)
                    .Select(g => new { g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Key, x => x.Count);

                result.RealEstatePropertyHomeDetails.DistinctByTypeRent = await rentProperties
                    .GroupBy(x => x.Typology ?? x.Category)
                    .Select(g => new { g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Key, x => x.Count);

                result.RealEstatePropertyHomeDetails.TotalCreatedPerMonth = await properties
                    .GroupBy(obj => new { obj.CreationDate.Year, obj.CreationDate.Month })
                    .Select(g => new
                    {
                        Key = g.Key.Month + "/" + g.Key.Year,
                        Count = g.Count()
                    })
                    .ToDictionaryAsync(g => g.Key, g => g.Count);

                result.RealEstatePropertyHomeDetails.MaxAnnual = result.RealEstatePropertyHomeDetails.TotalCreatedPerMonth.Values.Any() ?
                    result.RealEstatePropertyHomeDetails.TotalCreatedPerMonth.Values.Max() : 0;

                result.RealEstatePropertyHomeDetails.MinAnnual = result.RealEstatePropertyHomeDetails.TotalCreatedPerMonth.Values.Any() ?
                    result.RealEstatePropertyHomeDetails.TotalCreatedPerMonth.Values.Min() : 0;

                // Richieste
                result.RequestHomeDetails.Total = await requests.CountAsync();
                result.RequestHomeDetails.TotalActive = await requests.Where(x => !x.Closed && !x.Archived).CountAsync();
                result.RequestHomeDetails.TotalArchived = await requests.Where(x => x.Archived).CountAsync();
                result.RequestHomeDetails.TotalClosed = await requests.Where(x => x.Closed).CountAsync();
                result.RequestHomeDetails.TotalLastMonth = await requests.Where(x => x.CreationDate >= lastMonth && !x.Closed && !x.Archived).CountAsync();
                result.RequestHomeDetails.TotalSale = await requests.Where(x => x.Contract == "Vendita" && !x.Closed && !x.Archived).CountAsync();
                result.RequestHomeDetails.TotalRent = await requests.Where(x => x.Contract == "Affitto" && !x.Closed && !x.Archived).CountAsync();

                result.RequestHomeDetails.TotalCreatedPerMonth = await requests
                    .GroupBy(obj => new { obj.CreationDate.Year, obj.CreationDate.Month })
                    .Select(g => new
                    {
                        Key = g.Key.Month + "/" + g.Key.Year,
                        Count = g.Count()
                    })
                    .ToDictionaryAsync(g => g.Key, g => g.Count);

                result.RequestHomeDetails.MaxAnnual = result.RequestHomeDetails.TotalCreatedPerMonth.Values.Any() ?
                    result.RequestHomeDetails.TotalCreatedPerMonth.Values.Max() : 0;

                result.RequestHomeDetails.MinAnnual = result.RequestHomeDetails.TotalCreatedPerMonth.Values.Any() ?
                    result.RequestHomeDetails.TotalCreatedPerMonth.Values.Min() : 0;

                var saleRequests = await requests.Where(x => x.Contract == "Vendita")
                    .Select(x => new { x.Town, x.PropertyType }).ToListAsync();

                foreach (var item in saleRequests)
                {
                    string[] towns = item.Town.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    foreach (var town in towns)
                    {
                        if (!result.RequestHomeDetails.DistinctByTownSale.ContainsKey(town))
                        {
                            result.RequestHomeDetails.DistinctByTownSale[town] = 1;
                        }
                        else
                        {
                            result.RequestHomeDetails.DistinctByTownSale[town]++;
                        }
                    }

                    if (!result.RequestHomeDetails.DistinctByTypeSale.ContainsKey(item.PropertyType))
                    {
                        result.RequestHomeDetails.DistinctByTypeSale[item.PropertyType] = 1;
                    }
                    else
                    {
                        result.RequestHomeDetails.DistinctByTypeSale[item.PropertyType]++;
                    }
                }

                var rentRequests = await requests.Where(x => x.Contract == "Affitto")
                    .Select(x => new { x.Town, x.PropertyType }).ToListAsync();

                foreach (var item in rentRequests)
                {
                    string[] towns = item.Town.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    foreach (var town in towns)
                    {
                        if (!result.RequestHomeDetails.DistinctByTownRent.ContainsKey(town))
                        {
                            result.RequestHomeDetails.DistinctByTownRent[town] = 1;
                        }
                        else
                        {
                            result.RequestHomeDetails.DistinctByTownRent[town]++;
                        }
                    }

                    if (!result.RequestHomeDetails.DistinctByTypeRent.ContainsKey(item.PropertyType))
                    {
                        result.RequestHomeDetails.DistinctByTypeRent[item.PropertyType] = 1;
                    }
                    else
                    {
                        result.RequestHomeDetails.DistinctByTypeRent[item.PropertyType]++;
                    }
                }

                // Riepiloghi agenzia
                IQueryable<Customer> customers = _unitOfWork.dbContext.Customers.AsQueryable();
                if (!string.IsNullOrEmpty(scopeAgencyId))
                {
                    customers = customers.Where(x => x.AgencyId == scopeAgencyId);
                }

                result.TotalCustomers = await customers.CountAsync();

                var agents = await userManager.GetUsersInRoleAsync("Agent");
                if (!string.IsNullOrEmpty(scopeAgencyId))
                {
                    agents = agents.Where(x => x.AgencyId == scopeAgencyId).ToList();
                }
                result.TotalAgents = agents.Count;

                if (!string.IsNullOrEmpty(scopeAgencyId) || isAdmin || isAgency)
                {
                    result.AgencySummary = new AgencySummary
                    {
                        TotalCustomers = result.TotalCustomers,
                        TotalAgents = result.TotalAgents
                    };
                }

                // Calendario
                IQueryable<Calendar> calendars = _unitOfWork.dbContext.Calendars
                    .Include(x => x.ApplicationUser)
                    .Include(x => x.RealEstateProperty)
                    .AsQueryable();
                if (isAgent)
                {
                    calendars = calendars.Where(x => x.ApplicationUserId == currentUser.Id);
                }
                else if (!string.IsNullOrEmpty(scopeAgencyId))
                {
                    calendars = calendars.Where(x => x.ApplicationUser.AgencyId == scopeAgencyId);
                }

                DateTime today = now.Date;
                DateTime weekStart = today.AddDays(-(int)today.DayOfWeek);

                result.CalendarDetails.Total = await calendars.CountAsync();
                result.CalendarDetails.Today = await calendars.Where(x => x.DataInizioEvento.Date == today && !x.Cancelled).CountAsync();
                result.CalendarDetails.ThisWeek = await calendars.Where(x => x.DataInizioEvento >= weekStart && x.DataInizioEvento < weekStart.AddDays(7) && !x.Cancelled).CountAsync();
                result.CalendarDetails.ThisMonth = await calendars.Where(x => x.DataInizioEvento >= monthStart && !x.Cancelled).CountAsync();
                result.CalendarDetails.Confirmed = await calendars.Where(x => x.Confirmed).CountAsync();
                result.CalendarDetails.Cancelled = await calendars.Where(x => x.Cancelled).CountAsync();
                result.CalendarDetails.Postponed = await calendars.Where(x => x.Postponed).CountAsync();
                result.CalendarDetails.Upcoming = await calendars.Where(x => x.DataInizioEvento >= now && !x.Cancelled).CountAsync();
                result.CalendarDetails.LinkedToProperties = await calendars.Where(x => x.RealEstatePropertyId != null).CountAsync();
                result.CalendarDetails.LinkedToRequests = await calendars.Where(x => x.RequestId != null).CountAsync();

                // Per tipo
                result.CalendarDetails.ByType = await calendars
                    .Where(x => !string.IsNullOrEmpty(x.Type))
                    .GroupBy(x => x.Type)
                    .Select(g => new { g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Key, x => x.Count);

                // Per città/luogo
                result.CalendarDetails.ByLocation = await calendars
                    .Where(x => !string.IsNullOrEmpty(x.LuogoEvento))
                    .GroupBy(x => x.LuogoEvento)
                    .Select(g => new { g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Key, x => x.Count);

                // Per agente
                result.CalendarDetails.ByAgent = await calendars
                    .GroupBy(x => x.ApplicationUser.Name + " " + x.ApplicationUser.LastName)
                    .Select(g => new { g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Key, x => x.Count);

                // Per giorno della settimana
                var dayNames = new[] { "Domenica", "Lunedì", "Martedì", "Mercoledì", "Giovedì", "Venerdì", "Sabato" };
                var byDayOfWeek = await calendars
                    .Select(x => new { DayOfWeek = (int)x.DataInizioEvento.DayOfWeek })
                    .ToListAsync();
                result.CalendarDetails.ByDayOfWeek = byDayOfWeek
                    .GroupBy(x => dayNames[x.DayOfWeek])
                    .ToDictionary(g => g.Key, g => g.Count());

                // Trend mensile
                result.CalendarDetails.CreatedPerMonth = await calendars
                    .GroupBy(obj => new { obj.CreationDate.Year, obj.CreationDate.Month })
                    .Select(g => new
                    {
                        Key = g.Key.Month + "/" + g.Key.Year,
                        Count = g.Count()
                    })
                    .ToDictionaryAsync(g => g.Key, g => g.Count);

                // Appuntamenti (ultimi 30 giorni + prossimi 30 giorni, max 50)
                var startDate = now.AddDays(-30);
                var endDate = now.AddDays(30);
                var allAppointments = await calendars
                    .Where(x => x.DataInizioEvento >= startDate && x.DataInizioEvento <= endDate)
                    .Select(x => new UpcomingAppointment
                    {
                        Id = x.Id,
                        NomeEvento = x.NomeEvento,
                        Type = x.Type,
                        LuogoEvento = x.LuogoEvento,
                        DataInizioEvento = x.DataInizioEvento,
                        DataFineEvento = x.DataFineEvento,
                        Confirmed = x.Confirmed,
                        Cancelled = x.Cancelled,
                        AgentName = x.ApplicationUser.Name + " " + x.ApplicationUser.LastName,
                        PropertyTitle = x.RealEstateProperty != null ? x.RealEstateProperty.Title : null
                    })
                    .ToListAsync();
                
                // Ordina: prima i futuri (crescente), poi i passati (decrescente)
                var upcomingAppointments = allAppointments
                    .OrderByDescending(x => x.DataInizioEvento >= now)
                    .ThenBy(x => x.DataInizioEvento >= now ? x.DataInizioEvento : DateTime.MaxValue)
                    .ThenByDescending(x => x.DataInizioEvento < now ? x.DataInizioEvento : DateTime.MinValue)
                    .Take(50)
                    .ToList();
                
                result.CalendarDetails.UpcomingAppointments = upcomingAppointments;

                // Vendite
                IQueryable<RealEstateProperty> soldProperties = properties.Where(x => x.Sold);
                if (isAgent)
                {
                    soldProperties = soldProperties.Where(x => x.AgentId == currentUser.Id);
                }

                result.SalesDetails.TotalSold = await soldProperties.CountAsync();
                result.SalesDetails.SoldThisMonth = await soldProperties.Where(x => x.CreationDate >= monthStart).CountAsync();
                result.SalesDetails.SoldLastMonth = await soldProperties.Where(x => x.CreationDate >= lastMonth && x.CreationDate < monthStart).CountAsync();

                var soldWithPrices = await soldProperties
                    .Select(x => new { x.Price, x.AgreedCommission, x.CreationDate })
                    .ToListAsync();

                result.SalesDetails.TotalSalesValue = soldWithPrices.Sum(x => x.Price);
                result.SalesDetails.TotalSalesValueThisMonth = soldWithPrices
                    .Where(x => x.CreationDate >= monthStart)
                    .Sum(x => x.Price);
                result.SalesDetails.TotalCommissions = soldWithPrices.Sum(x => x.AgreedCommission);
                result.SalesDetails.TotalCommissionsThisMonth = soldWithPrices
                    .Where(x => x.CreationDate >= monthStart)
                    .Sum(x => x.AgreedCommission);

                result.SalesDetails.SoldPerMonth = await soldProperties
                    .GroupBy(obj => new { obj.CreationDate.Year, obj.CreationDate.Month })
                    .Select(g => new
                    {
                        Key = g.Key.Month + "/" + g.Key.Year,
                        Count = g.Count()
                    })
                    .ToDictionaryAsync(g => g.Key, g => g.Count);

                var salesValuePerMonth = await soldProperties
                    .GroupBy(obj => new { obj.CreationDate.Year, obj.CreationDate.Month })
                    .Select(g => new
                    {
                        Key = g.Key.Month + "/" + g.Key.Year,
                        TotalValue = g.Sum(x => x.Price)
                    })
                    .ToListAsync();

                result.SalesDetails.SalesValuePerMonth = salesValuePerMonth.ToDictionary(x => x.Key, x => x.TotalValue);

                // Dettagli per agente
                if (isAgent)
                {
                    IQueryable<RealEstateProperty> agentProperties = _unitOfWork.dbContext.RealEstateProperties.Where(x => x.AgentId == currentUser.Id);

                    result.AgentSelf = new AgentSelfDetails
                    {
                        PropertiesTotal = await agentProperties.CountAsync(),
                        PropertiesActive = await agentProperties.Where(x => !x.Archived && !x.Sold).CountAsync(),
                        PropertiesArchivedOrSold = await agentProperties.Where(x => x.Archived || x.Sold).CountAsync(),
                        RequestsTotal = await requests.CountAsync(),
                        RequestsActive = await requests.Where(x => !x.Closed && !x.Archived).CountAsync()
                    };
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Si è verificato un errore");
            }
        }

        private (DateTime? Start, DateTime? End) CalculatePeriodFilter(string? period, DateTime now)
        {
            if (string.IsNullOrEmpty(period) || period == "all")
                return (null, null);

            DateTime today = now.Date;
            DateTime monthStart = new DateTime(now.Year, now.Month, 1);

            return period switch
            {
                "all" => (null, null),
                "today" => (today, today.AddDays(1)),
                "yesterday" => (today.AddDays(-1), today),
                "lastWeek" => (today.AddDays(-(int)today.DayOfWeek).AddDays(-7), today.AddDays(-(int)today.DayOfWeek)),
                "week" => (today.AddDays(-(int)today.DayOfWeek), today.AddDays(-(int)today.DayOfWeek).AddDays(7)),
                "month" => (monthStart, monthStart.AddMonths(1)),
                "lastMonth" => (monthStart.AddMonths(-1), monthStart),
                "last3Months" => (monthStart.AddMonths(-3), monthStart),
                "last6Months" => (monthStart.AddMonths(-6), monthStart),
                "lastYear" => (monthStart.AddYears(-1), monthStart),
                _ => (null, null)
            };
        }

        private string CalculateScopeAgencyId(ApplicationUser currentUser, bool isAdmin, bool isAgency, bool isAgent, string? agencyId)
        {
            if (isAdmin && !string.IsNullOrWhiteSpace(agencyId))
                return agencyId;
            if (isAgency)
                return currentUser.Id;
            if (isAgent)
                return currentUser.AgencyId ?? string.Empty;
            return string.Empty;
        }

        private IQueryable<RealEstateProperty> GetFilteredPropertiesQuery(string scopeAgencyId, bool isAgent, string currentUserId)
        {
            IQueryable<RealEstateProperty> query = _unitOfWork.dbContext.RealEstateProperties.AsQueryable();
            
            if (!string.IsNullOrEmpty(scopeAgencyId))
                query = query.Where(x => x.Agent.AgencyId == scopeAgencyId);
            
            if (isAgent)
                query = query.Where(x => x.AgentId == currentUserId);
            
            return query;
        }

        private IQueryable<Request> GetFilteredRequestsQuery(string scopeAgencyId)
        {
            IQueryable<Request> query = _unitOfWork.dbContext.Requests.AsQueryable();
            
            if (!string.IsNullOrEmpty(scopeAgencyId))
                query = query.Where(x => x.AgencyId == scopeAgencyId);
            
            return query;
        }

        private async Task<RealEstatePropertyHomeDetails> CalculatePropertyStatistics(
            IQueryable<RealEstateProperty> properties, 
            DateTime? periodStart, 
            DateTime? periodEnd,
            DateTime lastMonth,
            DateTime monthStart)
        {
            var result = new RealEstatePropertyHomeDetails();

            // Applica filtro periodo se specificato
            if (periodStart.HasValue && periodEnd.HasValue)
            {
                properties = properties.Where(x => x.CreationDate >= periodStart.Value && x.CreationDate < periodEnd.Value);
            }

            // Esegui query sequenzialmente per evitare problemi di concorrenza con DbContext
            var saleProperties = properties.Where(x => x.Status == "Vendita");
            var rentProperties = properties.Where(x => x.Status == "Affitto");

            result.Total = await properties.CountAsync();
            result.TotalSale = await saleProperties.CountAsync();
            result.TotalRent = await rentProperties.CountAsync();
            result.TotalLastMonth = await properties.Where(x => x.CreationDate >= lastMonth).CountAsync();
            result.TotalCurrentMonth = await properties.Where(x => x.CreationDate >= monthStart).CountAsync();

            result.DistinctByTownSale = await saleProperties.GroupBy(x => x.Town).Select(g => new { g.Key, Count = g.Count() }).ToDictionaryAsync(x => x.Key, x => x.Count);
            result.DistinctByTownRent = await rentProperties.GroupBy(x => x.Town).Select(g => new { g.Key, Count = g.Count() }).ToDictionaryAsync(x => x.Key, x => x.Count);
            result.DistinctByTypeSale = await saleProperties.GroupBy(x => x.Typology ?? x.Category).Select(g => new { g.Key, Count = g.Count() }).ToDictionaryAsync(x => x.Key, x => x.Count);
            result.DistinctByTypeRent = await rentProperties.GroupBy(x => x.Typology ?? x.Category).Select(g => new { g.Key, Count = g.Count() }).ToDictionaryAsync(x => x.Key, x => x.Count);
            result.TotalCreatedPerMonth = await properties.GroupBy(obj => new { obj.CreationDate.Year, obj.CreationDate.Month })
                .Select(g => new { Key = g.Key.Month + "/" + g.Key.Year, Count = g.Count() })
                .ToDictionaryAsync(g => g.Key, g => g.Count);

            result.MaxAnnual = result.TotalCreatedPerMonth.Values.Any() ? result.TotalCreatedPerMonth.Values.Max() : 0;
            result.MinAnnual = result.TotalCreatedPerMonth.Values.Any() ? result.TotalCreatedPerMonth.Values.Min() : 0;

            return result;
        }

        private async Task<RequestHomeDetails> CalculateRequestStatistics(
            IQueryable<Request> requests,
            DateTime? periodStart,
            DateTime? periodEnd,
            DateTime lastMonth)
        {
            var result = new RequestHomeDetails();

            // Applica filtro periodo se specificato
            if (periodStart.HasValue && periodEnd.HasValue)
            {
                requests = requests.Where(x => x.CreationDate >= periodStart.Value && x.CreationDate < periodEnd.Value);
            }

            // Query base ottimizzate
            var activeRequests = requests.Where(x => !x.Closed && !x.Archived);
            var saleRequests = requests.Where(x => x.Contract == "Vendita");
            var rentRequests = requests.Where(x => x.Contract == "Affitto");

            // Esegui conteggi sequenzialmente per evitare problemi di concorrenza con DbContext
            result.Total = await requests.CountAsync();
            result.TotalActive = await activeRequests.CountAsync();
            result.TotalArchived = await requests.Where(x => x.Archived).CountAsync();
            result.TotalClosed = await requests.Where(x => x.Closed).CountAsync();
            result.TotalLastMonth = await requests.Where(x => x.CreationDate >= lastMonth && !x.Closed && !x.Archived).CountAsync();
            result.TotalSale = await saleRequests.Where(x => !x.Closed && !x.Archived).CountAsync();
            result.TotalRent = await rentRequests.Where(x => !x.Closed && !x.Archived).CountAsync();

            result.TotalCreatedPerMonth = await requests
                .GroupBy(obj => new { obj.CreationDate.Year, obj.CreationDate.Month })
                .Select(g => new { Key = g.Key.Month + "/" + g.Key.Year, Count = g.Count() })
                .ToDictionaryAsync(g => g.Key, g => g.Count);
            result.MaxAnnual = result.TotalCreatedPerMonth.Values.Any() ? result.TotalCreatedPerMonth.Values.Max() : 0;
            result.MinAnnual = result.TotalCreatedPerMonth.Values.Any() ? result.TotalCreatedPerMonth.Values.Min() : 0;

            // Calcola DistinctByTown e DistinctByType in modo ottimizzato
            var saleRequestsData = await saleRequests.Select(x => new { Town = x.Town ?? string.Empty, PropertyType = x.PropertyType ?? string.Empty }).ToListAsync();
            var rentRequestsData = await rentRequests.Select(x => new { Town = x.Town ?? string.Empty, PropertyType = x.PropertyType ?? string.Empty }).ToListAsync();

            ProcessRequestTownsAndTypes(
                saleRequestsData.Select(x => (x.Town, x.PropertyType)).ToList(),
                result.DistinctByTownSale,
                result.DistinctByTypeSale);
            ProcessRequestTownsAndTypes(
                rentRequestsData.Select(x => (x.Town, x.PropertyType)).ToList(),
                result.DistinctByTownRent,
                result.DistinctByTypeRent);

            return result;
        }

        private void ProcessRequestTownsAndTypes(
            List<(string Town, string PropertyType)> requestsData,
            IDictionary<string, int> townDict,
            IDictionary<string, int> typeDict)
        {
            foreach (var item in requestsData)
            {
                if (!string.IsNullOrEmpty(item.Town))
                {
                    string[] towns = item.Town.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    foreach (var town in towns)
                    {
                        if (!townDict.ContainsKey(town))
                            townDict[town] = 0;
                        townDict[town]++;
                    }
                }

                if (!string.IsNullOrEmpty(item.PropertyType))
                {
                    if (!typeDict.ContainsKey(item.PropertyType))
                        typeDict[item.PropertyType] = 0;
                    typeDict[item.PropertyType]++;
                }
            }
        }

        private async Task<AgentStatsDetails> CalculateAgentStats(
            IQueryable<RealEstateProperty> agentPropertiesQuery,
            IQueryable<Request> requests,
            IQueryable<Calendar> calendarsForStats,
            DateTime now,
            DateTime monthStart)
        {
            var result = new AgentStatsDetails();

            // Esegui query sequenzialmente per evitare problemi di concorrenza con DbContext
            result.TotalPropertiesManaged = await agentPropertiesQuery.CountAsync();
            result.ActivePropertiesManaged = await agentPropertiesQuery.Where(x => !x.Archived && !x.Sold).CountAsync();
            
            // Calcola acquisizioni dagli eventi del calendario con tipo "Acquisizione"
            // Acquisizioni valide: eventi passati con tipo "Acquisizione" che NON sono disdetti o rimandati
            result.TotalAcquisitions = await calendarsForStats
                .Where(x => x.Type == "Acquisizione" 
                    && x.DataInizioEvento < now 
                    && !x.Cancelled 
                    && !x.Postponed)
                .CountAsync();
            
            // Acquisizioni di questo mese: eventi passati con tipo "Acquisizione" che NON sono disdetti o rimandati e che sono nel mese corrente
            result.AcquisitionsThisMonth = await calendarsForStats
                .Where(x => x.Type == "Acquisizione" 
                    && x.DataInizioEvento < now 
                    && x.DataInizioEvento >= monthStart
                    && !x.Cancelled 
                    && !x.Postponed)
                .CountAsync();
            
            result.TotalAppointments = await calendarsForStats.CountAsync();
            result.AppointmentsDisdetti = await calendarsForStats.Where(x => x.Cancelled).CountAsync();
            result.AppointmentsConfermati = await calendarsForStats.Where(x => x.Confirmed && !x.Cancelled).CountAsync();

            return result;
        }

        private async Task<List<AgentDetail>> CalculateAgentDetails(
            IQueryable<Calendar> allCalendarsForDetails,
            List<ApplicationUser> agentsList,
            IQueryable<Request> requests,
            bool isAgent,
            ApplicationUser currentUser,
            DateTime now)
        {
            var calendarsList = await allCalendarsForDetails.ToListAsync();
            
            var agentDetailsMap = calendarsList
                .GroupBy(x => x.ApplicationUser.Name + " " + x.ApplicationUser.LastName)
                .Select(g => new AgentDetail
                {
                    Name = g.Key,
                    AppointmentsDisdetti = g.Count(x => x.Cancelled),
                    AppointmentsConfermati = g.Count(x => x.Confirmed && !x.Cancelled),
                    AppointmentsEffettuati = g.Count(x => x.DataInizioEvento < now && !x.Cancelled && !x.Postponed),
                    TotalAppointments = g.Count(),
                    // Calcola acquisizioni per questo agente: eventi passati con tipo "Acquisizione" che NON sono disdetti o rimandati
                    Acquisitions = g.Count(x => x.Type == "Acquisizione" 
                        && x.DataInizioEvento < now 
                        && !x.Cancelled 
                        && !x.Postponed)
                })
                .ToList();

            // Se è un agente e non ci sono appuntamenti, crea comunque un dettaglio
            if (isAgent && !agentDetailsMap.Any())
            {
                var currentAgentName = currentUser.Name + " " + currentUser.LastName;
                agentDetailsMap.Add(new AgentDetail
                {
                    Name = currentAgentName,
                    AppointmentsDisdetti = 0,
                    AppointmentsConfermati = 0,
                    AppointmentsEffettuati = 0,
                    TotalAppointments = 0,
                    Acquisitions = 0
                });
            }

            // Calcola immobili gestiti per ogni agente sequenzialmente per evitare problemi di concorrenza con DbContext
            var filteredAgentDetails = new List<AgentDetail>();
            foreach (var agent in agentsList)
            {
                var agentName = agent.Name + " " + agent.LastName;
                var agentDetail = agentDetailsMap.FirstOrDefault(ad => ad.Name == agentName);

                if (agentDetail == null)
                {
                    agentDetail = new AgentDetail
                    {
                        Name = agentName,
                        AppointmentsDisdetti = 0,
                        AppointmentsConfermati = 0,
                        AppointmentsEffettuati = 0,
                        TotalAppointments = 0,
                        Acquisitions = 0
                    };
                }

                var agentProperties = _unitOfWork.dbContext.RealEstateProperties.Where(x => x.AgentId == agent.Id);
                agentDetail.PropertiesManaged = await agentProperties.CountAsync();
                // Le acquisizioni sono già calcolate nel GroupBy sopra, quindi non serve ricalcolarle

                filteredAgentDetails.Add(agentDetail);
            }

            return filteredAgentDetails.OrderByDescending(x => x.TotalAppointments).ToList();
        }

        public async Task<DashboardDataResponse> GetDashboardData(ApplicationUser currentUser, string role, string? agencyId, string? period = null)
        {
            try
            {
                string normalizedRole = role ?? string.Empty;
                bool isAdmin = normalizedRole.Equals("Admin", StringComparison.OrdinalIgnoreCase);
                bool isAgency = normalizedRole.Equals("Agenzia", StringComparison.OrdinalIgnoreCase);
                bool isAgent = normalizedRole.Equals("Agente", StringComparison.OrdinalIgnoreCase);
                DateTime now = DateTime.Now;
                DateTime monthStart = new DateTime(now.Year, now.Month, 1);
                DateTime lastMonth = now.AddMonths(-1);

                // Calcola filtri
                var (periodStart, periodEnd) = CalculatePeriodFilter(period, now);
                string scopeAgencyId = CalculateScopeAgencyId(currentUser, isAdmin, isAgency, isAgent, agencyId);

                // Prepara query base
                var propertiesQuery = GetFilteredPropertiesQuery(scopeAgencyId, isAgent, currentUser.Id);
                var requestsQuery = GetFilteredRequestsQuery(scopeAgencyId);

                var result = new DashboardDataResponse
                {
                    Role = normalizedRole,
                    ScopeAgencyId = scopeAgencyId,
                    ScopeUserId = currentUser.Id
                };

                // Esegui calcoli sequenzialmente per evitare problemi di concorrenza con DbContext
                result.RealEstatePropertyHomeDetails = await CalculatePropertyStatistics(propertiesQuery, periodStart, periodEnd, lastMonth, monthStart);
                result.RequestHomeDetails = await CalculateRequestStatistics(requestsQuery, periodStart, periodEnd, lastMonth);
                result.TotalCustomers = await GetCustomersCount(scopeAgencyId);
                var agentsList = await GetAgentsList(scopeAgencyId, isAgent, currentUser.Id);
                result.TotalAgents = agentsList.Count;

                // AgencySummary
                if (!string.IsNullOrEmpty(scopeAgencyId) || isAdmin || isAgency)
                {
                    result.AgencySummary = new AgencySummary
                    {
                        TotalCustomers = result.TotalCustomers,
                        TotalAgents = result.TotalAgents
                    };
                }

                // Statistiche agenti aggregate (non filtrate per periodo)
                var agentPropertiesQuery = GetFilteredPropertiesQuery(scopeAgencyId, isAgent, currentUser.Id);
                var calendarsForStatsQuery = GetFilteredCalendarsQuery(scopeAgencyId, isAgent, currentUser.Id, periodStart, periodEnd);

                result.AgentStats = await CalculateAgentStats(agentPropertiesQuery, requestsQuery, calendarsForStatsQuery, now, monthStart);
                var agentDetailsCalendarsQuery = GetFilteredCalendarsQuery(scopeAgencyId, isAgent, currentUser.Id, periodStart, periodEnd);
                result.AgentDetails = await CalculateAgentDetails(agentDetailsCalendarsQuery, agentsList, requestsQuery, isAgent, currentUser, now);

                // Immobili caricati (filtrati per periodo)
                var loadedPropertiesQuery = GetFilteredPropertiesQuery(scopeAgencyId, isAgent, currentUser.Id);
                if (periodStart.HasValue && periodEnd.HasValue)
                {
                    loadedPropertiesQuery = loadedPropertiesQuery.Where(x => x.CreationDate >= periodStart.Value && x.CreationDate < periodEnd.Value);
                }
                
                result.LoadedProperties = await loadedPropertiesQuery
                    .OrderByDescending(x => x.CreationDate)
                    .Select(x => new LoadedProperty
                    {
                        Id = x.Id,
                        Title = x.Title,
                        CreationDate = x.CreationDate,
                        Active = !x.Archived && !x.Sold
                    })
                    .Take(100) // Limita a 100 per performance
                    .ToListAsync();

                // AgentSelf per agenti
                if (isAgent)
                {
                    var agentProperties = _unitOfWork.dbContext.RealEstateProperties.Where(x => x.AgentId == currentUser.Id);
                    if (periodStart.HasValue && periodEnd.HasValue)
                    {
                        agentProperties = agentProperties.Where(x => x.CreationDate >= periodStart.Value && x.CreationDate < periodEnd.Value);
                    }

                    // Esegui query sequenzialmente per evitare problemi di concorrenza con DbContext
                    result.AgentSelf = new AgentSelfDetails
                    {
                        PropertiesTotal = await agentProperties.CountAsync(),
                        PropertiesActive = await agentProperties.Where(x => !x.Archived && !x.Sold).CountAsync(),
                        PropertiesArchivedOrSold = await agentProperties.Where(x => x.Archived || x.Sold).CountAsync(),
                        RequestsTotal = await requestsQuery.CountAsync(),
                        RequestsActive = await requestsQuery.Where(x => !x.Closed && !x.Archived).CountAsync()
                    };
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Si è verificato un errore");
            }
        }

        private async Task<int> GetCustomersCount(string scopeAgencyId)
        {
            IQueryable<Customer> customers = _unitOfWork.dbContext.Customers.AsQueryable();
            if (!string.IsNullOrEmpty(scopeAgencyId))
            {
                customers = customers.Where(x => x.AgencyId == scopeAgencyId);
            }
            return await customers.CountAsync();
        }

        private async Task<List<ApplicationUser>> GetAgentsList(string scopeAgencyId, bool isAgent, string currentUserId)
        {
            var agents = await userManager.GetUsersInRoleAsync("Agent");
            if (!string.IsNullOrEmpty(scopeAgencyId))
            {
                agents = agents.Where(x => x.AgencyId == scopeAgencyId).ToList();
            }
            if (isAgent)
            {
                agents = agents.Where(x => x.Id == currentUserId).ToList();
            }
            return agents.ToList();
        }

        private IQueryable<Calendar> GetFilteredCalendarsQuery(string scopeAgencyId, bool isAgent, string currentUserId, DateTime? periodStart, DateTime? periodEnd)
        {
            IQueryable<Calendar> query = _unitOfWork.dbContext.Calendars
                .Include(x => x.ApplicationUser)
                .AsQueryable();

            if (isAgent)
            {
                query = query.Where(x => x.ApplicationUserId == currentUserId);
            }
            else if (!string.IsNullOrEmpty(scopeAgencyId))
            {
                query = query.Where(x => x.ApplicationUser.AgencyId == scopeAgencyId);
            }

            if (periodStart.HasValue && periodEnd.HasValue)
            {
                query = query.Where(x => x.DataInizioEvento >= periodStart.Value && x.DataInizioEvento < periodEnd.Value);
            }

            return query;
        }

        public async Task<CalendarDetails> GetDashboardAppointments(ApplicationUser currentUser, string role, string? agencyId, string? period = null)
        {
            try
            {
                string normalizedRole = role ?? string.Empty;
                bool isAdmin = normalizedRole.Equals("Admin", StringComparison.OrdinalIgnoreCase);
                bool isAgency = normalizedRole.Equals("Agenzia", StringComparison.OrdinalIgnoreCase);
                bool isAgent = normalizedRole.Equals("Agente", StringComparison.OrdinalIgnoreCase);
                DateTime now = DateTime.Now;
                DateTime monthStart = new DateTime(now.Year, now.Month, 1);
                
                // Calcola il filtro periodo
                DateTime? periodStart = null;
                DateTime? periodEnd = null;
                if (!string.IsNullOrEmpty(period) && period != "all")
                {
                    DateTime _today = now.Date;
                    if (period == "today")
                    {
                        periodStart = _today;
                        periodEnd = _today.AddDays(1);
                    }
                    else if (period == "week")
                    {
                        periodStart = _today.AddDays(-(int)_today.DayOfWeek);
                        periodEnd = periodStart.Value.AddDays(7);
                    }
                    else if (period == "month")
                    {
                        periodStart = monthStart;
                        periodEnd = monthStart.AddMonths(1);
                    }
                }

                string scopeAgencyId = string.Empty;
                if (isAdmin && !string.IsNullOrWhiteSpace(agencyId))
                {
                    scopeAgencyId = agencyId;
                }
                else if (isAgency)
                {
                    scopeAgencyId = currentUser.Id;
                }
                else if (isAgent)
                {
                    scopeAgencyId = currentUser.AgencyId ?? string.Empty;
                }

                // Calendario
                IQueryable<Calendar> calendars = _unitOfWork.dbContext.Calendars
                    .Include(x => x.ApplicationUser)
                    .Include(x => x.RealEstateProperty)
                    .AsQueryable();
                if (isAgent)
                {
                    calendars = calendars.Where(x => x.ApplicationUserId == currentUser.Id);
                }
                else if (!string.IsNullOrEmpty(scopeAgencyId))
                {
                    calendars = calendars.Where(x => x.ApplicationUser.AgencyId == scopeAgencyId);
                }

                // Applica filtro periodo agli appuntamenti se specificato
                if (periodStart.HasValue && periodEnd.HasValue)
                {
                    calendars = calendars.Where(x => x.DataInizioEvento >= periodStart.Value && x.DataInizioEvento < periodEnd.Value);
                }

                DateTime today = now.Date;
                DateTime weekStart = today.AddDays(-(int)today.DayOfWeek);

                CalendarDetails result = new CalendarDetails();

                result.Total = await calendars.CountAsync();
                result.Today = await calendars.Where(x => x.DataInizioEvento.Date == today && !x.Cancelled).CountAsync();
                result.ThisWeek = await calendars.Where(x => x.DataInizioEvento >= weekStart && x.DataInizioEvento < weekStart.AddDays(7) && !x.Cancelled).CountAsync();
                result.ThisMonth = await calendars.Where(x => x.DataInizioEvento >= monthStart && !x.Cancelled).CountAsync();
                result.Confirmed = await calendars.Where(x => x.Confirmed).CountAsync();
                result.Cancelled = await calendars.Where(x => x.Cancelled).CountAsync();
                result.Postponed = await calendars.Where(x => x.Postponed).CountAsync();
                result.Upcoming = await calendars.Where(x => x.DataInizioEvento >= now && !x.Cancelled).CountAsync();
                result.LinkedToProperties = await calendars.Where(x => x.RealEstatePropertyId != null).CountAsync();
                result.LinkedToRequests = await calendars.Where(x => x.RequestId != null).CountAsync();

                // Per tipo
                result.ByType = await calendars
                    .Where(x => !string.IsNullOrEmpty(x.Type))
                    .GroupBy(x => x.Type)
                    .Select(g => new { g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Key, x => x.Count);

                // Per città/luogo
                result.ByLocation = await calendars
                    .Where(x => !string.IsNullOrEmpty(x.LuogoEvento))
                    .GroupBy(x => x.LuogoEvento)
                    .Select(g => new { g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Key, x => x.Count);

                // Per agente
                result.ByAgent = await calendars
                    .GroupBy(x => x.ApplicationUser.Name + " " + x.ApplicationUser.LastName)
                    .Select(g => new { g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Key, x => x.Count);

                // Per giorno della settimana
                var dayNames = new[] { "Domenica", "Lunedì", "Martedì", "Mercoledì", "Giovedì", "Venerdì", "Sabato" };
                var byDayOfWeek = await calendars
                    .Select(x => new { DayOfWeek = (int)x.DataInizioEvento.DayOfWeek })
                    .ToListAsync();
                result.ByDayOfWeek = byDayOfWeek
                    .GroupBy(x => dayNames[x.DayOfWeek])
                    .ToDictionary(g => g.Key, g => g.Count());

                // Trend mensile
                result.CreatedPerMonth = await calendars
                    .GroupBy(obj => new { obj.CreationDate.Year, obj.CreationDate.Month })
                    .Select(g => new
                    {
                        Key = g.Key.Month + "/" + g.Key.Year,
                        Count = g.Count()
                    })
                    .ToDictionaryAsync(g => g.Key, g => g.Count);

                // Appuntamenti - se periodo specificato usa quello, altrimenti tutti gli appuntamenti
                IQueryable<Calendar> appointmentsQuery = calendars;
                if (periodStart.HasValue && periodEnd.HasValue)
                {
                    appointmentsQuery = appointmentsQuery.Where(x => x.DataInizioEvento >= periodStart.Value && x.DataInizioEvento < periodEnd.Value);
                }
                
                var allAppointments = await appointmentsQuery
                    .Select(x => new UpcomingAppointment
                    {
                        Id = x.Id,
                        NomeEvento = x.NomeEvento,
                        Type = x.Type,
                        LuogoEvento = x.LuogoEvento,
                        DataInizioEvento = x.DataInizioEvento,
                        DataFineEvento = x.DataFineEvento,
                        Confirmed = x.Confirmed,
                        Cancelled = x.Cancelled,
                        Postponed = x.Postponed,
                        AgentName = x.ApplicationUser.Name + " " + x.ApplicationUser.LastName,
                        PropertyTitle = x.RealEstateProperty != null ? x.RealEstateProperty.Title : null
                    })
                    .ToListAsync();
                
                // Ordina: prima i futuri (crescente), poi i passati (decrescente)
                // Limita solo se non è periodo "all" o se ci sono troppi risultati
                var upcomingAppointments = allAppointments
                    .OrderByDescending(x => x.DataInizioEvento >= now)
                    .ThenBy(x => x.DataInizioEvento >= now ? x.DataInizioEvento : DateTime.MaxValue)
                    .ThenByDescending(x => x.DataInizioEvento < now ? x.DataInizioEvento : DateTime.MinValue)
                    .ToList();
                
                // Se periodo è "all" o non specificato, restituisci tutti, altrimenti limita a 50
                if (!periodStart.HasValue || !periodEnd.HasValue)
                {
                    result.UpcomingAppointments = upcomingAppointments;
                }
                else
                {
                    result.UpcomingAppointments = upcomingAppointments.Take(50).ToList();
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Si è verificato un errore");
            }
        }

        public async Task<List<LocationSelectModel>> GetLocations()
        {
            try
            {
                return await _locationServices.GetAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Si è verificato un errore nel recupero delle località");
            }
        }
    }
}
