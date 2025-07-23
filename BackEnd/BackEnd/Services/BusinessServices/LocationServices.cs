using BackEnd.Data;
using BackEnd.Entities;
using BackEnd.Interfaces.IBusinessServices;
using BackEnd.Models.LocationModels;
using BackEnd.Models.OutputModels;
using BackEnd.Services;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services.BusinessServices
{
    public class LocationServices : ILocationServices
    {
        private readonly AppDbContext _context;
        private readonly ILogger<LocationServices> _logger;

        public LocationServices(AppDbContext context, ILogger<LocationServices> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<LocationSelectModel> Create(LocationCreateModel dto)
        {
            try
            {
                var location = new Location
                {
                    Name = dto.Name,
                    City = dto.City,
                    Province = dto.Province,
                    IsActive = dto.IsActive,
                    OrderIndex = dto.OrderIndex,
                    CreationDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                };

                _context.Locations.Add(location);
                await _context.SaveChangesAsync();

                return new LocationSelectModel
                {
                    Id = location.Id,
                    Name = location.Name,
                    City = location.City,
                    Province = location.Province,
                    IsActive = location.IsActive,
                    OrderIndex = location.OrderIndex,
                    CreationDate = location.CreationDate,
                    UpdateDate = location.UpdateDate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating location");
                throw;
            }
        }

        public async Task<LocationSelectModel> Update(LocationUpdateModel dto)
        {
            try
            {
                var location = await _context.Locations.FindAsync(dto.Id);
                if (location == null)
                    throw new Exception("Location not found");

                location.Name = dto.Name;
                location.City = dto.City;
                location.Province = dto.Province;
                location.IsActive = dto.IsActive;
                location.OrderIndex = dto.OrderIndex;
                location.UpdateDate = DateTime.Now;

                await _context.SaveChangesAsync();

                return new LocationSelectModel
                {
                    Id = location.Id,
                    Name = location.Name,
                    City = location.City,
                    Province = location.Province,
                    IsActive = location.IsActive,
                    OrderIndex = location.OrderIndex,
                    CreationDate = location.CreationDate,
                    UpdateDate = location.UpdateDate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating location");
                throw;
            }
        }

        public async Task<LocationSelectModel> GetById(int id)
        {
            try
            {
                var location = await _context.Locations.FindAsync(id);
                if (location == null)
                    throw new Exception("Location not found");

                return new LocationSelectModel
                {
                    Id = location.Id,
                    Name = location.Name,
                    City = location.City,
                    Province = location.Province,
                    IsActive = location.IsActive,
                    OrderIndex = location.OrderIndex,
                    CreationDate = location.CreationDate,
                    UpdateDate = location.UpdateDate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting location by id");
                throw;
            }
        }

        public async Task<ListViewModel<LocationSelectModel>> Get(int currentPage, string? filterRequest, string? city)
        {
            try
            {
                var query = _context.Locations.AsQueryable();

                if (!string.IsNullOrEmpty(filterRequest))
                {
                    query = query.Where(l => l.Name.Contains(filterRequest) || l.City.Contains(filterRequest) || l.Province.Contains(filterRequest));
                }

                if (!string.IsNullOrEmpty(city))
                {
                    query = query.Where(l => l.City == city);
                }

                query = query.OrderBy(l => l.City).ThenBy(l => l.OrderIndex).ThenBy(l => l.Name);

                var totalCount = await query.CountAsync();
                var pageSize = 20;
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var locations = await query
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .Select(l => new LocationSelectModel
                    {
                        Id = l.Id,
                        Name = l.Name,
                        City = l.City,
                        Province = l.Province,
                        IsActive = l.IsActive,
                        OrderIndex = l.OrderIndex,
                        CreationDate = l.CreationDate,
                        UpdateDate = l.UpdateDate
                    })
                    .ToListAsync();

                return new ListViewModel<LocationSelectModel>
                {
                    Data = locations,
                    Total = totalCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting locations");
                throw;
            }
        }

        public async Task<List<LocationSelectModel>> GetAll()
        {
            try
            {
                return await _context.Locations
                    .Where(l => l.IsActive)
                    .OrderBy(l => l.City)
                    .ThenBy(l => l.OrderIndex)
                    .ThenBy(l => l.Name)
                    .Select(l => new LocationSelectModel
                    {
                        Id = l.Id,
                        Name = l.Name,
                        City = l.City,
                        Province = l.Province,
                        IsActive = l.IsActive,
                        OrderIndex = l.OrderIndex,
                        CreationDate = l.CreationDate,
                        UpdateDate = l.UpdateDate
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all locations");
                throw;
            }
        }

        public async Task<List<LocationGroupedModel>> GetGroupedByCity()
        {
            try
            {
                var locations = await _context.Locations
                    .Where(l => l.IsActive)
                    .OrderBy(l => l.City)
                    .ThenBy(l => l.OrderIndex)
                    .ThenBy(l => l.Name)
                    .ToListAsync();

                var grouped = locations
                    .GroupBy(l => l.City)
                    .Select(g => new LocationGroupedModel
                    {
                        City = g.Key,
                        Locations = g.Select(l => new LocationItemModel
                        {
                            Id = l.Name,
                            Name = l.Name
                        }).ToList()
                    })
                    .ToList();

                return grouped;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting grouped locations");
                throw;
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                var location = await _context.Locations.FindAsync(id);
                if (location == null)
                    throw new Exception("Location not found");

                _context.Locations.Remove(location);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting location");
                throw;
            }
        }

        public async Task ToggleActive(int id)
        {
            try
            {
                var location = await _context.Locations.FindAsync(id);
                if (location == null)
                    throw new Exception("Location not found");

                location.IsActive = !location.IsActive;
                location.UpdateDate = DateTime.Now;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling location active status");
                throw;
            }
        }

        public async Task UpdateOrder(List<LocationUpdateModel> locations)
        {
            try
            {
                foreach (var locationDto in locations)
                {
                    var location = await _context.Locations.FindAsync(locationDto.Id);
                    if (location != null)
                    {
                        location.OrderIndex = locationDto.OrderIndex;
                        location.UpdateDate = DateTime.Now;
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating location order");
                throw;
            }
        }
    }
} 