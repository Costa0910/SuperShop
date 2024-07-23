using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Data.Entities;

namespace WebApplication.Data
{
    public interface IRepository
    {
        IEnumerable<Product> GetProducts();
        Product GetProduct(int id);
        void AddProduct(Product product);
        void UpdateProduct(Product toUpdate);
        void DeleteProduct(Product product);
        Task<bool> SaveAllAsync();
        bool IsProductExist(int id);
    }
}