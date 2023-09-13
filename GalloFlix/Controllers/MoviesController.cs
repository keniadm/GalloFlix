using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GalloFlix.Data;
using GalloFlix.Models;

namespace GalloFlix.Controllers
{
    public class MoviesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public MoviesController(AppDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movies.ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Movies == null)
            {
                return NotFound();
            }
            var movie = await _context.Movies
                .Where(m => m.Id == id)
                .Include(m => m.Genres).ThenInclude(g => g.Genre)
                .SingleOrDefaultAsync();
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            ViewData["Genres"] = new MultiSelectList(_context.Genres.OrderBy(t => t.Name), "Id", "Name");
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,OriginalTitle,Synopsis,MovieYear,Duration,AgeRating,Image")] Movie movie, IFormFile formFile, List<string> Genres)
        {
            if (ModelState.IsValid)
            {
                // Salva o Filme
                _context.Add(movie);
                await _context.SaveChangesAsync();

                // Se tiver arquivo de imagem, salva a imagem no servidor com o ID do filme e adiciona o nome e caminho da imagem no banco
                if (formFile != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = movie.Id.ToString("00") + Path.GetExtension(formFile.FileName);
                    string uploads = Path.Combine(wwwRootPath, @"img\movies");
                    string newFile = Path.Combine(uploads, fileName);
                    using (var stream = new FileStream(newFile, FileMode.Create))
                    {
                        formFile.CopyTo(stream);
                    }
                    movie.Image = @"\img\movies\" + fileName;
                    await _context.SaveChangesAsync();
                }

                // Salva, se tiver, os Gêneros selecionados
                movie.Genres = new List<MovieGenre>();
                foreach (var genre in Genres)
                {
                    movie.Genres.Add(new MovieGenre()
                    {
                        GenreId = byte.Parse(genre),
                        MovieId = movie.Id
                    });
                }
                if (Genres.Count > 0) await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["Genres"] = new MultiSelectList(_context.Genres.OrderBy(t => t.Name), "Id", "Name");
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Movies == null)
            {
                return NotFound();
            }
            var movie = await _context.Movies
                .Where(m => m.Id == id)
                .Include(m => m.Genres).ThenInclude(g => g.Genre)
                .SingleOrDefaultAsync();
            if (movie == null)
            {
                return NotFound();
            }
            var x = new MultiSelectList(_context.Genres.OrderBy(t => t.Name), "Id", "Name", movie.Genres.Select(g => g.Genre.Id.ToString()));
            ViewData["Genres"] = x;
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,OriginalTitle,Synopsis,MovieYear,Duration,AgeRating,Image")] Movie movie, IFormFile formFile, List<string> Genres)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Atualiza a Foto de Capa
                    if (formFile != null)
                    {
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        if (movie.Image != null)
                        {
                            string oldFile = Path.Combine(wwwRootPath, movie.Image.TrimStart('\\'));
                            if (System.IO.File.Exists(oldFile))
                            {
                                System.IO.File.Delete(oldFile);
                            }
                        }

                        string fileName = movie.Id.ToString("00") + Path.GetExtension(formFile.FileName);
                        string uploads = Path.Combine(wwwRootPath, @"img\pokemons");
                        string newFile = Path.Combine(uploads, fileName);
                        using (var stream = new FileStream(newFile, FileMode.Create))
                        {
                            formFile.CopyTo(stream);
                        }
                        movie.Image = @"\img\movies\" + fileName;
                    }
                    movie.Genres = _context.MovieGenres.Where(mg => mg.MovieId == movie.Id).OrderBy(mg => mg.GenreId).ToList();
                    _context.Update(movie);
                    _context.RemoveRange(movie.Genres);
                    await _context.SaveChangesAsync();

                    // Adiciona os Gêneros do Filme
                    Genres.ForEach(g => _context.MovieGenres.Add(
                        new MovieGenre()
                        {
                            MovieId = movie.Id,
                            GenreId = byte.Parse(g)
                        }
                    ));
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Genres"] = new SelectList(_context.Genres.OrderBy(t => t.Name), "Id", "Name");
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Movies == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Where(m => m.Id == id)
                .Include(m => m.Genres).ThenInclude(g => g.Genre)
                .SingleOrDefaultAsync();
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Movies == null)
            {
                return Problem("Entity set 'AppDbContext.Movies'  is null.");
            }
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                var movieGenres = await _context.MovieGenres.Where(mg => mg.MovieId == id).ToListAsync();
                _context.MovieGenres.RemoveRange(movieGenres);
                _context.Movies.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}