﻿using System.ComponentModel.DataAnnotations;

namespace BackEnd.Entities
{
    public class RealEstatePropertyPhoto : EntityBase
    {
        public int RealEstatePropertyId { get; set; }
        public virtual RealEstateProperty RealEstateProperty { get; set; } = new RealEstateProperty();
        [Required]
        public string FileName { get; set; } = string.Empty;
        [Required]
        public string Url { get; set; } = string.Empty;
        [Required]
        public int Type { get; set; }
    }
}
