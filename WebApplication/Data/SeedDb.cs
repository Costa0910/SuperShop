using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebApplication.Data.Entities;
using WebApplication.Helpers;

namespace WebApplication.Data
{
    /// <summary>
    /// Create Db if not exists and create user admin, seed 3 admin products into table Products
    /// </summary>
    public class SeedDb
    {
        readonly DataContext _dataContext;
        readonly IUserHelper _userHelper;
        readonly Random _random;

        public SeedDb(DataContext dataContext, IUserHelper userHelper)
        {
            _dataContext = dataContext;
            _userHelper = userHelper;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _dataContext.Database.EnsureCreatedAsync();

            await _userHelper.CreateRoleAsync(nameof(Roles.Admin));
            await _userHelper.CreateRoleAsync(nameof(Roles.Costumer));

            var user = await _userHelper.GetUserByEmailAsync("Costa0910@gmail.com");

            if (user == null)
            {
                user = new User()
                {
                    FirstName = "Costa",
                    LastName = "0910",
                    Email = "Costa0910@gmail.com",
                    UserName = "Costa0910@gmail.com"
                };
                var result = await _userHelper.AddUserAsync(user, "123456");

                if (result != IdentityResult.Success)
                    throw new InvalidOperationException("Could not create the user in seeder");

                await _userHelper.AddUserToRoleAsync(user, nameof(Roles.Admin));
            }

            var isInRole = await _userHelper.IsUserInRoleAsync(user, nameof(Roles.Admin));

            if (!isInRole)
            {
                await _userHelper.AddUserToRoleAsync(user, nameof(Roles.Admin));
            }

            if (_dataContext.Products.Any() == false)
            {
                await AddProductAsync("Samsung A14", user);
                await AddProductAsync("Pixel 8", user);
                await AddProductAsync("Samsung s23 ultra", user);
                await _dataContext.SaveChangesAsync();
            }
        }

        async Task AddProductAsync(string name, User user)
        {
            await _dataContext.Products.AddAsync(
                new Entities.Product
                {
                    Name = name,
                    Price = _random.Next(100, 1400),
                    IsAvailable = true,
                    Stock = _random.Next(10, 50),
                    User = user
                });
        }
    }
}