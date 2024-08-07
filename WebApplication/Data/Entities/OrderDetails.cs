using System.ComponentModel.DataAnnotations;

namespace WebApplication.Data.Entities
{
    public class OrderDetails : IEntity
    {
        public int Id { get; set; }

        [Required]
        public Product Product { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Price { get; set; }

        [DisplayFormat(DataFormatString = "{0:M2}")]
        public double Quantity { get; set; }

        public decimal Value => Price * (decimal)Quantity;
    }
}