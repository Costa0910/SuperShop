using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Views;

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
            if (User.Identity == null)
                return NotFound();

            var model = await _orderRepository.GetOrderAsync(User.Identity.Name);

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            if (User.Identity == null)
                return NotFound();

            var model = await _orderRepository.GetDetailsTempsAsync(User.Identity.Name);

            return View(model);
        }

        public async Task<IActionResult> Deliver(int? id)
        {
            if (id == null)
                return NotFound();

            var order = await _orderRepository.GetOrderAsync(id.Value);

            if (order == null)
                return NotFound();

            var model = new DeliveryViewModel()
            {
                Id = id.Value,
                DeliveryDate = DateTime.Today
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Deliver(DeliveryViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _orderRepository.DeliveryOrder(model);

            return RedirectToAction("Index");

        }

        public IActionResult Delete()
            => throw new System.NotImplementedException();

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
        public async Task<IActionResult> AddProductAsync(AddItemViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (User.Identity != null)
                await _orderRepository.AddItemToOrderAsync(model, User.Identity.Name);

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

            await _orderRepository.DeleteDetailTempAsync(id.Value);

            return RedirectToAction("Create");
        }

        public async Task<IActionResult> ConfirmOrder()
        {
            if (User.Identity == null)
                return NotFound();
            var response = await _orderRepository.ConfirmOrderAsync(User.Identity.Name);

            if (response)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Create");
        }
    }
}