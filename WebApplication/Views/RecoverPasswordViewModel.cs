using System.ComponentModel.DataAnnotations;

namespace WebApplication.Views
{
    public class RecoverPasswordViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}