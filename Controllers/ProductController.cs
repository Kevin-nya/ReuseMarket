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

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();

            return View(products);
        }

        // Opens the product creation page.
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Receives and saves the submitted product.
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

            // Save the product to PostgreSQL.
            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            // Return to the products page.
            return RedirectToAction("Index");
        }
    }
}