using BackEnd.Entities;
using BackEnd.Interfaces;
using BackEnd.Models.InputModels;
using BackEnd.Models.OutputModels;
using BackEnd.Models.ResponseModel;
using BackEnd.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("/api/[controller]/")]
    [Authorize]
    public class BlobStorageController : ControllerBase
    {
        private readonly IStorageServices _storageServices;
        private readonly IUnitOfWork _unitOfWork;
        public BlobStorageController(IStorageServices storageServices, IUnitOfWork unitOfWork) 
        {
            _storageServices = storageServices;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Route(nameof(InsertDocument))]
        public async Task<IActionResult> InsertDocument(SendFileModel request)
        {
            try
            {
                Stream stream = request.File.OpenReadStream();
                string fileName = $"{request.FolderName}/{request.File.FileName.Replace(" ", "-")}";
                string fileUrl = await _storageServices.UploadFile(stream, fileName);

                Documentation document = new Documentation()
                {
                    FileName = fileName,
                    FileUrl = fileUrl
                };

                await _unitOfWork.dbContext.Documentation.AddAsync(document);
                _unitOfWork.Save();
                return Ok(document);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }

        [HttpGet]
        [Route(nameof(GetDocuments))]
        public async Task<IActionResult> GetDocuments()
        {
            try
            {
                var documents = await _unitOfWork.dbContext.Documentation.ToListAsync();

                List<DocumentationSelectModel> docs = documents
                    .Select(document => new DocumentationSelectModel
                    {
                        Id = document.Id,
                        FileName = document.FileName,
                        FileUrl = document.FileUrl

                    }).ToList();

                return Ok(docs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }

        [HttpDelete]
        [Route(nameof(DeleteModule))]
        public async Task<IActionResult> DeleteModule(int id)
        {
            try
            {
                Documentation document = await _unitOfWork.dbContext.Documentation.FirstAsync(x => x.Id == id);

                await _storageServices.DeleteFile(document.FileName);

                _unitOfWork.dbContext.Documentation.Remove(document);
                await _unitOfWork.SaveAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route(nameof(UploadPropertyDocument))]
        public async Task<IActionResult> UploadPropertyDocument([FromForm] IFormFile file, [FromForm] int realEstatePropertyId)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new AuthResponseModel() { Status = "Error", Message = "File is empty" });
                }

                Stream stream = file.OpenReadStream();
                string fileName = $"{realEstatePropertyId}/{file.FileName.Replace(" ", "-")}";
                string fileUrl = await _storageServices.UploadFileToPrivateContainer(stream, fileName);

                Documentation document = new Documentation()
                {
                    FileName = fileName,
                    FileUrl = fileUrl,
                    RealEstatePropertyId = realEstatePropertyId
                };

                await _unitOfWork.dbContext.Documentation.AddAsync(document);
                await _unitOfWork.SaveAsync();
                
                return Ok(new DocumentationSelectModel
                {
                    Id = document.Id,
                    FileName = document.FileName,
                    FileUrl = fileUrl
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }

        [HttpGet]
        [Route(nameof(GetPropertyDocuments))]
        public async Task<IActionResult> GetPropertyDocuments(int realEstatePropertyId)
        {
            try
            {
                var documents = await _unitOfWork.dbContext.Documentation
                    .Where(x => x.RealEstatePropertyId == realEstatePropertyId)
                    .ToListAsync();

                List<DocumentationSelectModel> docs = documents
                    .Select(document => new DocumentationSelectModel
                    {
                        Id = document.Id,
                        FileName = document.FileName,
                        FileUrl = document.FileUrl
                    }).ToList();

                return Ok(docs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }

        [HttpGet]
        [Route(nameof(GetDocumentDownloadUrl))]
        public IActionResult GetDocumentDownloadUrl(string fileName)
        {
            try
            {
                string urlWithSas = _storageServices.GenerateSasToken(fileName);
                return Ok(new { url = urlWithSas });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }

        [HttpDelete]
        [Route(nameof(DeletePropertyDocument))]
        public async Task<IActionResult> DeletePropertyDocument(int id)
        {
            try
            {
                Documentation document = await _unitOfWork.dbContext.Documentation.FirstAsync(x => x.Id == id);

                await _storageServices.DeleteFileFromPrivateContainer(document.FileName);

                _unitOfWork.dbContext.Documentation.Remove(document);
                await _unitOfWork.SaveAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }

        [HttpGet]
        [Route(nameof(GetDocumentById))]
        public async Task<IActionResult> GetDocumentById(int id)
        {
            try
            {
                Documentation document = await _unitOfWork.dbContext.Documentation.FirstOrDefaultAsync(x => x.Id == id);
                
                if (document == null)
                {
                    return NotFound(new AuthResponseModel() { Status = "Error", Message = "Documento non trovato" });
                }

                return Ok(new DocumentationSelectModel
                {
                    Id = document.Id,
                    FileName = document.FileName,
                    FileUrl = document.FileUrl
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }
    }
}
