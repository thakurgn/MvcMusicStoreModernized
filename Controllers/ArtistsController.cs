using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMusicStoreModernized.Data;
using MvcMusicStoreModernized.Models;

[Authorize(Roles = "Administrator")]
public class ArtistsController : Controller
{
    private readonly MusicStoreContext _context;

    public ArtistsController(MusicStoreContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var artists = await _context.Artists
            .Include(a => a.Albums)
            .ToListAsync();

        return View(artists);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var artist = await _context.Artists
            .Include(a => a.Albums)
            .FirstOrDefaultAsync(a => a.ArtistId == id);

        if (artist == null)
            return NotFound();

        return View(artist);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ArtistId,Name")] Artist artist)
    {
        if (ModelState.IsValid)
        {
            _context.Add(artist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(artist);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var artist = await _context.Artists.FindAsync(id);

        if (artist == null)
            return NotFound();

        return View(artist);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("ArtistId,Name")] Artist artist)
    {
        if (id != artist.ArtistId)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(artist);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArtistExists(artist.ArtistId))
                    return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(artist);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var artist = await _context.Artists
            .Include(a => a.Albums)
            .FirstOrDefaultAsync(a => a.ArtistId == id);

        if (artist == null)
            return NotFound();

        return View(artist);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var artist = await _context.Artists
            .Include(a => a.Albums)
            .FirstOrDefaultAsync(a => a.ArtistId == id);

        if (artist == null)
            return NotFound();

        if (artist.Albums != null && artist.Albums.Count > 0)
        {
            ModelState.AddModelError("", "This artist cannot be deleted because albums are linked to it.");
            return View("Delete", artist);
        }

        _context.Artists.Remove(artist);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private bool ArtistExists(int id)
    {
        return _context.Artists.Any(e => e.ArtistId == id);
    }
}