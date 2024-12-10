using AutoMapper;
using BackEnd.Entities;
using BackEnd.Interfaces;
using BackEnd.Models.Options;
using BackEnd.Models.OutputModels;
using BackEnd.Models.RealEstatePropertyModels;
using BackEnd.Models.RealEstatePropertyPhotoModels;
using BackEnd.Services.BusinessServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Utilities;

namespace BackEnd.Services
{
    public class GenericService: IGenericService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GenericService> _logger;
        private readonly IOptionsMonitor<PaginationOptions> options;
        private readonly UserManager<ApplicationUser> userManager;
        public GenericService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GenericService> logger, IOptionsMonitor<PaginationOptions> options, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            this.options = options;
            this.userManager = userManager;
        }

        public async Task<HomeDetailsModel> GetHomeDetails()
        {
            try
            {
                HomeDetailsModel result = new HomeDetailsModel();
                List<RealEstateProperty> propertiesInHome = await _unitOfWork.dbContext.RealEstateProperties.Where(x => x.InHome).Include(x => x.Photos).OrderByDescending(x => x.Id).ToListAsync();

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
        
        public async Task<AdminHomeDetailsModel> GetAdminHomeDetails()
        {
            try
            {
                AdminHomeDetailsModel result = new AdminHomeDetailsModel();
                IQueryable<RealEstateProperty> propertiesInHome = _unitOfWork.dbContext.RealEstateProperties;

                result.RealEstatePropertyHomeDetails.Total = propertiesInHome.Count();
                result.RealEstatePropertyHomeDetails.TotalSale = propertiesInHome.Where(x => x.Status == "Vendita").Count();
                result.RealEstatePropertyHomeDetails.TotalRent = propertiesInHome.Where(x => x.Status == "Affitto").Count();
                result.RealEstatePropertyHomeDetails.TotalLastMonth = propertiesInHome.Where(x => x.CreationDate >= DateTime.Now.AddMonths(-1)).Count();

                // Creazione di un array di 12 elementi inizializzati a 0
                int[] counts = new int[12];

                // Gruppo per mese
                var groupedByMonth = propertiesInHome.ToList()
                    .Where(obj => obj.CreationDate.Year == DateTime.Now.Year) // Considera solo l'anno corrente
                    .GroupBy(obj => obj.CreationDate.Month)
                    .ToDictionary(g => g.Key, g => g.Count());

                // Aggiorna l'array dei conteggi
                for (int i = 1; i <= 12; i++)
                {
                    if (groupedByMonth.ContainsKey(i))
                    {
                        counts[i - 1] = groupedByMonth[i]; // Il mese i corrisponde all'indice i-1
                    }
                }

                result.RealEstatePropertyHomeDetails.TotalCreatedPerMonth = counts;

                result.RealEstatePropertyHomeDetails.MaxAnnual = result.RealEstatePropertyHomeDetails.TotalCreatedPerMonth.Max();
                result.RealEstatePropertyHomeDetails.MinAnnual = result.RealEstatePropertyHomeDetails.TotalCreatedPerMonth.Min();

                IQueryable<Request> request = _unitOfWork.dbContext.Requests;
                result.RequestHomeDetails.Total = request.Count();
                result.RequestHomeDetails.TotalActive = request.Where(x => !x.Closed && !x.Archived).Count();
                result.RequestHomeDetails.TotalArchived = request.Where(x => x.Archived).Count();
                result.RequestHomeDetails.TotalClosed = request.Where(x => x.Closed).Count();
                result.RequestHomeDetails.TotalLastMonth = _unitOfWork.dbContext.Requests.Where(x => x.CreationDate >= DateTime.Now.AddMonths(-1) && !x.Closed && !x.Archived).Count();
                result.RequestHomeDetails.TotalSale = request.Where(x => x.Contract == "Vendita" && !x.Closed && !x.Archived).Count();
                result.RequestHomeDetails.TotalRent = request.Where(x => x.Contract == "Affitto" && !x.Closed && !x.Archived).Count();

                // Creazione di un array di 12 elementi inizializzati a 0
                int[] requestCounts = new int[12];

                // Gruppo per mese
                var requestsGroupedByMonth = request.ToList()
                    .Where(obj => obj.CreationDate.Year == DateTime.Now.Year) // Considera solo l'anno corrente
                    .GroupBy(obj => obj.CreationDate.Month)
                    .ToDictionary(g => g.Key, g => g.Count());

                // Aggiorna l'array dei conteggi
                for (int i = 1; i <= 12; i++)
                {
                    if (requestsGroupedByMonth.ContainsKey(i))
                    {
                        requestCounts[i - 1] = requestsGroupedByMonth[i]; // Il mese i corrisponde all'indice i-1
                    }
                }

                result.RequestHomeDetails.TotalCreatedPerMonth = requestCounts;
                result.RequestHomeDetails.MaxAnnual = result.RequestHomeDetails.TotalCreatedPerMonth.Max();
                result.RequestHomeDetails.MinAnnual = result.RequestHomeDetails.TotalCreatedPerMonth.Min();


                result.TotalCustomers = _unitOfWork.dbContext.Customers.Count();
                result.TotalAgents = userManager.GetUsersInRoleAsync("Agent").Result.Count();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Si è verificato un errore");
            }
        }
    }
}
