using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReuseMarket.Data;
using ReuseMarket.Models;

namespace ReuseMarket.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MessageController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Send(
    int productId,
    string content)
{
    // Validate the message.
    if (string.IsNullOrWhiteSpace(content) || content.Length > 1000)
    {
        TempData["MessageError"] = "Please enter a message (maximum 1000 characters).";
        return RedirectToAction("Details", "Product", new { id = productId });
    }

    // Find the product.
    var product = await _context.Products
        .FirstOrDefaultAsync(p => p.Id == productId);

    if (product == null)
    {
        return NotFound();
    }

    // Identify the logged-in buyer.
    var senderId = _userManager.GetUserId(User);

    if (string.IsNullOrEmpty(senderId))
    {
        return Challenge();
    }

    // Check that the product has a seller.
    if (string.IsNullOrEmpty(product.SellerId))
    {
        TempData["MessageError"] = "This product has no registered seller.";
        return RedirectToAction("Details", "Product", new { id = productId });
    }

    // Prevent sellers from messaging themselves.
    if (senderId == product.SellerId)
    {
        return Forbid();
    }

    // Create the message.
    var message = new Message
    {
        SenderId = senderId,
        ReceiverId = product.SellerId,
        ProductId = product.Id,
        Content = content.Trim(),
        SentAt = DateTime.UtcNow,
        IsRead = false
    };

    _context.Messages.Add(message);

    await _context.SaveChangesAsync();

    TempData["MessageSuccess"] = "Your message was sent successfully!";

    return RedirectToAction("Details", "Product", new { id = productId });
}
// Displays messages received by the logged-in user.
public async Task<IActionResult> Inbox()
{
    var userId = _userManager.GetUserId(User);

    var messages = await _context.Messages
        .Include(m => m.Sender)
        .Include(m => m.Product)
        .Where(m => m.ReceiverId == userId)
        .OrderByDescending(m => m.SentAt)
        .ToListAsync();

    // Mark unread messages as read
    foreach (var message in messages)
    {
        if (!message.IsRead)
        {
            message.IsRead = true;
        }
    }

    await _context.SaveChangesAsync();

    return View(messages);
}
    public async Task<IActionResult> Conversation(int productId, string otherUserId)
{
    var userId = _userManager.GetUserId(User);

    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(otherUserId))
    {
        return BadRequest();
    }

    var messages = await _context.Messages
        .Include(m => m.Sender)
        .Include(m => m.Receiver)
        .Include(m => m.Product)
        .Where(m => m.ProductId == productId &&
            ((m.SenderId == userId && m.ReceiverId == otherUserId) ||
             (m.SenderId == otherUserId && m.ReceiverId == userId)))
        .OrderBy(m => m.SentAt)
        .ToListAsync();

    if (!messages.Any())
    {
        return NotFound();
    }

    // Mark messages received by the current user as read
    foreach (var message in messages)
    {
        if (message.ReceiverId == userId && !message.IsRead)
        {
            message.IsRead = true;
        }
    }

    await _context.SaveChangesAsync();

    return View(messages);
}
    
    [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Reply(int productId, string otherUserId, string content)
{
    var userId = _userManager.GetUserId(User);

    if (string.IsNullOrEmpty(userId) ||
        string.IsNullOrEmpty(otherUserId))
    {
        return BadRequest();
    }

    if (string.IsNullOrWhiteSpace(content) || content.Length > 1000)
    {
        TempData["ReplyError"] = "Please enter a message (maximum 1000 characters).";

        return RedirectToAction("Conversation",
            new { productId, otherUserId });
    }

    // Confirm that this conversation exists and the user is a participant.
    bool conversationExists = await _context.Messages.AnyAsync(m =>
        m.ProductId == productId &&
        ((m.SenderId == userId && m.ReceiverId == otherUserId) ||
         (m.SenderId == otherUserId && m.ReceiverId == userId)));

    if (!conversationExists)
    {
        return NotFound();
    }

    var message = new Message
    {
        SenderId = userId,
        ReceiverId = otherUserId,
        ProductId = productId,
        Content = content.Trim(),
        SentAt = DateTime.UtcNow,
        IsRead = false
    };

    _context.Messages.Add(message);

    await _context.SaveChangesAsync();

    return RedirectToAction("Conversation",
        new { productId, otherUserId });
}
}
    
}