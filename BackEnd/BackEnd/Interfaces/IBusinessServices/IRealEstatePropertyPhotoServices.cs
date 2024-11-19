﻿using BackEnd.Entities;
using BackEnd.Models.CustomerModels;
using BackEnd.Models.OutputModels;
using BackEnd.Models.RealEstatePropertyPhotoModels;

namespace BackEnd.Interfaces.IBusinessServices
{
    public interface IRealEstatePropertyPhotoServices
    {
        Task<RealEstatePropertyPhotoSelectModel> Create(RealEstatePropertyPhotoCreateModel dto);
        Task<ListViewModel<RealEstatePropertyPhotoSelectModel>> Get(int currentPage, string? filterRequest, char? fromName, char? toName);
        Task<RealEstatePropertyPhotoSelectModel> Update(RealEstatePropertyPhotoUpdateModel dto);
        Task<RealEstatePropertyPhotoSelectModel> GetById(int id);
        Task<RealEstatePropertyPhoto> Delete(int id);
    }
}
