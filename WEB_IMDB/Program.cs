using Microsoft.EntityFrameworkCore;
using WEB_IMDB.Data;

namespace WEB_IMDB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null);
                    }));
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        context.Database.EnsureCreated();
                        break;
                    }
                    catch
                    {
                        Thread.Sleep(5000);
                    }
                }

                if (!context.Genres.Any())
                {
                    var genres = new List<Genre>
                    {
                        new Genre { Name = "Action" },
                        new Genre { Name = "Comedy" }
                    };
                    context.Genres.AddRange(genres);
                    context.SaveChanges();
                }

                if (!context.Movies.Any())
                {
                    var placeholderImage = System.Text.Encoding.UTF8.GetBytes("placeholder");
                    var genres = context.Genres.ToList();
                    var movies = new List<Movie>
                    {
                        new Movie { Title = "The Dark Knight", Year = 2008, Rate = 9.0, StoryLine = "Batman raises the stakes in his war on crime.", Image = placeholderImage, Genre = genres[0] },
                        new Movie { Title = "Inception", Year = 2010, Rate = 8.8, StoryLine = "A thief who steals corporate secrets through dream-sharing technology.", Image = placeholderImage, Genre = genres[0] },
                        new Movie { Title = "The Hangover", Year = 2009, Rate = 7.7, StoryLine = "Three buddies wake up from a bachelor party in Las Vegas.", Image = placeholderImage, Genre = genres[1] },
                        new Movie { Title = "Superbad", Year = 2007, Rate = 7.6, StoryLine = "Two co-dependent high school seniors are forced to deal with separation anxiety.", Image = placeholderImage, Genre = genres[1] },
                        new Movie { Title = "Deadpool", Year = 2016, Rate = 8.0, StoryLine = "A wisecracking mercenary hunts the man who ruined his looks.", Image = placeholderImage, Genre = genres[0] }
                    };
                    context.Movies.AddRange(movies);
                    context.SaveChanges();
                }
            }

            app.Run();
        }
    }
}
