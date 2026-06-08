using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMusicStoreModernized.Data;
using MvcMusicStoreModernized.Models;

public class ShoppingCartController : Controller
{
    private readonly MusicStoreContext _context;
    private const string CartSessionKey = "CartId";

    public ShoppingCartController(MusicStoreContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var cartId = GetCartId();

        var cartItems = await _context.Carts
            .Include(c => c.Album)
                .ThenInclude(a => a!.Artist)
            .Where(c => c.CartId == cartId)
            .ToListAsync();

        ViewBag.CartTotal = cartItems.Sum(c => c.Count * (c.Album?.Price ?? 0));

        return View(cartItems);
    }

    public async Task<IActionResult> AddToCart(int id)
    {
        var album = await _context.Albums.FindAsync(id);

        if (album == null)
            return NotFound();

        var cartId = GetCartId();

        var cartItem = await _context.Carts
            .FirstOrDefaultAsync(c => c.CartId == cartId && c.AlbumId == id);

        if (cartItem == null)
        {
            cartItem = new Cart
            {
                CartId = cartId,
                AlbumId = id,
                Count = 1,
                DateCreated = DateTime.UtcNow
            };

            _context.Carts.Add(cartItem);
        }
        else
        {
            cartItem.Count++;
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> RemoveFromCart(int id)
    {
        var cartId = GetCartId();

        var cartItem = await _context.Carts
            .FirstOrDefaultAsync(c => c.RecordId == id && c.CartId == cartId);

        if (cartItem != null)
        {
            if (cartItem.Count > 1)
                cartItem.Count--;
            else
                _context.Carts.Remove(cartItem);

            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private string GetCartId()
    {
        var cartId = HttpContext.Session.GetString(CartSessionKey);

        if (string.IsNullOrWhiteSpace(cartId))
        {
            cartId = Guid.NewGuid().ToString();
            HttpContext.Session.SetString(CartSessionKey, cartId);
        }

        return cartId;
    }
}