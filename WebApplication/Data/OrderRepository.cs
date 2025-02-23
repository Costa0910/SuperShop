using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data.Entities;
using WebApplication.Helpers;
using WebApplication.Models;
using WebApplication.Views;

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
                                   .Include(o => o.User)
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

        public async Task<Order> GetOrderAsync(int id)
            => await _dataContext.Orders.FindAsync(id);

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

        public async Task DeleteDetailTempAsync(int id)
        {
            var detailTemp = await _dataContext.OrderDetailsTemps.FindAsync(id);

            if (detailTemp == null)
                return;

            _dataContext.OrderDetailsTemps.Remove(detailTemp);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<bool> ConfirmOrderAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);

            if (user == null)
                return false;

            var tempOrders = await _dataContext.OrderDetailsTemps
                                         .Include(o => o.Product)
                                         .Where(o => o.User == user)
                                         .ToListAsync();

            if (tempOrders == null || tempOrders.Count == 0)
                return false;

            var orderDetails = tempOrders.Select(
                o => new OrderDetails()
                {
                    Price = o.Price,
                    Product = o.Product,
                    Quantity = o.Quantity,
                }).ToList();

            var order = new Order()
            {
                OrderDate = DateTime.UtcNow,
                User = user,
                OrderDetails = orderDetails
            };

            await CreateAsync(order);
            _dataContext.OrderDetailsTemps.RemoveRange(tempOrders);

            await _dataContext.SaveChangesAsync();
            return true;
        }

        public async Task DeliveryOrder(DeliveryViewModel model)
        {
            var order = await _dataContext.Orders.FindAsync(model.Id);

            if (order == null)
                return;

            order.DeliveryDate = model.DeliveryDate;
            _dataContext.Orders.Update(order);
            await _dataContext.SaveChangesAsync();
        }
    }
}