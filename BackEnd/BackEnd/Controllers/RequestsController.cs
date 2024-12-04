﻿using Microsoft.AspNetCore.Mvc;
using BackEnd.Entities;
using BackEnd.Interfaces.IBusinessServices;
using BackEnd.Models.CustomerModels;
using BackEnd.Services;
using System.Data;
using BackEnd.Models.ResponseModel;
using BackEnd.Models.OutputModels;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("/api/[controller]/")]
    public class RequestsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ICustomerServices _requestServices;
        private readonly ILogger<RequestsController> _logger;

        public RequestsController(
           IConfiguration configuration,
           ICustomerServices requestServices,
            ILogger<RequestsController> logger)
        {
            _configuration = configuration;
            _requestServices = requestServices;
            _logger = logger;
        }
        [HttpPost]
        [Route(nameof(Create))]
        public async Task<IActionResult> Create(CustomerCreateModel request)
        {
            try
            {
                CustomerSelectModel Result = await _requestServices.Create(request);
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
        public async Task<IActionResult> Update(CustomerUpdateModel request)
        {
            try
            {
                CustomerSelectModel Result = await _requestServices.Update(request);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }
        [HttpGet]
        [Route(nameof(Get))]
        public async Task<IActionResult> Get(int currentPage, string? filterRequest)
        {
            try
            {
                //currentPage = currentPage > 0 ? currentPage : 1;
                ListViewModel<CustomerSelectModel> res = await _requestServices.Get(currentPage, filterRequest, null, null);

                return Ok(res);
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
                CustomerSelectModel result = new CustomerSelectModel();
                result = await _requestServices.GetById(id);

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
                Customer result = await _requestServices.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }
        [HttpGet]
        [Route(nameof(ExportExcel))]
        public async Task<IActionResult> ExportExcel(char? fromName, char? toName)
        {
            try
            {
                var result = await _requestServices.Get(0, null, fromName, toName);
                DataTable table = Export.ToDataTable<CustomerSelectModel>(result.Data);
                byte[] fileBytes = Export.GenerateExcelContent(table);

                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Output.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }
        [HttpGet]
        [Route(nameof(ExportCsv))]
        public async Task<IActionResult> ExportCsv(char? fromName, char? toName)
        {
            try
            {
                var result = await _requestServices.Get(0, null, fromName, toName);
                DataTable table = Export.ToDataTable<CustomerSelectModel>(result.Data);
                byte[] fileBytes = Export.GenerateCsvContent(table);

                return File(fileBytes, "text/csv", "Output.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }
    }
}