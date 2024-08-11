using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication.Data.Entities;
using WebApplication.Models;

namespace WebApplication.Data
{
    public interface ICountryRepository : IGenericRepository<Country>
    {
        IQueryable<Country> GetCountriesWithCities();
        Task<Country> GetCountryWithCitiesAsync(int id);
        Task<City> GetCityAsync(int id);
        Task AddCityAsync(CityViewModel model);
        Task<int> UpdateCityAsync(City city);
        Task<int> DeleteCityAsync(City city);
        IEnumerable<SelectListItem> GetComboCountries();
        Task<IEnumerable<SelectListItem>> GetComboCitiesAsync(int countryId);
        Task<Country> GetCountryAsync(City city);
    }
}