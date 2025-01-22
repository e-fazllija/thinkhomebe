namespace BackEnd.Entities
{
    public class RequestNotes : EntityBase
    {
        public int RequestId { get; set; }
        public virtual Request Request { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
