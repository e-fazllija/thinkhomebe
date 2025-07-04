﻿using BackEnd.Models.CalendarModels;
using BackEnd.Models.OutputModels;
using BackEnd.Models.RealEstatePropertyModels;

namespace BackEnd.Interfaces.IBusinessServices
{
    public interface IRealEstatePropertyServices
    {
        Task<RealEstatePropertySelectModel> Create(RealEstatePropertyCreateModel dto);
        Task InsertFiles(UploadFilesModel dto);
        Task<ListViewModel<RealEstatePropertySelectModel>> Get(int currentPage, string? filterRequest, string? status, string? typologie, string? location, int? code, int? from, int? to, string? agencyId, char? fromName, char? toName);
        Task<ListViewModel<RealEstatePropertySelectModel>> Get(int currentPage, string? agencyId, string? filterRequest, string? contract, int? priceFrom, int? priceTo, string? category, string? typologie, string? town);
        public int GetPropertyCount();
        Task<RealEstatePropertyCreateViewModel> GetToInsert(string? agencyId);
        Task<RealEstatePropertySelectModel> Update(RealEstatePropertyUpdateModel dto);
        Task<RealEstatePropertySelectModel> GetById(int id);
        Task Delete(int id);
        Task SetHighlighted(int realEstatePropertyId);
        Task SetInHome(int realEstatePropertyId);
        Task<CalendarSearchModel> GetSearchItems(string userId, string? agencyId);
    }
}
