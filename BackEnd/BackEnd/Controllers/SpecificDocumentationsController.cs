using Microsoft.AspNetCore.Mvc;
using BackEnd.Interfaces.IBusinessServices;
using BackEnd.Models.ResponseModel;
using BackEnd.Models.InputModels;
using BackEnd.Models.SpecificDocumentationModels;
using Microsoft.AspNetCore.Authorization;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("/api/[controller]/")]
    [Authorize]
    public class SpecificDocumentationsController : ControllerBase
    {
        private readonly ISpecificDocumentationServices _specificDocumentationServices;
        private readonly IDocumentServices _documentServices;
        private readonly ILogger<SpecificDocumentationsController> _logger;

        public SpecificDocumentationsController(
            ISpecificDocumentationServices specificDocumentationServices,
            IDocumentServices documentServices,
            ILogger<SpecificDocumentationsController> logger)
        {
            _specificDocumentationServices = specificDocumentationServices;
            _documentServices = documentServices;
            _logger = logger;
        }

        [HttpPost]
        [Route(nameof(Upload))]
        public async Task<IActionResult> Upload(SendFileModel request, int realEstatePropertyId, string documentType)
        {
            try
            {
                var document = await _documentServices.UploadDocument(request);

                var specificDoc = new SpecificDocumentationCreateModel
                {
                    DocumentType = documentType,
                    FileName = document.FileName,
                    FileUrl = document.FileUrl,
                    RealEstatePropertyId = realEstatePropertyId
                };

                var result = await _specificDocumentationServices.Create(specificDoc);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }

        [HttpGet]
        [Route(nameof(GetByRealEstatePropertyId))]
        public async Task<IActionResult> GetByRealEstatePropertyId(int realEstatePropertyId)
        {
            try
            {
                var result = await _specificDocumentationServices.GetByRealEstatePropertyId(realEstatePropertyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }

        [HttpGet]
        [Route(nameof(GetByRealEstatePropertyIdAndType))]
        public async Task<IActionResult> GetByRealEstatePropertyIdAndType(int realEstatePropertyId, string documentType)
        {
            try
            {
                var result = await _specificDocumentationServices.GetByRealEstatePropertyIdAndType(realEstatePropertyId, documentType);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }

        [HttpDelete]
        [Route(nameof(Delete))]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _specificDocumentationServices.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }
    }
}
