using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMusicStoreModernized.Data;
using MvcMusicStoreModernized.Models;

[Authorize]
public class CheckoutController : Controller
{
    private readonly MusicStoreContext _context;
    private const string CartSessionKey = "CartId";
    private const string PromoCode = "FREE";

    public CheckoutController(MusicStoreContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return RedirectToAction(nameof(AddressAndPayment));
    }

    public IActionResult AddressAndPayment()
    {
        return View(new Order());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddressAndPayment(Order order, string promoCode)
    {
        order.Username = User.Identity?.Name ?? string.Empty;
        ModelState.Remove(nameof(Order.Username));

        var cartId = HttpContext.Session.GetString(CartSessionKey);

        if (string.IsNullOrWhiteSpace(cartId))
            return RedirectToAction("Index", "ShoppingCart");

        var cartItems = await _context.Carts
            .Include(c => c.Album)
            .Where(c => c.CartId == cartId)
            .ToListAsync();

        if (!cartItems.Any())
            return RedirectToAction("Index", "ShoppingCart");

        if (!string.Equals(promoCode, PromoCode, StringComparison.OrdinalIgnoreCase))
            ModelState.AddModelError("promoCode", "Invalid promo code. Use FREE.");

        if (!ModelState.IsValid)
            return View(order);

        order.OrderDate = DateTime.UtcNow;
        order.Total = cartItems.Sum(c => c.Count * (c.Album?.Price ?? 0));

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        foreach (var item in cartItems)
        {
            _context.OrderDetails.Add(new OrderDetail
            {
                OrderId = order.OrderId,
                AlbumId = item.AlbumId,
                Quantity = item.Count,
                UnitPrice = item.Album?.Price ?? 0
            });
        }

        _context.Carts.RemoveRange(cartItems);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Complete), new { id = order.OrderId });
    }

    public async Task<IActionResult> Complete(int id)
    {
        var username = User.Identity?.Name ?? string.Empty;

        var order = await _context.Orders
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Album)
            .FirstOrDefaultAsync(o => o.OrderId == id && o.Username == username);

        if (order == null)
            return NotFound();

        return View(order);
    }
}