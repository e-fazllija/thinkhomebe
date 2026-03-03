namespace BackEnd.Models.RealEstatePropertyModels
{
    public class RealEstatePropertyDetailsModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool Sold { get; set; }
        public bool Negotiation { get; set; }
        public string AddressLine { get; set; } = string.Empty;
        public string Town { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostCode { get; set; } = string.Empty;
        public int CommercialSurfaceate { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int ParkingSpaces { get; set; }
        public string? Heating { get; set; }
        public string? EnergyClass { get; set; }
        public double Price { get; set; }
        public double PriceReduced { get; set; }
        public int MQGarden { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? Typology { get; set; }
        public string? VideoUrl { get; set; }
        public List<RealEstatePropertyPhotoDetailsModel> Photos { get; set; } = new();
        public RealEstatePropertyDetailsAgentModel? Agent { get; set; }
    }

    public class RealEstatePropertyPhotoDetailsModel
    {
        public string Url { get; set; } = string.Empty;
    }

    public class RealEstatePropertyDetailsAgentModel
    {
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public RealEstatePropertyDetailsAgencyModel? Agency { get; set; }
    }

    public class RealEstatePropertyDetailsAgencyModel
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Town { get; set; }
    }
}
