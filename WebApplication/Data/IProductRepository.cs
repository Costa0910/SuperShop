using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication.Data.Entities;

namespace WebApplication.Data
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        public IQueryable<Product> GetAllWithUsers();
        public IEnumerable<SelectListItem> GetProducts();
    }
}