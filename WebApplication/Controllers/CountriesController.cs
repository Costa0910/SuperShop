using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Data;
using WebApplication.Data.Entities;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class CountriesController : Controller
    {
        readonly ICountryRepository _countryRepository;

        public CountriesController(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        // GET
        public IActionResult Index()
            => View(_countryRepository.GetCountriesWithCities());

        public async Task<IActionResult> DeleteCity(int? id)
        {
            if (id == null)
                return NotFound();

            var city = await _countryRepository.GetCityAsync(id.Value);

            if (city == null)
                return NotFound();

            var countryId = await _countryRepository.DeleteCityAsync(city);

            return RedirectToAction("Details", new { id = countryId });
        }

        public async Task<IActionResult> EditCity(int? id)
        {
            if (id == null)
                return NotFound();

            var city = await _countryRepository.GetCityAsync(id.Value);

            if (city == null)
                return NotFound();

            return View(city);
        }

        [HttpPost]
        public async Task<IActionResult> EditCity(City city)
        {
            if (ModelState.IsValid)
            {
                var countryId = await _countryRepository.UpdateCityAsync(city);

                if (countryId != 0)
                    return RedirectToAction("Details", new { id = countryId });
            }

            return View(city);
        }

        public async Task<IActionResult> AddCity(int? id)
        {
            if (id == null)
                return NotFound();

            var country = await _countryRepository.GetByIdAsync(id.Value);

            if (country == null)
                return NotFound();

            var model = new CityViewModel { CountryId = country.Id };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddCity(CityViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _countryRepository.AddCityAsync(model);

                return RedirectToAction("Details", new { id = model.CountryId });
            }

            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var country = await _countryRepository.GetCountryWithCitiesAsync(id.Value);

            if (country == null)
                return NotFound();

            return View(country);
        }

        public IActionResult Create()
            => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Country country)
        {
            if (ModelState.IsValid)
            {
                await _countryRepository.CreateAsync(country);

                return RedirectToAction(nameof(Index));
            }

            return View(country);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var country = await _countryRepository.GetByIdAsync(id.Value);

            if (country == null)
                return NotFound();

            return View(country);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Country country)
        {
            if (ModelState.IsValid)
            {
                await _countryRepository.UpdateAsync(country);

                return RedirectToAction(nameof(Index));
            }

            return View(country);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var country = await _countryRepository.GetByIdAsync(id.Value);

            if (country == null)
                return NotFound();

            await _countryRepository.DeleteAsync(country);

            return RedirectToAction(nameof(Index));
        }
    }
}