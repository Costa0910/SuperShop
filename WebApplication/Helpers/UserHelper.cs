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
        readonly RoleManager<IdentityRole> _roleManager;

        public UserHelper(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
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

        public async Task CreateRoleAsync(string role)
        {
            var alreadyExist = await _roleManager.RoleExistsAsync(role);

            if (!alreadyExist)
            {
                await _roleManager.CreateAsync(new IdentityRole() { Name = role });
            }
        }

        public async Task AddUserToRoleAsync(User user, string roleName)
            => await _userManager.AddToRoleAsync(user, roleName);

        public async Task<bool> IsUserInRoleAsync(User user, string roleName)
            => await _userManager.IsInRoleAsync(user, roleName);

        public async Task<SignInResult> ValidatePasswordAsync(User user, string password)
            => await _signInManager.CheckPasswordSignInAsync(user, password, false);

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
            => await _userManager.GenerateEmailConfirmationTokenAsync(user);

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<User> GetUserByIdAsync(string userId)
            => await _userManager.FindByIdAsync(userId);
    }
}