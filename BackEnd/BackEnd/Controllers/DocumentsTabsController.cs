using Microsoft.AspNetCore.Mvc;
using BackEnd.Entities;
using BackEnd.Interfaces.IBusinessServices;
using BackEnd.Models.CustomerModels;
using BackEnd.Services;
using System.Data;
using BackEnd.Models.ResponseModel;
using BackEnd.Models.OutputModels;
using BackEnd.Services.BusinessServices;
using BackEnd.Models.DocumentsTabModelModels;
using BackEnd.Models.DocumentsTabModels;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("/api/[controller]/")]
    public class DocumentsTabsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDocumentsTabServices _documentsTabServices;
        private readonly ILogger<DocumentsTabsController> _logger;

        public DocumentsTabsController(
           IConfiguration configuration,
           IDocumentsTabServices documentsTabServices,
            ILogger<DocumentsTabsController> logger)
        {
            _configuration = configuration;
            _documentsTabServices = documentsTabServices;
            _logger = logger;
        }
        [HttpPost]
        [Route(nameof(Create))]
        public async Task<IActionResult> Create(DocumentsTabCreateModel request)
        {
            try
            {
                DocumentsTabSelectModel Result = await _documentsTabServices.Create(request);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }
        [HttpPost]
        [Route(nameof(Update))]
        public async Task<IActionResult> Update(DocumentsTabUpdateModel request)
        {
            try
            {
                DocumentsTabSelectModel Result = await _documentsTabServices.Update(request);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }

        [HttpGet]
        [Route(nameof(GetById))]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                DocumentsTabSelectModel result = new DocumentsTabSelectModel();
                result = await _documentsTabServices.GetById(id);

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
                DocumentsTab result = await _documentsTabServices.Delete(id);
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
