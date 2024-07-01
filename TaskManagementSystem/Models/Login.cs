using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Models
{
    public class Login
    {
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
