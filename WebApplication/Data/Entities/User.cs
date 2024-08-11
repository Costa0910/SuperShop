using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WebApplication.Data.Entities
{
    public class User : IdentityUser
    {
        [MaxLength(50, ErrorMessage = "The field {0} only contain {1} characters length.")]

        public string FirstName { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} only contain {1} characters length.")]

        public string LastName { get; set; }

        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";

        [MaxLength(100, ErrorMessage = "The field {0} only contain {1} characters length.")]
        public string Address { get; set; }

        public int CityId { get; set; }

        public City City { get; set; }
    }
}