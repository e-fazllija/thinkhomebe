using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.CalendarModels
{
    public class CalendarListModel
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;
        public string AgentName { get; set; } = string.Empty;
        public string AgentLastName { get; set; } = string.Empty;
        public string AgentColor { get; set; } = string.Empty;
        public string NomeEvento { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int? CustomerId { get; set; }
        public int? RealEstatePropertyId { get; set; }
        public int? RequestId { get; set; }
        public string? DescrizioneEvento { get; set; }
        public string? LuogoEvento { get; set; }
        public string? Color { get; set; }
        public DateTime DataInizioEvento { get; set; }
        public DateTime DataFineEvento { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Confirmed { get; set; }
        public bool Cancelled { get; set; }
        public bool Postponed { get; set; }
    }
}

