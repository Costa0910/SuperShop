using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class RegisterViewModel
    {
        [Display(Name = "First Name"), Required]
        public string FirstName { get; set; }

        [Display(Name = "First Name"), Required]
        public string LastName { get; set; }

        [Required, DataType(DataType.EmailAddress), Display(Name = "Email")]
        public string Username { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Required, Compare("Password"), Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; }
    }
}