using BackEnd.Interfaces;
using BackEnd.Interfaces.IBusinessServices;
using BackEnd.Models.OutputModels;
using BackEnd.Models.ResponseModel;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("/api/[controller]/")]
    public class GenericController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IGenericService _genericService;
        private readonly IRealEstatePropertyPhotoServices _realEstatePropertyPhotoServices;
        private readonly ILogger<GenericController> _logger;

        public GenericController(
           IConfiguration configuration,
           IGenericService genericService,
           IRealEstatePropertyPhotoServices realEstatePropertyPhotoServices,
            ILogger<GenericController> logger)
        {
            _configuration = configuration;
            _genericService = genericService;
            _realEstatePropertyPhotoServices = realEstatePropertyPhotoServices;
            _logger = logger;
        }

        [HttpGet]
        [Route(nameof(GetHomeDetails))]
        public async Task<IActionResult> GetHomeDetails()
        {
            try
            {
                HomeDetailsModel result = await _genericService.GetHomeDetails();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }
    }
}
