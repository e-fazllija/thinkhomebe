namespace BackEnd.Models.LocationModels
{
    public class LocationSelectModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int OrderIndex { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
} 