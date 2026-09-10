using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using WEB_IMDB.Data;
using WEB_IMDB.Helpers;
using WEB_IMDB.ViewModel;

namespace WEB_IMDB;

public class MovieController:Controller
{
    private readonly AppDbContext _context;
    private readonly IToastNotification _toastNotification;
    public  MovieController(AppDbContext context, IToastNotification toastNotification)
    {
        _context = context;
        _toastNotification = toastNotification;
    }

    public async Task<IActionResult> Index()
    {
        var movies = await _context.Movies.OrderBy(s=>-s.Rate).ToListAsync();
        return View(movies);
    }

    public async Task<IActionResult> Create()
    {
        var viewModel = new MovieFromViewModel
        {
          Genres = await _context.Genres.ToListAsync(),
         Year = DateTime.Now.Year
        };
        return View("MovieForm",viewModel);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MovieFromViewModel model)
    {
        
    
        if (!ModelState.IsValid)
        {
           

            foreach (var error in ModelState)
            {
                if (error.Value.Errors.Count > 0)
                {
                    Console.WriteLine($"FIELD: {error.Key}");
                    foreach (var e in error.Value.Errors)
                        Console.WriteLine($"  -> {e.ErrorMessage} (attempted: {error.Value.AttemptedValue})");
                }
            }
            model.Genres = await _context.Genres.ToListAsync();
            return View("MovieForm",model);
        }
        
        using var memoryStream = new MemoryStream();
        await model.Image.CopyToAsync(memoryStream);
        var movie = new Movie
        {  
            Title = model.Title,
            Year = model.Year,
            Rate = model.Rate.GetValueOrDefault(),
            StoryLine = model.StoryLine,
            Image = memoryStream.ToArray(),
            GenreID = model.GenreID.Value
        };

        if (movie.Image.Length > 1024 * 1024)
        {
            model.Genres = await _context.Genres.ToListAsync();
            ModelState.AddModelError("Image", "The image is too large");
            return View("MovieForm",model);
        }
        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();
        _toastNotification.AddSuccessToastMessage("Movie created  successfully");
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return BadRequest();
        var movie =  _context.Movies.Find(id);
        if (movie == null)
            return NotFound();
        var viewModel = new MovieFromViewModel
        {
            Id = movie.Id,
            Title = movie.Title,
            GenreID =  movie.GenreID,
            Rate =  movie.Rate,
            StoryLine = movie.StoryLine,
            Year =  movie.Year,
            Image = new ByteArrayFormFile(movie.Image, movie.Title),
            Genres = await _context.Genres.OrderBy(g=>g.Name).ToListAsync(),
        };
        return View("MovieForm", viewModel);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(MovieFromViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Genres = await _context.Genres.OrderBy(g=>g.Name).ToListAsync();
            if (model.Image == null)
            {
                var existingMovie = _context.Movies.Find(model.Id);
                if (existingMovie != null)
                    model.Image = new ByteArrayFormFile(existingMovie.Image, existingMovie.Title);
            }
            return View("MovieForm",model);
        }
        var movie =  _context.Movies.Find(model.Id);
        if (movie == null)
            return NotFound();
        movie.Title = model.Title;
        movie.GenreID = model.GenreID.Value;
        movie.Year = model.Year;
        movie.Rate = model.Rate.GetValueOrDefault();
        movie.StoryLine = model.StoryLine;
        if (model.Image != null)
        {
            using var memoryStream = new MemoryStream();
            await model.Image.CopyToAsync(memoryStream);
            movie.Image = memoryStream.ToArray();
        }
        _context.SaveChanges();
        _toastNotification.AddSuccessToastMessage("Movie updated  successfully");

        return RedirectToAction(nameof(Index));

    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return BadRequest();
        var movie =  _context.Movies.Include(g=>g.Genre).SingleOrDefault(g=>g.Id==id);
        if (movie == null)
            return NotFound();
        return View(movie);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return BadRequest();
        var movie =  _context.Movies.Find(id);
        if(movie==null)
            return NotFound();
        
        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();
        _toastNotification.AddSuccessToastMessage("Movie deleted successfully");
        return RedirectToAction(nameof(Index));
    }

    


    
}
