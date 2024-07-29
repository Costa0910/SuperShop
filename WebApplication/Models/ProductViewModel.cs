using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using WebApplication.Data.Entities;

namespace WebApplication.Models
{
    public class ProductViewModel : Product
    {
        [Display(Name="Image")]
        public IFormFile ImageFile { get; set; }
    }
}