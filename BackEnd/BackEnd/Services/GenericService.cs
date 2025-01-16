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
using System;

namespace BackEnd.Services
{
    public class GenericService : IGenericService
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
                    .ToDictionary(g => g.Key, g => g.Count()); ;

                result.RealEstatePropertyHomeDetails.MaxAnnual = result.RealEstatePropertyHomeDetails.TotalCreatedPerMonth.Values.Max();
                result.RealEstatePropertyHomeDetails.MinAnnual = result.RealEstatePropertyHomeDetails.TotalCreatedPerMonth.Values.Min();

                IQueryable<Request> request = _unitOfWork.dbContext.Requests;
                result.RequestHomeDetails.Total = request.Count();
                result.RequestHomeDetails.TotalActive = request.Where(x => !x.Closed && !x.Archived).Count();
                result.RequestHomeDetails.TotalArchived = request.Where(x => x.Archived).Count();
                result.RequestHomeDetails.TotalClosed = request.Where(x => x.Closed).Count();
                result.RequestHomeDetails.TotalLastMonth = _unitOfWork.dbContext.Requests.Where(x => x.CreationDate >= DateTime.Now.AddMonths(-1) && !x.Closed && !x.Archived).Count();
                result.RequestHomeDetails.TotalSale = request.Where(x => x.Contract == "Vendita" && !x.Closed && !x.Archived).Count();
                result.RequestHomeDetails.TotalRent = request.Where(x => x.Contract == "Affitto" && !x.Closed && !x.Archived).Count();

                result.RequestHomeDetails.TotalCreatedPerMonth = request.ToList()
                    .GroupBy(obj => obj.CreationDate.Month + "/" + obj.CreationDate.Year.ToString())
                    .ToDictionary(g => g.Key, g => g.Count());
                result.RequestHomeDetails.MaxAnnual = result.RequestHomeDetails.TotalCreatedPerMonth.Values.Max();
                result.RequestHomeDetails.MinAnnual = result.RequestHomeDetails.TotalCreatedPerMonth.Values.Min();

                foreach (var item in request.Where(x => x.Contract == "Vendita"))
                {
                    if (!result.RequestHomeDetails.DistinctByTownSale.ContainsKey(item.Town))
                    {
                        result.RequestHomeDetails.DistinctByTownSale[item.Town] = 1;
                    }
                    else
                    {
                        result.RequestHomeDetails.DistinctByTownSale[item.Town]++;
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
                    if (!result.RequestHomeDetails.DistinctByTownRent.ContainsKey(item.Town))
                    {
                        result.RequestHomeDetails.DistinctByTownRent[item.Town] = 1;
                    }
                    else
                    {
                        result.RequestHomeDetails.DistinctByTownRent[item.Town]++;
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
                 
                result.RealEstatePropertyHomeDetails.DistinctByTownSale = result.RealEstatePropertyHomeDetails.DistinctByTownSale.Where(x => x.Value > 10).ToDictionary();
                result.RealEstatePropertyHomeDetails.DistinctByTypeSale = result.RealEstatePropertyHomeDetails.DistinctByTypeSale.Where(x => x.Value > 10).ToDictionary();
                result.RealEstatePropertyHomeDetails.DistinctByTownRent = result.RealEstatePropertyHomeDetails.DistinctByTownRent.Where(x => x.Value > 10).ToDictionary();
                result.RealEstatePropertyHomeDetails.DistinctByTypeRent = result.RealEstatePropertyHomeDetails.DistinctByTypeRent.Where(x => x.Value > 10).ToDictionary();
                result.RequestHomeDetails.DistinctByTownSale = result.RequestHomeDetails.DistinctByTownSale.Where(x => x.Value > 10).ToDictionary();
                result.RequestHomeDetails.DistinctByTypeSale = result.RequestHomeDetails.DistinctByTypeSale.Where(x => x.Value > 10).ToDictionary();
                result.RequestHomeDetails.DistinctByTownRent = result.RequestHomeDetails.DistinctByTownRent.Where(x => x.Value > 10).ToDictionary();
                result.RequestHomeDetails.DistinctByTypeRent = result.RequestHomeDetails.DistinctByTypeRent.Where(x => x.Value > 10).ToDictionary();


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
