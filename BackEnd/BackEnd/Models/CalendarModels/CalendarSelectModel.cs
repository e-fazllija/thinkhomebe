using BackEnd.Entities;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.CalendarModels
{
    public class CalendarSelectModel
    {
        public int Id { get; set; }
        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;
        public virtual ApplicationUser ApplicationUser { get; set; }
        public string NomeEvento { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int? CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }
        public int? RealEstatePropertyId { get; set; }
        public virtual RealEstateProperty? RealEstateProperty { get; set; }
        public int? RequestId { get; set; }
        public virtual Request? Request { get; set; }
        public string? DescrizioneEvento { get; set; }
        public string? LuogoEvento { get; set; }
        public DateTime DataInizioEvento { get; set; }
        public DateTime DataFineEvento { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}
