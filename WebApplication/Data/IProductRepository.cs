using System.Linq;
using WebApplication.Data.Entities;

namespace WebApplication.Data
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        public IQueryable<Product> GetAllWithUsers();
    }
}