﻿using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.RealEstatePropertyModels
{
    public class RealEstatePropertyCreateModel
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Category { get; set; } = string.Empty;
        public string? Typology { get; set; }
        public bool InHome { get; set; }
        public bool Highlighted { get; set; }
        public bool Auction { get; set; }
        public bool Sold { get; set; }
        public bool Negotiation { get; set; }
        [Required]
        public string Status { get; set; } = string.Empty;
        [Required]
        public string AddressLine { get; set; } = string.Empty;
        [Required]
        public string Town { get; set; } = string.Empty;
        public string? Location { get; set; }
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
        public double PriceReduced { get; set; }
        public int MQGarden { get; set; }
        public double CondominiumExpenses { get; set; }
        public string? Availability { get; set; }
        [Required]
        public string Description { get; set; } = string.Empty;
        public string? VideoUrl { get; set; }
        public DateTime AssignmentEnd { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public List<IFormFile>? Files { get; set; }
        public int CustomerId { get; set; }
        public string AgentId { get; set; } = string.Empty;
        public string? TypeOfAssignment { get; set; }
        public int AgreedCommission { get; set; }
        public int FlatRateCommission { get; set; }

    }
}
