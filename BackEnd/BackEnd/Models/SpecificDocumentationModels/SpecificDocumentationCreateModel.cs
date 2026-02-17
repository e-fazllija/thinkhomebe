namespace BackEnd.Models.SpecificDocumentationModels
{
    public class SpecificDocumentationCreateModel
    {
        public string DocumentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public int RealEstatePropertyId { get; set; }
    }
}
