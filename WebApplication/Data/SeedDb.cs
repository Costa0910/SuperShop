using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
            await _dataContext.Database.MigrateAsync();

            await _userHelper.CreateRoleAsync(nameof(Roles.Admin));
            await _userHelper.CreateRoleAsync(nameof(Roles.Costumer));

            if (!await _dataContext.Countries.AnyAsync())
            {
                var cities = new List<City>()
                {
                    new City() { Name = "Lisboa" },
                    new City() { Name = "Porto" },
                    new City() { Name = "Faro" },

                };

                _dataContext.Countries.Add(new Country()
                {
                    Name = "Portugal",
                    Cities = cities
                });

                await _dataContext.SaveChangesAsync();
            }

            var user = await _userHelper.GetUserByEmailAsync("Costa0910@gmail.com");

            if (user == null)
            {
                var country = await _dataContext.Countries.Include(c => c.Cities).FirstOrDefaultAsync();

                user = new User()
                {
                    FirstName = "Costa",
                    LastName = "0910",
                    Email = "Costa0910@gmail.com",
                    UserName = "Costa0910@gmail.com",
                    Address = "Rua antonio",
                    CityId = country.Cities.FirstOrDefault().Id,
                    City = country.Cities.FirstOrDefault()
                };
                var result = await _userHelper.AddUserAsync(user, "123456");

                if (result != IdentityResult.Success)
                    throw new InvalidOperationException("Could not create the user in seeder");

                await _userHelper.AddUserToRoleAsync(user, nameof(Roles.Admin));

                var token = await  _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);
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