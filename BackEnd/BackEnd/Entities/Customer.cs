using System.ComponentModel.DataAnnotations;

namespace BackEnd.Entities
{
    public class Customer : EntityBase
    {
        [Required]
        public string Code { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public long Phone { get; set; } 
        public string? Description { get; set; } 
        public string? AdressLine { get; set; } 
        public string? Town { get; set; } 
        public string? State { get; set; } 
        public bool Buyer { get; set; } 
        public bool Seller { get; set; }
        public virtual ICollection<RealEstateProperty> RealEstateProperties { get; set; } = new List<RealEstateProperty>();
    }
}
