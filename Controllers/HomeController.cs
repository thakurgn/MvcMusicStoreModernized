using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMusicStoreModernized.Data;
using MvcMusicStoreModernized.Models;

namespace MvcMusicStoreModernized.Controllers
{
    public class HomeController : Controller
    {
        private readonly MusicStoreContext _context;

        public HomeController(MusicStoreContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var albums = await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .OrderBy(a => a.AlbumId)
                .Take(5)
                .ToListAsync();

            return View(albums);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}