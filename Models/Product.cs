using System.ComponentModel.DataAnnotations;

namespace ReuseMarket.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the product name.")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a product description.")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the product price.")]
        [Range(1, 100000000, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the product location.")]
        public string Location { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public DateTime DatePosted { get; set; } = DateTime.UtcNow;
    }
}