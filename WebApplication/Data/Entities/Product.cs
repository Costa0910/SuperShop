using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Data.Entities
{
    public class Product : IEntity
    {
        public int Id { get; set; }

        [Required, MaxLength(50, ErrorMessage = "The field {0} can contain {1} characters length.")]
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }

        [DisplayName("Image")]
        public string ImageUrl { get; set; }

        [DisplayName("Last Purchase")]
        public DateTime? LastParchase { get; set; }

        [DisplayName("Last Sale")]
        public DateTime? LastSale { get; set; }

        [DisplayName("Is Available")]
        public bool IsAvailable { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public double Stock { get; set; }

        public User User { get; set; }

        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(ImageUrl))
                {
                    return null;
                }

                return $"https://localhost:5001{ImageUrl.Substring(1)}";
            }
        }
    }
}