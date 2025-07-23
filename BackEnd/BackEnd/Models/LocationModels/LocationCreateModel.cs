using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.LocationModels
{
    public class LocationCreateModel
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Province { get; set; } = string.Empty;
        
        public bool IsActive { get; set; } = true;
        
        public int OrderIndex { get; set; } = 0;
    }
} 