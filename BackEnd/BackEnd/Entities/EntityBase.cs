using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Entities
{
    public class EntityBase
    {
        public int Id { get; set; } 
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }
        //[ForeignKey(nameof(ApplicationUser))]
        //public string ApplicationUserId { get; set; } = string.Empty;
        //public virtual ApplicationUser ApplicationUser { get; set; } = new ApplicationUser();
    }
}
