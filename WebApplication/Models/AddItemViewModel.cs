using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication.Models
{
    public class AddItemViewModel
    {
        [Display(Name = "Product"), Range(1, int.MaxValue, ErrorMessage = "You must select a product.")]
        public int ProductId { get; set; }

        [Range(0.0001, double.MaxValue, ErrorMessage = "The quantity mush be positive number")]
        public double Quantity { get; set; }

        public IEnumerable<SelectListItem> Products { get; set; }
    }
}