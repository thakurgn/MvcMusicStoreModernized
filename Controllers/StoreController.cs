using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMusicStoreModernized.Data;

public class StoreController : Controller
{
    private readonly MusicStoreContext _context;

    public StoreController(MusicStoreContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var genres = await _context.Genres
            .OrderBy(g => g.Name)
            .ToListAsync();

        return View(genres);
    }

    public async Task<IActionResult> Browse(string genre)
    {
        if (string.IsNullOrWhiteSpace(genre))
            return RedirectToAction(nameof(Index));

        var genreModel = await _context.Genres
            .Include(g => g.Albums)
                .ThenInclude(a => a.Artist)
            .FirstOrDefaultAsync(g => g.Name == genre);

        if (genreModel == null)
            return NotFound();

        return View(genreModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var album = await _context.Albums
            .Include(a => a.Artist)
            .Include(a => a.Genre)
            .FirstOrDefaultAsync(a => a.AlbumId == id);

        if (album == null)
            return NotFound();

        return View(album);
    }
}