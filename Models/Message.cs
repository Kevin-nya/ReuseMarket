using System.ComponentModel.DataAnnotations;

namespace ReuseMarket.Models
{
    public class Message
    {
        public int Id { get; set; }

        [Required]
        public string SenderId { get; set; } = string.Empty;

        [Required]
        public string ReceiverId { get; set; } = string.Empty;

        public int ProductId { get; set; }

        [Required]
        [StringLength(1000)]
        public string Content { get; set; } = string.Empty;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;

        public ApplicationUser? Sender { get; set; }

        public ApplicationUser? Receiver { get; set; }

        public Product? Product { get; set; }
    }
}