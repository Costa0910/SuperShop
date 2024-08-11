using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data.Entities;
using WebApplication.Models;

namespace WebApplication.Data
{
    public class CountryRepository : GenericRepository<Country>, ICountryRepository {
        readonly DataContext _dataContext;
        public CountryRepository(DataContext dataContext) : base(dataContext)
        {
            _dataContext = dataContext;
        }

        public IQueryable<Country> GetCountriesWithCities()
            => _dataContext.Countries.Include(c => c.Cities).OrderBy(c => c.Name);

        public async Task<Country> GetCountryWithCitiesAsync(int id)
            => await _dataContext.Countries.Include(c => c.Cities).Where(c => c.Id == id).FirstOrDefaultAsync();

        public async Task<City> GetCityAsync(int id)
            => await _dataContext.Cities.FindAsync(id);

        public async Task AddCityAsync(CityViewModel model)
            {
                var country = await GetCountryWithCitiesAsync(model.CountryId);

                if (country == null)
                {
                    return;
                }


                country.Cities.Add(new City { Name = model.Name });
                _dataContext.Countries.Update(country);
                await _dataContext.SaveChangesAsync();

        }

        public async Task<int> UpdateCityAsync(City city)
        {
            var country = await _dataContext.Countries
                                            .Where(c => c.Cities.Any(ci => ci.Id == city.Id)).FirstOrDefaultAsync();

            if (country == null)
                return 0;

            _dataContext.Cities.Update(city);
            await _dataContext.SaveChangesAsync();

            return country.Id;
        }

        public async Task<int> DeleteCityAsync(City city)
        {
            var country = await _dataContext.Countries
                                            .Where(c => c.Cities.Any(ci => ci.Id == city.Id))
                                            .FirstOrDefaultAsync();

            if (country == null)
                return 0;

            _dataContext.Cities.Remove(city);
            await _dataContext.SaveChangesAsync();

            return country.Id;
        }

        public async Task<Country> GetCountryAsync(City city)
            => await _dataContext.Countries.Where(c => c.Cities.Any(ci => ci.Id == city.Id)).FirstOrDefaultAsync();
    }
}