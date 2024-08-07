using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Data;

namespace WebApplication.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        readonly IOrderRepository _orderRepository;

        public OrdersController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        // GET
        public async Task<IActionResult> Index()
        {
            var model = await _orderRepository.GetOrderAsync(User.Identity.Name);

            return View(model);
        }

        public IActionResult Create()
        {
            throw new System.NotImplementedException();
        }

        public IActionResult Deliver()
        {
            throw new System.NotImplementedException();
        }

        public IActionResult Delete()
        {
            throw new System.NotImplementedException();
        }
    }
}