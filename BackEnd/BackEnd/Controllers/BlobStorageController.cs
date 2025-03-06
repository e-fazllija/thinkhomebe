﻿using BackEnd.Entities;
using BackEnd.Interfaces;
using BackEnd.Models.InputModels;
using BackEnd.Models.OutputModels;
using BackEnd.Models.ResponseModel;
using BackEnd.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("/api/[controller]/")]
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
                        FileName = Path.GetFileNameWithoutExtension(new Uri(document.FileUrl).AbsolutePath),
                        FileUrl = document.FileUrl

                    }).ToList();

                return Ok(docs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }
    }
}
