using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            {
                return RedirectToAction("Index", "Home");
            }

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
                {
                    return Redirect(Request.Query["ReturnUrl"].First());
                }

                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "Problem with your credentials");

            return View(model);
        }

        public IActionResult ChangeUser()
        {
            throw new System.NotImplementedException();
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid == false)
                return View(model);

            var user = _userHelper.GetUserByEmailAsync(model.Username);

            if (user != null)
            {
                ModelState.AddModelError(model.Username, "User already exist!");
                return View(model);
            }
            var newUser = new User()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Username,
                Email = model.Username
            };

            var createResult = await _userHelper.AddUserAsync(newUser, model.Password);

            if (createResult != IdentityResult.Success)
            {
                ModelState.AddModelError(string.Empty, "Could not create the user, try again later!");
                return View(model);
            }

            var loginDetails = new LoginViewModel()
            {
                Username = model.Username,
                Password = model.Password
            };

            var loginResult = await _userHelper.LoginAsync(loginDetails);

            if (loginResult.Succeeded)
            {
                RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(model.Username, "User created, but could not login. Try login manually");
            return View(model);
        }
    }
}