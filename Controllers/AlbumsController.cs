using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMusicStoreModernized.Data;
using MvcMusicStoreModernized.Models;

[Authorize(Roles = "Administrator")]
public class AlbumsController : Controller
{
    private readonly MusicStoreContext _context;

    public AlbumsController(MusicStoreContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var albums = _context.Albums
            .Include(a => a.Artist)
            .Include(a => a.Genre);

        return View(await albums.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var album = await _context.Albums
            .Include(a => a.Artist)
            .Include(a => a.Genre)
            .FirstOrDefaultAsync(m => m.AlbumId == id);

        if (album == null)
            return NotFound();

        return View(album);
    }

    public IActionResult Create()
    {
        PopulateDropdowns();
        return View();
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
        return View(album);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var album = await _context.Albums.FindAsync(id);

        if (album == null)
            return NotFound();

        PopulateDropdowns(album.ArtistId, album.GenreId);
        return View(album);
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
        return View(album);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var album = await _context.Albums
            .Include(a => a.Artist)
            .Include(a => a.Genre)
            .FirstOrDefaultAsync(m => m.AlbumId == id);

        if (album == null)
            return NotFound();

        return View(album);
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