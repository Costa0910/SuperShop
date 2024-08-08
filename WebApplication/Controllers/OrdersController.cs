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

        [HttpPost]
        public IActionResult AddProduct(AddItemViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _orderRepository.AddItemToOrderAsync(model, User.Identity.Name);

            return RedirectToAction("Create");
        }

        public async Task<IActionResult> Increase(int? id)
        {
            if (id == null)
                return NotFound();

            await _orderRepository.ModifyOrderDetailTempQuantityAsync(id.Value, 1);

            return RedirectToAction("Create");
        }

        public async Task<IActionResult> Decrease(int? id)
        {
            if (id == null)
                return NotFound();

            await _orderRepository.ModifyOrderDetailTempQuantityAsync(id.Value, -1);

            return RedirectToAction("Create");
        }

        public async Task<IActionResult> DeleteItem(int? id)
        {
            if (id == null)
                return NotFound();

            await _orderRepository.DeleteDeatilTempAsync(id.Value);

            return RedirectToAction("Create");
        }
    }
}