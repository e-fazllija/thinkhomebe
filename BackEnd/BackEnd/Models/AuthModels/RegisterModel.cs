using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.AuthModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Lastname is required")]
        public string Lastname { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public string? Role { get; set; } = null;
    }
}
