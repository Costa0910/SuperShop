using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Data
{
    /// <summary>
    /// Create Db if not exists and seed 3 products into table Products
    /// </summary>
	public class SeedDb
	{
        public readonly DataContext _dataContext;
        private Random _random;

        public SeedDb(DataContext dataContext)
		{
            _dataContext = dataContext;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _dataContext.Database.EnsureCreatedAsync();

            if (_dataContext.Products.Any() == false)
            {
                await AddProductAsync("Samsung A14");
                await AddProductAsync("Pixel 8");
                await AddProductAsync("Samsung s23 ultra");
            }

        }

        private async Task AddProductAsync(string name)
        {
            await _dataContext.Products.AddAsync(new Entities.Product
            {
                Name = name,
                Price = _random.Next(100, 1400),
                IsAvailable = true,
                Stock = _random.Next(10, 50)
            });

            await _dataContext.SaveChangesAsync();
        }
    }
}

