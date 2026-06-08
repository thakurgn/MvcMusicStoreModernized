using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMusicStoreModernized.Data;
using MvcMusicStoreModernized.Models;

[Authorize(Roles = "Administrator")]
public class StoreManagerController : Controller
{
    private readonly MusicStoreContext _context;

    public StoreManagerController(MusicStoreContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var albums = await _context.Albums
            .Include(a => a.Artist)
            .Include(a => a.Genre)
            .ToListAsync();

        return View("~/Views/Albums/Index.cshtml", albums);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var album = await _context.Albums
            .Include(a => a.Artist)
            .Include(a => a.Genre)
            .FirstOrDefaultAsync(a => a.AlbumId == id);

        if (album == null)
            return NotFound();

        return View("~/Views/Albums/Details.cshtml", album);
    }

    public IActionResult Create()
    {
        PopulateDropdowns();
        return View("~/Views/Albums/Create.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("AlbumId,Title,Price,AlbumArtUrl,ArtistId,GenreId")] Album album)
    {
        if (ModelState.IsValid)
        {
            _context.Add(album);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        PopulateDropdowns(album.ArtistId, album.GenreId);
        return View("~/Views/Albums/Create.cshtml", album);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var album = await _context.Albums.FindAsync(id);

        if (album == null)
            return NotFound();

        PopulateDropdowns(album.ArtistId, album.GenreId);
        return View("~/Views/Albums/Edit.cshtml", album);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("AlbumId,Title,Price,AlbumArtUrl,ArtistId,GenreId")] Album album)
    {
        if (id != album.AlbumId)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(album);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlbumExists(album.AlbumId))
                    return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        PopulateDropdowns(album.ArtistId, album.GenreId);
        return View("~/Views/Albums/Edit.cshtml", album);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var album = await _context.Albums
            .Include(a => a.Artist)
            .Include(a => a.Genre)
            .FirstOrDefaultAsync(a => a.AlbumId == id);

        if (album == null)
            return NotFound();

        return View("~/Views/Albums/Delete.cshtml", album);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var album = await _context.Albums.FindAsync(id);

        if (album != null)
            _context.Albums.Remove(album);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool AlbumExists(int id)
    {
        return _context.Albums.Any(e => e.AlbumId == id);
    }

    private void PopulateDropdowns(int? selectedArtistId = null, int? selectedGenreId = null)
    {
        ViewData["ArtistId"] = new SelectList(_context.Artists, "ArtistId", "Name", selectedArtistId);
        ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Name", selectedGenreId);
    }
}