using BackEnd.Entities;
using BackEnd.Models.RealEstatePropertyModels;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.RequestModels
{
    public class RequestSelectModel
    {
        public int Id { get; set; }
        public bool Closed { get; set; }
        [Required]
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        [Required]
        public string Contract { get; set; } = string.Empty;
        [Required]
        public string PropertyType { get; set; } = string.Empty;
        [Required]
        public string Province { get; set; } = string.Empty;
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
        public double Price { get; set; }
        public string? Notes { get; set; }
        public bool Archived { get; set; }
        public bool MortgageAdviceRequired { get; set; }
        public List<RealEstatePropertySelectModel>? RealEstateProperties { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public ICollection<RequestNotes>? RequestNotes { get; set; }
    }
}
