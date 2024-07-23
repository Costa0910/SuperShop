using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Data.Entities;

namespace WebApplication.Data
{
    public class MockRepository: IRepository
    {
        public IEnumerable<Product> GetProducts()
        {
            List<Product> products = new()
            {
                new Product() { Id = 101, Name = "Test 101", Price = 34},
                new Product() { Id = 120, Name = "Test 120", Price = 32},
                new Product() { Id = 11, Name = "Test 11", Price = 22},
                new Product() { Id = 121, Name = "Test 121", Price = 111},
                new Product() { Id = 21, Name = "Test 21", Price = 12}
            };

            return products;
        }

        public Product GetProduct(int id)
            => throw new System.NotImplementedException();

        public void AddProduct(Product product)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateProduct(Product toUpdate)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteProduct(Product product)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> SaveAllAsync()
            => throw new System.NotImplementedException();

        public bool IsProductExist(int id)
            => throw new System.NotImplementedException();
    }
}