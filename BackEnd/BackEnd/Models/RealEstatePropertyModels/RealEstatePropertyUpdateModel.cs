﻿using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.RealEstatePropertyModels
{
    public class RealEstatePropertyUpdateModel
    {
        public int Id { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;
        [Required]
        public string Typology { get; set; } = string.Empty;
        public bool InHome { get; set; }
        public bool Highlighted { get; set; }
        [Required]
        public string Status { get; set; } = string.Empty;
        [Required]
        public string AddressLine { get; set; } = string.Empty;
        [Required]
        public string Town { get; set; } = string.Empty;
        [Required]
        public string State { get; set; } = string.Empty;
        [Required]
        public string PostCode { get; set; } = string.Empty;
        [Required]
        public int CommercialSurfaceate { get; set; }
        public string? Floor { get; set; }
        [Required]
        public int TotalBuildingfloors { get; set; }
        public int Elevators { get; set; }
        public string? MoreDetails { get; set; }
        public int Bedrooms { get; set; }
        public int WarehouseRooms { get; set; }
        public int Kitchens { get; set; }
        public int Bathrooms { get; set; }
        public string? Furniture { get; set; }
        public string? OtherFeatures { get; set; }
        public int ParkingSpaces { get; set; }
        public string? Heating { get; set; }
        public string? Exposure { get; set; }
        public string? EnergyClass { get; set; }
        public string? TypeOfProperty { get; set; }
        public string? StateOfTheProperty { get; set; }
        public int YearOfConstruction { get; set; }
        [Required]
        public double Price { get; set; }
        public double CondominiumExpenses { get; set; }
        public string? Availability { get; set; }
        public string? Description { get; set; }
        public DateTime UpdateDate { get; set; } = DateTime.Now;
        public int CustomerId { get; set; }
        public string AgentId { get; set; } = string.Empty;
    }
}