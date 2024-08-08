using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        readonly IOrderRepository _orderRepository;
        readonly IProductRepository _productRepository;

        public OrdersController(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        // GET
        public async Task<IActionResult> Index()
        {
            var model = await _orderRepository.GetOrderAsync(User.Identity.Name);

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var model = await _orderRepository.GetDetailsTempsAsync(User.Identity.Name);

            return View(model);
        }

        public IActionResult Deliver()
        {
            throw new System.NotImplementedException();
        }

        public IActionResult Delete()
        {
            throw new System.NotImplementedException();
        }

        public IActionResult AddProduct()
        {
            var model = new AddItemViewModel()
            {
                Quantity = 1,
                Products = _productRepository.GetProducts()
            };

            return View(model);
        }
    }
}