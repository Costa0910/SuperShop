using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data.Entities;

namespace WebApplication.Data
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        readonly DataContext _dataContext;
        public ProductRepository(DataContext dataContext) : base(dataContext)
        {
            _dataContext = dataContext;
        }

        public IQueryable<Product> GetAllWithUsers()
        {
            return _dataContext.Products.Include(p => p.User);
        }
    }
}