using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebApplication.Data;
using WebApplication.Data.Entities;
using WebApplication.Helpers;
using WebApplication.Models;
using WebApplication.Views;

namespace WebApplication.Controllers
{
    public class AccountController : Controller
    {
        readonly IConfiguration _configuration;
        readonly IUserHelper _userHelper;
        readonly ICountryRepository _countryRepository;
        readonly IMailHelper _mailHelper;

        public AccountController(IConfiguration configuration, IUserHelper userHelper, ICountryRepository countryRepository, IMailHelper mailHelper)
        {
            _configuration = configuration;
            _userHelper = userHelper;
            _countryRepository = countryRepository;
            _mailHelper = mailHelper;
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

            //
            // var loginDetails = new LoginViewModel
            // {
            //     Username = model.Username,
            //     Password = model.Password
            // };

            var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(newUser);

            var confirmLink = Url.Action(
                "ConfirmEmail",
                "Account",
                new
                {
                    userId = newUser.Id,
                    token = myToken
                },
                protocol: HttpContext.Request.Scheme);

            var response = _mailHelper.SendMail(
                model.Username,
                "Email confirmation",
                $"<h1>Confirm your Account</h1> <p>To complete registration, please click in this link: <br/> <a href='{confirmLink}'>Confirm your email</a></p>");

            //var loginResult = await _userHelper.LoginAsync(loginDetails);

            if (response.IsSuccess)
            {
                ViewBag.Message = "The instructions to confirm your email has been sent.";

                return View(model);
            }

            ModelState.AddModelError(model.Username, "User could not be logged.");

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

        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);

                if (user != null)
                {
                    var result = await _userHelper.ValidatePasswordAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(
                            _configuration["Tokens:Issuer"],
                            _configuration["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddDays(15),
                            signingCredentials: credentials);

                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return Created(string.Empty, results);
                    }
                }
            }

            return BadRequest();
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (!ModelState.IsValid)
                return NotFound();

            var user = await _userHelper.GetUserByIdAsync(userId);

            if (user == null)
                return NotFound();

            var result = await _userHelper.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
                return NotFound();

            return View();
        }

        public IActionResult RecoverPassword()
            => View();

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "The email doesn't correspond to a registered user.");

                    return View(model);
                }
                var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);

                var link = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { token = myToken },
                    protocol: HttpContext.Request.Scheme);

                var response = _mailHelper.SendMail(
                    model.Email,
                    "SuperShop password reset",
                    $"<h1>SuperShop</h1> <p>To reset your password click in the link below: <br/> <a href='{link}'>Reset password</a></p>");

                if (response.IsSuccess)
                    ViewBag.Message = "The instructions to recover your password has been sent to email";

                return View();
            }

            return View(model);
        }

        public IActionResult ResetPassword(string token) {
            if (!ModelState.IsValid)
                return View();

            var model = new ResetPasswordViewModel()
            {
                Token = token
            };

            return View(model);

        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userHelper.GetUserByEmailAsync(model.Username);

            if (user != null)
            {
                var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);

                if (result.Succeeded)
                {
                    ViewBag.Message = "Password reset successful.";

                    return View();
                }

                ViewBag.Message = "Error while resetting the password.";

                return View(model);
            }

            ViewBag.Message = "User not found.";

            return View(model);
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