namespace BackEnd.Models.SpecificDocumentationModels
{
    public class SpecificDocumentationSelectModel
    {
        public int Id { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public int RealEstatePropertyId { get; set; }
    }
}
