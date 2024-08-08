using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public IEnumerable<SelectListItem> GetProducts()
        {
            var list = _dataContext.Products.Select(
                p => new SelectListItem()
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                }).ToList();

            list.Insert(
                0,
                new SelectListItem()
                {
                    Text = "(Select a product...)",
                    Value = "0"
                });

            return list;
        }
    }
}