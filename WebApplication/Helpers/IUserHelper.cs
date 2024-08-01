using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebApplication.Data.Entities;
using WebApplication.Models;

namespace WebApplication.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<IdentityResult> AddUserAsync(User user, string password);
        Task<SignInResult> LoginAsync(LoginViewModel model);
        Task LogoutAsync();
    }
}