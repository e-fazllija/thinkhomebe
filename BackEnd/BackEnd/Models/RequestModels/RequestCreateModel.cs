using BackEnd.Entities;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.RequestModels
{
    public class RequestCreateModel
    {
        [Required]
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        [Required]
        public string Contract { get; set; }
        [Required]
        public string PropertyType { get; set; }
        [Required]
        public string province { get; set; }
        [Required]
        public string City { get; set; }
        public string RoomsNumber { get; set; }
        public int MQFrom { get; set; }
        public int MQTo { get; set; }
        public string PropertyState { get; set; }
        public string Heating { get; set; }
        public int ParkingSpaces { get; set; }
        [Required]
        public double Price { get; set; }
        public string Notes { get; set; }
    }
}
