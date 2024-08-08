using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data.Entities;
using WebApplication.Helpers;
using WebApplication.Models;

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
            var user = await _userHelper.GetUserByEmailAsync(userName);

            if (user == null)
                return null;

            return _dataContext.OrderDetailsTemps
                               .Include(o => o.Product)
                               .Where(o => o.User == user)
                               .OrderByDescending(o => o.Product.Name);
        }

        public async Task AddItemToOrderAsync(AddItemViewModel model, string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);

            if (user == null)
                return;

            var product = await _dataContext.Products.FindAsync(model.ProductId);

            if (product == null)
                return;

            var orderDetailTemp = await _dataContext.OrderDetailsTemps
                                                    .Where(odt => odt.User == user && odt.Product == product)
                                                    .FirstOrDefaultAsync();

            if (orderDetailTemp == null)
            {
                orderDetailTemp = new()
                {
                    Price = product.Price,
                    Quantity = model.Quantity,
                    Product = product,
                    User = user
                };

                _dataContext.OrderDetailsTemps.Add(orderDetailTemp);
            }
            else
            {
                orderDetailTemp.Quantity += model.Quantity;
                _dataContext.OrderDetailsTemps.Update(orderDetailTemp);
            }

            await _dataContext.SaveChangesAsync();
        }

        public async Task ModifyOrderDetailTempQuantityAsync(int id, double quantity)
        {
            var orderDetailTemp = await _dataContext.OrderDetailsTemps.FindAsync(id);

            if (orderDetailTemp == null)
                return;

            orderDetailTemp.Quantity += quantity;

            if (orderDetailTemp.Quantity > 0)
            {
                await _dataContext.SaveChangesAsync();
            }
        }
    }
}