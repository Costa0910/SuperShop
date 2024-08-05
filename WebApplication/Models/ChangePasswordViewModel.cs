using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class ChangePasswordViewModel
    {
        [Required, Display(Name = "Old password")]
        public string OldPassword { get; set; }

        [Required, Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required, Display(Name = "Confirm Password"), Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}