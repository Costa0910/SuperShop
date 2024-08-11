using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WebApplication.Data.Entities
{
    public class Order : IEntity
    {
        public int Id { get; set; }

        [Required, Display(Name = "Order date"), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}", ApplyFormatInEditMode = false)]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Delivery date"), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}", ApplyFormatInEditMode = false)]
        public DateTime? DeliveryDate { get; set; }

        [Required]
        public User User { get; set; }

        public IEnumerable<OrderDetails> OrderDetails { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Quantity => OrderDetails?.Sum(o => o.Quantity) ?? 0;

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Value => OrderDetails?.Sum(o => o.Value) ?? 0;

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int Lines => OrderDetails?.Count() ?? 0;

        [Display(Name = "Order date"), DisplayFormat(DataFormatString = "{0:MM/dd/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? OrderDateLocal => OrderDate.ToLocalTime();
    }
}