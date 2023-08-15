using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GalloFlix.Models;
using GalloFlix.Data;
using Microsoft.EntityFrameworkCore;

namespace GalloFlix.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context;

    public HomeController(ILogger<HomeController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        var movies = _context.Movies.Include(m => m.Genres).ThenInclude(g => g.Genre).ToList();
        return View(movies);
    }

    public IActionResult Movie(int? id)
    {
        var movie = _context.Movies
            .Where(m => m.Id == id)
            .Include(m => m.Genres)
            .ThenInclude(g => g.Genre)
            .SingleOrDefault();
        return View(movie);
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