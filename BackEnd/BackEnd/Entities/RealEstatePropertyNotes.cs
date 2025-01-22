namespace BackEnd.Entities
{
    public class RealEstatePropertyNotes: EntityBase
    {
        public int RealEstatePropertyId { get; set; }
        public virtual RealEstateProperty RealEstateProperty { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
