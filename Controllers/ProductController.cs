using Microsoft.AspNetCore.Mvc;
using ReuseMarket.Models;

namespace ReuseMarket.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Used Laptop",
                    Description = "Good condition laptop for students.",
                    Price = 450000m,
                    Category = "Electronics",
                    Location = "Kampala"
                },

                new Product
                {
                    Id = 2,
                    Name = "Study Chair",
                    Description = "Comfortable chair for studying.",
                    Price = 80000m,
                    Category = "Furniture",
                    Location = "Wandegeya"
                },

                new Product
                {
                    Id = 3,
                    Name = "Samsung Galaxy S23",
                    Description = "Second-hand smartphone in good condition.",
                    Price = 850000m,
                    Category = "Electronics",
                    Location = "Kisementi"
                }
            };

            return View(products);
        }
    }
}