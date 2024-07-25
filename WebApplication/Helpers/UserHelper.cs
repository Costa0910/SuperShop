using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebApplication.Data.Entities;

namespace WebApplication.Helpers
{
    public class UserHelper: IUserHelper
    {
        readonly UserManager<User> _userManager;

        public UserHelper(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<User> GetUserByEmailAsync(string email)
            => await _userManager.FindByEmailAsync(email);

        public async Task<IdentityResult> AddUserAsync(User user, string password)
            => await _userManager.CreateAsync(user, password);
    }
}