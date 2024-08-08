using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data.Entities;
using WebApplication.Helpers;

namespace WebApplication.Data
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        readonly DataContext _dataContext;
        readonly IUserHelper _userHelper;

        public OrderRepository(DataContext dataContext, IUserHelper userHelper) : base(dataContext)
        {
            _dataContext = dataContext;
            _userHelper = userHelper;
        }

        public async Task<IQueryable<Order>> GetOrderAsync(string userName)
        {
            //TODO: Check if it's possible to get user by email using user's name
            var user = await _userHelper.GetUserByEmailAsync(userName);

            if (user == null)
                return null;

            if (await _userHelper.IsUserInRoleAsync(user, nameof(Roles.Admin)))
            {
                return _dataContext.Orders
                                   .Include(o => o.OrderDetails)
                                   .ThenInclude(o => o.Product)
                                   .OrderByDescending(o => o.OrderDate);
            }

            return _dataContext.Orders
                               .Include(o => o.OrderDetails)
                               .ThenInclude(o => o.Product)
                               .Where(o => o.User == user)
                               .OrderByDescending(o => o.OrderDate);
        }

        public async Task<IQueryable<OrderDetailsTemp>> GetDetailsTempsAsync(string userName)
        {
            //TODO: Check if it's possible to get user by email using user's name
            var user = await _userHelper.GetUserByEmailAsync(userName);

            if (user == null)
                return null;

            return _dataContext.OrderDetailsTemps
                               .Include(o => o.Product)
                               .Where(o => o.User == user)
                               .OrderByDescending(o => o.Product.Name);
        }
    }
}