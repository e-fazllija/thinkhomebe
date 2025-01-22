﻿using BackEnd.Entities;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.CalendarModels
{
    public class CalendarUpdateModel
    {
        public int Id { get; set; }
        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;
        [Required]
        public string NomeEvento { get; set; } = string.Empty;
        [Required]
        public string Type { get; set; } = string.Empty;
        public int? CustomerId { get; set; }
        public int? RealEstatePropertyId { get; set; }
        public int? RequestId { get; set; }
        public string? DescrizioneEvento { get; set; }
        public string? LuogoEvento { get; set; }
        public DateTime DataInizioEvento { get; set; }
        public DateTime DataFineEvento { get; set; }
        public DateTime UpdateDate { get; set; } = DateTime.Now;
    }
}
