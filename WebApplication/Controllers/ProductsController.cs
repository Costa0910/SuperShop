using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Data.Entities;
using WebApplication.Helpers;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class ProductsController : Controller
    {
        readonly IProductRepository _productRepository;
        readonly IUserHelper _userHelper;

        public ProductsController(IProductRepository productRepository, IUserHelper userHelper)
        {
            _productRepository = productRepository;
            _userHelper = userHelper;
        }

        // GET: Products
        public IActionResult Index()
            => View(_productRepository.GetAll().OrderBy(p => p.Name));

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _productRepository.GetByIdAsync(id.Value);

            if (product == null)
                return NotFound();

            return View(product);
        }

        ProductViewModel ToProductViewModel(Product product)
        {
            return new ProductViewModel()
            {
                Id = product.Id,
                ImageUrl = product.ImageUrl,
                IsAvailable = product.IsAvailable,
                LastParchase = product.LastParchase,
                LastSale = product.LastSale,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
                User = product.User
            };
        }

        // GET: Products/Create
        public IActionResult Create()
            => View();

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var path = string.Empty;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var newFileName = $"{Guid.NewGuid()}.jpg";

                path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\products", newFileName);

                await using var stream = new FileStream(path, FileMode.Create);

                await model.ImageFile.CopyToAsync(stream);
                path = $"~/images/products/{newFileName}";
            }

            var product = ToProduct(model, path);

            //TODO: change for login user
            product.User = await _userHelper.GetUserByEmailAsync("Costa0910@gmail.com");
            await _productRepository.CreateAsync(product);

            return RedirectToAction(nameof(Index));
        }

        Product ToProduct(ProductViewModel model, string path)
        {
            return new Product()
            {
                Id = model.Id,
                Name = model.Name,
                ImageUrl = path,
                IsAvailable = model.IsAvailable,
                Price = model.Price,
                LastParchase = model.LastParchase,
                LastSale = model.LastSale,
                Stock = model.Stock,
                User = model.User
            };
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _productRepository.GetByIdAsync(id.Value);

            if (product == null)
                return NotFound();

            var model = ToProductViewModel(product);

            return View(model);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var path = model.ImageUrl;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        var newFileName = $"{Guid.NewGuid()}.jpg";

                        path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\products", newFileName);

                        await using var stream = new FileStream(path, FileMode.Create);

                        await model.ImageFile.CopyToAsync(stream);
                        path = $"~/wwwroot/images/products/{newFileName}";
                    }
                    var product = ToProduct(model, path);

                    //TODO: change for login user
                    product.User = await _userHelper.GetUserByEmailAsync("Costa0910@gmail.com");
                    await _productRepository.UpdateAsync(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _productRepository.ExistAsync(id))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _productRepository.GetByIdAsync(id.Value);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            await _productRepository.DeleteAsync(product);

            return RedirectToAction(nameof(Index));
        }
    }
}