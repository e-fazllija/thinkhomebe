namespace BackEnd.Entities
{
    public class SpecificDocumentation : EntityBase
    {
        public string DocumentType { get; set; } = string.Empty; // es. "IdentificationDocument", "TaxCodeOrHealthCard", ecc.
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        
        // TEMPORANEO: Manteniamo entrambe le colonne durante la migrazione
        public int? DocumentsTabId { get; set; }  // ⚠️ Da rimuovere dopo Migration 2
        public int RealEstatePropertyId { get; set; }
    }
}
