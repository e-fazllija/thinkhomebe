using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.CalendarModels
{
    public class CalendarUpdateModel
    {
        public int Id { get; set; }
        [Required]
        public string NomeEvento { get; set; } = string.Empty;
        public string? DescrizioneEvento { get; set; }
        public string? LuogoEvento { get; set; }
        public DateTime DataInizioEvento { get; set; }
        public DateTime DataFineEvento { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
