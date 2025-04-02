﻿using BackEnd.Entities;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.RequestModels
{
    public class RequestUpdateModel
    {
        public int Id { get; set; }
        public bool Closed { get; set; }
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public string Contract { get; set; } = string.Empty;
        [Required]
        public string PropertyType { get; set; } = string.Empty;
        [Required]
        public string province { get; set; } = string.Empty;
        [Required]
        public string Town { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? RoomsNumber { get; set; }
        public int MQFrom { get; set; }
        public int MQTo { get; set; }
        public string? PropertyState { get; set; }
        public string? Heating { get; set; }
        public int ParkingSpaces { get; set; }
        [Required]
        public double PriceTo { get; set; }
        public double PriceFrom { get; set; }
        public int GardenFrom { get; set; }
        public int GardenTo { get; set; }
        public string? Notes { get; set; }
        public bool Archived { get; set; }
        public bool MortgageAdviceRequired { get; set; }
        public DateTime UpdateDate { get; set; } = DateTime.Now;
        public string AgencyId { get; set; } = string.Empty;
    }
}
