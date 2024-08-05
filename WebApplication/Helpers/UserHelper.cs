using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebApplication.Data.Entities;
using WebApplication.Models;

namespace WebApplication.Helpers
{
    public class UserHelper : IUserHelper
    {
        readonly UserManager<User> _userManager;
        readonly SignInManager<User> _signInManager;

        public UserHelper(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<User> GetUserByEmailAsync(string email)
            => await _userManager.FindByEmailAsync(email);

        public async Task<IdentityResult> AddUserAsync(User user, string password)
            => await _userManager.CreateAsync(user, password);

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
            => await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);

        public async Task LogoutAsync()
            => await _signInManager.SignOutAsync();

        public async Task<IdentityResult> UpdateUserAsync(User user)
            => await _userManager.UpdateAsync(user);

        public async Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword)
            => await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
    }
}