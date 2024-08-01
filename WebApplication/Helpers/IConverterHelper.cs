using System;
using WebApplication.Data.Entities;
using WebApplication.Models;

namespace WebApplication.Helpers
{
    public interface IConverterHelper
    {
        Product ToProduct(ProductViewModel model, bool isNew, Guid imageId);
        ProductViewModel ToProductViewModel(Product product);
    }
}