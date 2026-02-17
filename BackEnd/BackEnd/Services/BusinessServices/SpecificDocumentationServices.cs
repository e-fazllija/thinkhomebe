using AutoMapper;
using BackEnd.Entities;
using BackEnd.Interfaces;
using BackEnd.Interfaces.IBusinessServices;
using BackEnd.Models.SpecificDocumentationModels;

namespace BackEnd.Services.BusinessServices
{
    public class SpecificDocumentationServices : ISpecificDocumentationServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SpecificDocumentationServices> _logger;
        private readonly IStorageServices _storageServices;

        public SpecificDocumentationServices(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            ILogger<SpecificDocumentationServices> logger,
            IStorageServices storageServices)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _storageServices = storageServices;
        }

        public async Task<SpecificDocumentationSelectModel> Create(SpecificDocumentationCreateModel dto)
        {
            try
            {
                var entity = _mapper.Map<SpecificDocumentation>(dto);
                await _unitOfWork.SpecificDocumentationRepository.InsertAsync(entity);
                await _unitOfWork.SaveAsync();

                var response = _mapper.Map<SpecificDocumentationSelectModel>(entity);
                _logger.LogInformation(nameof(Create));
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Si è verificato un errore in fase creazione");
            }
        }

        public async Task<List<SpecificDocumentationSelectModel>> GetByRealEstatePropertyId(int realEstatePropertyId)
        {
            try
            {
                var documents = await _unitOfWork.SpecificDocumentationRepository.GetByRealEstatePropertyIdAsync(realEstatePropertyId);
                return _mapper.Map<List<SpecificDocumentationSelectModel>>(documents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Si è verificato un errore nel recupero dei documenti");
            }
        }

        public async Task<List<SpecificDocumentationSelectModel>> GetByRealEstatePropertyIdAndType(int realEstatePropertyId, string documentType)
        {
            try
            {
                var documents = await _unitOfWork.SpecificDocumentationRepository.GetByRealEstatePropertyIdAndTypeAsync(realEstatePropertyId, documentType);
                return _mapper.Map<List<SpecificDocumentationSelectModel>>(documents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Si è verificato un errore nel recupero dei documenti");
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                var document = await _unitOfWork.SpecificDocumentationRepository.FirstOrDefaultAsync(
                    filterPredicate: query => query.Where(x => x.Id == id)
                );

                if (document == null)
                    throw new NullReferenceException("Documento non trovato!");

                // Elimina il file dallo storage
                if (!string.IsNullOrEmpty(document.FileName))
                {
                    await _storageServices.DeleteFileFromPrivateContainer(document.FileName);
                }

                _unitOfWork.SpecificDocumentationRepository.Delete(document);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation(nameof(Delete));
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
                    throw new Exception("Si è verificato un errore in fase di eliminazione");
                }
            }
        }
    }
}
