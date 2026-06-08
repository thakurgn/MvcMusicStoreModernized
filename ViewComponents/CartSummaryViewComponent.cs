using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMusicStoreModernized.Data;

public class CartSummaryViewComponent : ViewComponent
{
    private readonly MusicStoreContext _context;
    private const string CartSessionKey = "CartId";

    public CartSummaryViewComponent(MusicStoreContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var cartId = HttpContext.Session.GetString(CartSessionKey);
        var count = 0;

        if (!string.IsNullOrWhiteSpace(cartId))
        {
            count = await _context.Carts
                .Where(c => c.CartId == cartId)
                .SumAsync(c => c.Count);
        }

        return View(count);
    }
}