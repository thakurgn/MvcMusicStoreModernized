using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMusicStoreModernized.Data;
using MvcMusicStoreModernized.Models;


[Authorize(Roles = "Administrator")]
public class GenresController : Controller
{
    private readonly MusicStoreContext _context;

    public GenresController(MusicStoreContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var genres = await _context.Genres
            .Include(g => g.Albums)
            .ToListAsync();

        return View(genres);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var genre = await _context.Genres
            .Include(g => g.Albums)
            .FirstOrDefaultAsync(g => g.GenreId == id);

        if (genre == null) return NotFound();

        return View(genre);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("GenreId,Name,Description")] Genre genre)
    {
        if (ModelState.IsValid)
        {
            _context.Add(genre);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(genre);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var genre = await _context.Genres.FindAsync(id);

        if (genre == null) return NotFound();

        return View(genre);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("GenreId,Name,Description")] Genre genre)
    {
        if (id != genre.GenreId) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(genre);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenreExists(genre.GenreId)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(genre);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var genre = await _context.Genres
            .Include(g => g.Albums)
            .FirstOrDefaultAsync(g => g.GenreId == id);

        if (genre == null) return NotFound();

        return View(genre);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var genre = await _context.Genres
            .Include(g => g.Albums)
            .FirstOrDefaultAsync(g => g.GenreId == id);

        if (genre == null) return NotFound();

        if (genre.Albums != null && genre.Albums.Count > 0)
        {
            ModelState.AddModelError("", "This genre cannot be deleted because albums are linked to it.");
            return View("Delete", genre);
        }

        _context.Genres.Remove(genre);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private bool GenreExists(int id)
    {
        return _context.Genres.Any(e => e.GenreId == id);
    }
}