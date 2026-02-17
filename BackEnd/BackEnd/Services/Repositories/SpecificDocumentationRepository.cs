using BackEnd.Data;
using BackEnd.Entities;
using BackEnd.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services.Repositories
{
    public class SpecificDocumentationRepository : GenericRepository<SpecificDocumentation>, ISpecificDocumentationRepository
    {
        private readonly AppDbContext _context;

        public SpecificDocumentationRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<SpecificDocumentation>> GetByRealEstatePropertyIdAsync(int realEstatePropertyId)
        {
            return await _context.SpecificDocumentations
                .Where(d => d.RealEstatePropertyId == realEstatePropertyId)
                .OrderBy(d => d.DocumentType)
                .ThenBy(d => d.Id)
                .ToListAsync();
        }

        public async Task<List<SpecificDocumentation>> GetByRealEstatePropertyIdAndTypeAsync(int realEstatePropertyId, string documentType)
        {
            return await _context.SpecificDocumentations
                .Where(d => d.RealEstatePropertyId == realEstatePropertyId && d.DocumentType == documentType)
                .OrderBy(d => d.Id)
                .ToListAsync();
        }
    }
}
