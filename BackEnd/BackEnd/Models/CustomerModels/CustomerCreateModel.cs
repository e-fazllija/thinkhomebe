﻿using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.CustomerModels
{
    public class CustomerCreateModel
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
        public DateTime CreationDate { get; set; } = DateTime.Now;

    }
}