using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WEB_IMDB.Data;
using WEB_IMDB.ViewModel;

namespace WEB_IMDB;

public class MovieController:Controller
{
    private readonly AppDbContext _context;
    public  MovieController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var movies = await _context.Movies.ToListAsync();
        return View(movies);
    }

    public async Task<IActionResult> Create()
    {
        var viewModel = new MovieFromViewModel
        {
          Genres = await _context.Genres.ToListAsync(),
         Year = DateTime.Now.Year
        };
        return View(viewModel);
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
            return View(model);
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
            return View(model);
        }
        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    
}
