using System.ComponentModel.DataAnnotations;

namespace BackEnd.Entities
{
    public class Calendar : EntityBase
    {
        [Required]
        public string NomeEvento { get; set; } = string.Empty;
        public string? DescrizioneEvento { get; set; }
        public string? LuogoEvento { get; set; }
        public DateTime DataInizioEvento { get; set; }
        public DateTime DataFineEvento { get; set; }

         
    }
}
