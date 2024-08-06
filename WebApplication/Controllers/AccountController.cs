using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Data;
using WebApplication.Data.Entities;
using WebApplication.Helpers;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class AccountController : Controller
    {
        readonly IUserHelper _userHelper;

        public AccountController(IUserHelper userHelper)
        {
            _userHelper = userHelper;
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

        public IActionResult Register()
            => View();

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
            var newUser = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Username,
                Email = model.Username
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
            }

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
                    currentUser.FirstName = userViewModel.FirstName;
                    currentUser.LastName = userViewModel.LastName;

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
    }
}