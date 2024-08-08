using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data.Entities;
using WebApplication.Models;

namespace WebApplication.Data
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IQueryable<Order>> GetOrderAsync(string userName);
        Task<IQueryable<OrderDetailsTemp>> GetDetailsTempsAsync(string userName);
        Task AddItemToOrderAsync(AddItemViewModel model, string userName);
        Task ModifyOrderDetailTempQuantityAsync(int id, double quantity);
        Task DeleteDeatilTempAsync(int id);
    }
}