using WebApplication.Data.Entities;
using WebApplication.Models;

namespace WebApplication.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        public Product ToProduct(ProductViewModel model, bool isNew, string path)
        {
            return new Product()
            {
                Id = isNew ? 0 : model.Id,
                Name = model.Name,
                ImageUrl = path,
                IsAvailable = model.IsAvailable,
                Price = model.Price,
                LastParchase = model.LastParchase,
                LastSale = model.LastSale,
                Stock = model.Stock,
                User = model.User
            };
        }

        public ProductViewModel ToProductViewModel(Product product)
        {
            return new ProductViewModel()
            {
                Id = product.Id,
                ImageUrl = product.ImageUrl,
                IsAvailable = product.IsAvailable,
                LastParchase = product.LastParchase,
                LastSale = product.LastSale,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
                User = product.User
            };
        }
    }
}