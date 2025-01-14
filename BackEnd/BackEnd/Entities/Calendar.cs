﻿using System.ComponentModel.DataAnnotations;

namespace BackEnd.Entities
{
    public class Calendar: EntityBase
    {
        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;
        public virtual ApplicationUser ApplicationUser { get; set; }
        [Required]
        public string NomeEvento { get; set; } = string.Empty;
        [Required]
        public string Type { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }
        public string? DescrizioneEvento { get; set; }
        public string? LuogoEvento { get; set; }
        public DateTime DataInizioEvento { get; set; }
        public DateTime DataFineEvento { get; set; }
    }
}
