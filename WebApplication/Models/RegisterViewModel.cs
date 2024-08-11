using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication.Models
{
    public class RegisterViewModel
    {
        [Display(Name = "First Name"), Required]
        public string FirstName { get; set; }

        [Display(Name = "First Name"), Required]
        public string LastName { get; set; }

        [MaxLength(100, ErrorMessage = "The field {0} only can contain {1} characters")]
        public string Address { get; set; }

        [MaxLength(20, ErrorMessage = "The field {0} only can contain {1} characters")]
        public string PhoneNumber { get; set; }

        [Display(Name = "City"), Range(1, int.MaxValue, ErrorMessage = "You must select a city")]
        public int CityId { get; set; }

        public IEnumerable<SelectListItem> Cities { get; set; }

        [Display(Name = "Country"), Range(1, int.MaxValue, ErrorMessage = "You must select a country")]
        public int CountryId { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        [Required, DataType(DataType.EmailAddress), Display(Name = "Email")]
        public string Username { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Required, Compare("Password"), Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; }
    }
}