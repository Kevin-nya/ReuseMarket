using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReuseMarket.Data;
using ReuseMarket.Models;

namespace ReuseMarket.ViewComponents
{
    public class UnreadMessagesViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UnreadMessagesViewComponent(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                return Content(string.Empty);
            }

            var unreadCount = await _context.Messages
                .CountAsync(m => m.ReceiverId == user.Id && !m.IsRead);

            return View(unreadCount);
        }
    }
}