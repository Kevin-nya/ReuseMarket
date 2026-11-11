using Microsoft.AspNetCore.Identity;

namespace ReuseMarket.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}