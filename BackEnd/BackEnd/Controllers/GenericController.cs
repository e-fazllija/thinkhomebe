using BackEnd.Interfaces;
using BackEnd.Interfaces.IBusinessServices;
using BackEnd.Models.InputModels;
using BackEnd.Models.MailModels;
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
        private readonly IMailService _mailService;
        private readonly IRealEstatePropertyPhotoServices _realEstatePropertyPhotoServices;
        private readonly ILogger<GenericController> _logger;

        public GenericController(
           IConfiguration configuration,
           IGenericService genericService,
           IMailService mailService,
           IRealEstatePropertyPhotoServices realEstatePropertyPhotoServices,
            ILogger<GenericController> logger)
        {
            _configuration = configuration;
            _genericService = genericService;
            _mailService = mailService;
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

        [HttpGet]
        [Route(nameof(GetAdminHomeDetails))]
        public async Task<IActionResult> GetAdminHomeDetails()
        {
            try
            {
                AdminHomeDetailsModel result = await _genericService.GetAdminHomeDetails();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route(nameof(SendEvaluationRequest))]
        public async Task<IActionResult> SendEvaluationRequest([FromBody] SendRequestModel request)
        {
            try
            {
                await _mailService.SendEvaluationRequestAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route(nameof(SendRequest))]
        public async Task<IActionResult> SendRequest([FromBody] SendRequestModel request)
        {
            try
            {
                await _mailService.SendRequestAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }
        
        [HttpPost]
        [Route(nameof(WorkWithUs))]
        public async Task<IActionResult> WorkWithUs([FromBody] SendRequestModel request)
        {
            try
            {
                await _mailService.SendWorkWithUsRequestAsync(request);
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
