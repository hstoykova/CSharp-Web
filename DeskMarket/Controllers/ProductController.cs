using DeskMarket.Data;
using DeskMarket.Data.Models;
using DeskMarket.Models;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;
using System.Security.Policy;
using static DeskMarket.Constants.ValidationConstants;

namespace DeskMarket.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext context;

        public ProductController(ApplicationDbContext _context)
        {
            context = _context;
        }

        [Authorize]
        [HttpGet]
        public async Task <IActionResult> Add()
        {
            var model = new ProductViewModel();
            model.Categories = await GetCategories();

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(ProductViewModel model)
        {
            if (ModelState.IsValid == false)
            {
                model.Categories = await GetCategories();

                return View(model);
            }

            DateTime addedOn;

            if (DateTime.TryParseExact(model.AddedOn, AddedOnDateFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out addedOn) == false)
            {
                ModelState.AddModelError(nameof(model.AddedOn), "Invalid date format");
                model.Categories = await GetCategories();

                return View(model);
            }

            Product product = new()
            {
                ProductName = model.ProductName,
                Price = model.Price,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                AddedOn = addedOn,
                CategoryId = model.CategoryId,
                SellerId = GetCurrentUserId() ?? string.Empty
            };

            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();

            var model = await context.Products
                .Where(p => p.IsDeleted == false)
                .AsNoTracking()
                .Select(p => new ProductInfoViewModel()
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    IsSeller = p.SellerId == userId,
                    HasBought = p.ProductsClients.Any(pc => pc.ClientId == userId)
                })
                .ToListAsync();

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            var userId = GetCurrentUserId();

            var model = await context.Products
                .Where(p => p.IsDeleted == false)
                .Where(p => p.ProductsClients.Any(pc => pc.ClientId == userId))
                .AsNoTracking()
                .Select(p => new ProductInfoViewModel()
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                })
                .ToListAsync();

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await context.Products
                .Where(p => p.Id == id)
                .Where(p => p.IsDeleted == false)
                .AsNoTracking()
                .Select(p => new ProductEditViewModel()
                {
                    ProductName = p.ProductName,
                    Price = p.Price,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl,
                    AddedOn = p.AddedOn.ToString(AddedOnDateFormat),
                    CategoryId = p.CategoryId,
                    SellerId = p.SellerId

                })
                .FirstOrDefaultAsync();

            model.Categories = await GetCategories();

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(ProductEditViewModel model, int id)
        {
            if (ModelState.IsValid == false)
            {
                model.Categories = await GetCategories();

                return View(model);
            }

            DateTime addedOn;

            if (DateTime.TryParseExact(model.AddedOn, AddedOnDateFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out addedOn) == false)
            {
                ModelState.AddModelError(nameof(model.AddedOn), "Invalid date format");
                model.Categories = await GetCategories();

                return View(model);
            }

            Product? product = await context.Products.FindAsync(id);

            if (product == null || product.IsDeleted)
            {
                return NotFound();
            }

            string currentUserId = GetCurrentUserId() ?? string.Empty;

            if (product.SellerId != currentUserId)
            {
                return RedirectToAction(nameof(Index));
            }

            product.ProductName = model.ProductName;
            product.Price = model.Price;
            product.Description = model.Description;
            product.ImageUrl = model.ImageUrl;
            product.AddedOn = addedOn;
            product.CategoryId = model.CategoryId;

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = GetCurrentUserId();

            var model = await context.Products
                .Where(p => p.Id == id)
                .Where(p => p.IsDeleted == false)
                .Select(p => new ProductDetailsViewModel()
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    ImageUrl = p.ImageUrl,
                    Description = p.Description,
                    CategoryName = p.Category.Name,
                    AddedOn = p.AddedOn.ToString(AddedOnDateFormat),
                    Seller = p.Seller.UserName ?? string.Empty,
                    HasBought = p.ProductsClients.Any(pc => pc.ClientId == userId),
                    Price = p.Price,

                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            Product? product = await context.Products
                .Where (p => p.Id == id)
                .Include(p => p.ProductsClients)
                .FirstOrDefaultAsync();

            if (product == null || product.IsDeleted)
            {
                return NotFound();
            }

            string currentUserId = GetCurrentUserId() ?? string.Empty;

            if (product.ProductsClients.Any(pc => pc.ClientId == currentUserId))
            {
                return RedirectToAction(nameof(Index));
            }

            product.ProductsClients.Add(new ProductClient()
            {
                ProductId = product.Id,
                ClientId = currentUserId
            });

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            Product? product = await context.Products
                .Where(p => p.Id == id)
                .Include(p => p.ProductsClients)
                .FirstOrDefaultAsync();

            if (product == null || product.IsDeleted)
            {
                return NotFound();
            }

            string currentUserId = GetCurrentUserId() ?? string.Empty;

            ProductClient? productClient = product.ProductsClients
                .FirstOrDefault(pc => pc.ClientId == currentUserId);

            if (productClient != null)
            {
                product.ProductsClients.Remove(productClient);
                await context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Cart));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await context.Products
                .Where (p => p.Id == id)
                .Where(p => p.IsDeleted == false)
                .AsNoTracking()
                .Select(p => new ProductDeleteViewModel()
                { 
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Seller = p.Seller.UserName ?? string.Empty,
                    SellerId = p.SellerId
                })
                .FirstOrDefaultAsync();

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(ProductDeleteViewModel model)
        {
            Product? product = await context.Products
                .Where(p => p.Id == model.Id)
                .Where(p => p.IsDeleted == false)
                .FirstOrDefaultAsync();

            if (product != null)
            {
                product.IsDeleted = true;

                await context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private async Task<List<Category>> GetCategories()
        {
            return await context.Categories.ToListAsync();
        }
    }
}
