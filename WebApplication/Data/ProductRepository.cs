using WebApplication.Data.Entities;

namespace WebApplication.Data
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(DataContext dataContext) : base(dataContext) { }
    }
}