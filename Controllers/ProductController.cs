using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ReuseMarket.Data;
using ReuseMarket.Models;
using System.IO;
using System.Linq;

namespace ReuseMarket.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ProductController(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager)
{
    _context = context;
    _userManager = userManager;
}
      public async Task<IActionResult> Index(string searchString, string category)
{
    var products = _context.Products
    .Include(p => p.Seller)
    .AsQueryable();

    // Search products by name
    if (!string.IsNullOrEmpty(searchString))
    {
        products = products.Where(p =>
            p.Name.ToLower().Contains(searchString.ToLower()));
    }

    // Filter products by category
    if (!string.IsNullOrEmpty(category))
    {
        products = products.Where(p =>
            p.Category == category);
    }

    return View(await products.ToListAsync());
}


     public async Task<IActionResult> Details(int id)
{
    var product = await _context.Products
        .Include(p => p.Seller)
        .FirstOrDefaultAsync(p => p.Id == id);

    if (product == null)
    {
        return NotFound();
    }

    return View(product);

}
// Opens the product editing page.
[HttpGet]
[Authorize]
public async Task<IActionResult> Edit(int id)
{
    var product = await _context.Products
        .FirstOrDefaultAsync(p => p.Id == id);

    if (product == null)
    {
        return NotFound();
    }

    // Ensure only the seller can edit this product.
    var userId = _userManager.GetUserId(User);

    if (product.SellerId != userId)
    {
        return Forbid();
    }

    return View(product);
}
// Receives and saves the edited product.
[HttpPost]
[Authorize]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, Product product)
{
    if (id != product.Id)
    {
        return BadRequest();
    }

    var existingProduct = await _context.Products
        .FirstOrDefaultAsync(p => p.Id == id);

    if (existingProduct == null)
    {
        return NotFound();
    }

    // Ensure only the original seller can edit the product.
    var userId = _userManager.GetUserId(User);

    if (existingProduct.SellerId != userId)
    {
        return Forbid();
    }

    if (!ModelState.IsValid)
    {
        return View(product);
    }

    // Update only the permitted product fields.
    existingProduct.Name = product.Name;
    existingProduct.Description = product.Description;
    existingProduct.Price = product.Price;
    existingProduct.Category = product.Category;
    existingProduct.Location = product.Location;

    await _context.SaveChangesAsync();

    return RedirectToAction("Details", new { id = existingProduct.Id });
}
// Opens the product deletion confirmation page.
[HttpGet]
[Authorize]
public async Task<IActionResult> Delete(int id)
{
    var product = await _context.Products
        .FirstOrDefaultAsync(p => p.Id == id);

    if (product == null)
    {
        return NotFound();
    }

    var userId = _userManager.GetUserId(User);

    if (product.SellerId != userId)
    {
        return Forbid();
    }

    return View(product);
}
 // Receives the deletion confirmation.
[HttpPost, ActionName("Delete")]
[Authorize]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmed(int id)
{
    var product = await _context.Products
        .FirstOrDefaultAsync(p => p.Id == id);

    if (product == null)
    {
        return NotFound();
    }

    // Verify that the logged-in user owns this product.
    var userId = _userManager.GetUserId(User);

    if (product.SellerId != userId)
    {
        return Forbid();
    }

    _context.Products.Remove(product);

    await _context.SaveChangesAsync();

    return RedirectToAction("Index");
}

        // Opens the product creation page.
        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // Receives and saves the submitted product.
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

                var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("imageFile",
                        "Only JPG, PNG and WEBP images are allowed.");

                    return View(product);
                }

                if (imageFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("imageFile",
                        "Image size must not exceed 5 MB.");

                    return View(product);
                }

                string uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/images/products"
                );

                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + extension;

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                product.ImageUrl = "/images/products/" + uniqueFileName;
            }

            // Assign the currently logged-in user as the seller.
product.SellerId = _userManager.GetUserId(User);

// Save the product to PostgreSQL.
_context.Products.Add(product);

await _context.SaveChangesAsync();
            // Return to the products page.
            return RedirectToAction("Index");
        }
    }
}