using BackEnd.Interfaces;
using BackEnd.Interfaces.IBusinessServices;
using BackEnd.Models.InputModels;
using BackEnd.Models.MailModels;
using BackEnd.Models.OutputModels;
using BackEnd.Models.ResponseModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BackEnd.Entities;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

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
        private readonly UserManager<ApplicationUser> _userManager;

        public GenericController(
           IConfiguration configuration,
           IGenericService genericService,
           IMailService mailService,
           IRealEstatePropertyPhotoServices realEstatePropertyPhotoServices,
            ILogger<GenericController> logger,
            UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _genericService = genericService;
            _mailService = mailService;
            _realEstatePropertyPhotoServices = realEstatePropertyPhotoServices;
            _logger = logger;
            _userManager = userManager;
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
        [Route(nameof(GetDashboard))]
        [Authorize]
        public async Task<IActionResult> GetDashboard(string? agencyId = null)
        {
            try
            {
                var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email || c.Type == JwtRegisteredClaimNames.Email);
                if (emailClaim == null)
                {
                    return Unauthorized();
                }

                var user = await _userManager.FindByEmailAsync(emailClaim.Value);
                if (user == null)
                {
                    return Unauthorized();
                }

                var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                string role = roleClaim?.Value ?? string.Empty;

                var result = await _genericService.GetDashboard(user, role, agencyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }

        [HttpGet]
        [Route(nameof(GetDashboardData))]
        [Authorize]
        public async Task<IActionResult> GetDashboardData(string? agencyId = null, string? period = null)
        {
            try
            {
                var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email || c.Type == JwtRegisteredClaimNames.Email);
                if (emailClaim == null)
                {
                    return Unauthorized();
                }

                var user = await _userManager.FindByEmailAsync(emailClaim.Value);
                if (user == null)
                {
                    return Unauthorized();
                }

                var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                string role = roleClaim?.Value ?? string.Empty;

                var result = await _genericService.GetDashboardData(user, role, agencyId, period);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }

        [HttpGet]
        [Route(nameof(GetDashboardAppointments))]
        [Authorize]
        public async Task<IActionResult> GetDashboardAppointments(string? agencyId = null, string? period = null)
        {
            try
            {
                var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email || c.Type == JwtRegisteredClaimNames.Email);
                if (emailClaim == null)
                {
                    return Unauthorized();
                }

                var user = await _userManager.FindByEmailAsync(emailClaim.Value);
                if (user == null)
                {
                    return Unauthorized();
                }

                var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                string role = roleClaim?.Value ?? string.Empty;

                var result = await _genericService.GetDashboardAppointments(user, role, agencyId, period);
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
        [Authorize]
        public async Task<IActionResult> GetAdminHomeDetails(string agencyId)
        {
            try
            {
                AdminHomeDetailsModel result = await _genericService.GetAdminHomeDetails(agencyId);
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

        [HttpPost]
        [Route(nameof(InformationRequest))]
        public async Task<IActionResult> InformationRequest([FromBody] SendRequestModel request)
        {
            try
            {
                await _mailService.InformationRequestAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route(nameof(SendComplaint))]
        [Authorize]
        public async Task<IActionResult> SendComplaint([FromBody] SegnalazioneRequest request)
        {
            try
            {
                var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email || c.Type == JwtRegisteredClaimNames.Email);
                if (emailClaim == null)
                {
                    return Unauthorized();
                }

                var user = await _userManager.FindByEmailAsync(emailClaim.Value);
                if (user == null)
                {
                    return Unauthorized();
                }

                await _mailService.SendComplaintAsync(request.Message, user.Name, user.LastName, user.Email, user.PhoneNumber?.ToString(), user.MobilePhone?.ToString());
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }

        [HttpGet]
        [Route(nameof(GetLocations))]
        public async Task<IActionResult> GetLocations()
        {
            try
            {
                var result = await _genericService.GetLocations();
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
