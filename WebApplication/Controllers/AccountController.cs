using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication.Data;
using WebApplication.Data.Entities;
using WebApplication.Helpers;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class AccountController : Controller
    {
        readonly IUserHelper _userHelper;
        readonly ICountryRepository _countryRepository;

        public AccountController(IUserHelper userHelper, ICountryRepository countryRepository)
        {
            _userHelper = userHelper;
            _countryRepository = countryRepository;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid == false)
                return View(model);

            var result = await _userHelper.LoginAsync(model);

            if (result.Succeeded)
            {
                if (Request.Query.Keys.Contains("ReturnUrl"))
                    return Redirect(Request.Query["ReturnUrl"].First());

                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "Problem with your credentials");

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Register()
        {
            var model = new RegisterViewModel
            {
                Countries = _countryRepository.GetComboCountries(),
                Cities = await _countryRepository.GetComboCitiesAsync(0)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid == false)
                return View(model);

            var user = await _userHelper.GetUserByEmailAsync(model.Username);

            if (user != null)
            {
                ModelState.AddModelError(model.Username, "User already exist!");

                return View(model);
            }

            var city = await _countryRepository.GetCityAsync(model.CityId);

            var newUser = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Username,
                Email = model.Username,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                CityId = model.CityId,
                City = city
            };

            var createResult = await _userHelper.AddUserAsync(newUser, model.Password);

            if (createResult.Succeeded == false)
            {
                ModelState.AddModelError(string.Empty, "Could not create the user, try again later!");

                return View(model);
            }

            await _userHelper.AddUserToRoleAsync(newUser, nameof(Roles.Costumer));

            var loginDetails = new LoginViewModel
            {
                Username = model.Username,
                Password = model.Password
            };

            var loginResult = await _userHelper.LoginAsync(loginDetails);

            if (loginResult.Succeeded)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError(model.Username, "User created, but could not log in. Try login manually");

            return View(model);
        }

        public async Task<IActionResult> ChangeUser()
        {
            var currentUser = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

            var newModel = new ChangeUserViewModel();

            if (currentUser != null)
            {
                newModel.FirstName = currentUser.FirstName;
                newModel.LastName = currentUser.LastName;
                newModel.Address = currentUser.Address;
                newModel.PhoneNumber = currentUser.PhoneNumber;
                var city = await _countryRepository.GetCityAsync(currentUser.CityId);

                if (city != null)
                {
                    var country = await _countryRepository.GetCountryAsync(city);

                    if (country != null)
                    {
                        newModel.CountryId = country.Id;
                        newModel.CityId = city.Id;
                        newModel.Cities = await _countryRepository.GetComboCitiesAsync(country.Id);
                        newModel.Countries = _countryRepository.GetComboCountries();
                    }
                }
            }
            newModel.Cities = await _countryRepository.GetComboCitiesAsync(newModel.CountryId);
            newModel.Countries = _countryRepository.GetComboCountries();

            return View(newModel);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUser(ChangeUserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

                if (currentUser != null)
                {
                    var city = await _countryRepository.GetCityAsync(userViewModel.CityId);
                    currentUser.FirstName = userViewModel.FirstName;
                    currentUser.LastName = userViewModel.LastName;
                    currentUser.Address = userViewModel.Address;
                    currentUser.PhoneNumber = userViewModel.PhoneNumber;
                    currentUser.CityId = userViewModel.CityId;
                    currentUser.City = city;

                    var updateResult = await _userHelper.UpdateUserAsync(currentUser);

                    if (updateResult.Succeeded)
                        ViewBag.userMessage = "User data updated.";
                    else
                        ModelState.AddModelError(string.Empty, "User was not updated!");
                }
            }

            return View(userViewModel);
        }

        public IActionResult ChangePassword()
            => View();

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel passwordViewModel)
        {
            if (ModelState.IsValid == false)
                return View(passwordViewModel);

            var currentUser = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

            if (currentUser == null)
            {
                ModelState.AddModelError(string.Empty, "Could not update password!");

                return View(passwordViewModel);
            }

            var changePasswordResult = await _userHelper.ChangePasswordAsync(currentUser, passwordViewModel.OldPassword, passwordViewModel.NewPassword);

            if (changePasswordResult.Succeeded)
                return RedirectToAction("ChangeUser");

            ModelState.AddModelError(string.Empty, "Could not update password!");

            return View(passwordViewModel);
        }

        public IActionResult NotAuthorized()
            => View();

        [HttpPost, Route("/Account/GetCitiesAsync")]
        public async Task<JsonResult> GetCitiesAsync(int countryId)
        {
            var country = await _countryRepository.GetCountryWithCitiesAsync(countryId);

            return Json(country.Cities.OrderBy(c => c.Name));
        }

    }
}