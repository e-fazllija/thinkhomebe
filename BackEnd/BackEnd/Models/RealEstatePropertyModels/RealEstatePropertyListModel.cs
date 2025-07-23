using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.RealEstatePropertyModels
{
    public class RealEstatePropertyListModel
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime AssignmentEnd { get; set; }
        public int CommercialSurfaceate { get; set; }
        public string AddressLine { get; set; } = string.Empty;
        public string Town { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? Typology { get; set; }
        public string? StateOfTheProperty { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool Auction { get; set; }
        public string? FirstPhotoUrl { get; set; }
    }
} 