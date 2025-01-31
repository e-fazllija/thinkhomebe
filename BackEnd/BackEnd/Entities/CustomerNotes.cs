﻿namespace BackEnd.Entities
{
    public class CustomerNotes: EntityBase
    {
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
