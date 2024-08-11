using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data.Entities;
using WebApplication.Models;
using WebApplication.Views;

namespace WebApplication.Data
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IQueryable<Order>> GetOrderAsync(string userName);
        Task<Order> GetOrderAsync(int id);
        Task<IQueryable<OrderDetailsTemp>> GetDetailsTempsAsync(string userName);
        Task AddItemToOrderAsync(AddItemViewModel model, string userName);
        Task ModifyOrderDetailTempQuantityAsync(int id, double quantity);
        Task DeleteDetailTempAsync(int id);
        Task<bool> ConfirmOrderAsync(string userName);
        Task DeliveryOrder(DeliveryViewModel model);
    }
}