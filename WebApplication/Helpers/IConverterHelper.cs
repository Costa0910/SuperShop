using WebApplication.Data.Entities;
using WebApplication.Models;

namespace WebApplication.Helpers
{
    public interface IConverterHelper
    {
        Product ToProduct(ProductViewModel model, bool isNew, string path);
        ProductViewModel ToProductViewModel(Product product);
    }
}