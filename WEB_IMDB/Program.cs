using Microsoft.EntityFrameworkCore;
using NToastNotify;
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
            builder.Services.AddMvc().AddNToastNotifyToastr(new ToastrOptions()
            {
              ProgressBar = true,
              PositionClass = ToastPositions.TopRight,
              PreventDuplicates = true,
              CloseButton = true
              
            });
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
                        new Genre { Name = "Comedy" },
                        new Genre { Name = "Drama" },
                        new Genre { Name = "Crime" },
                        new Genre { Name = "Thriller" },
                        new Genre { Name = "Sci-Fi" }
                    };
                    context.Genres.AddRange(genres);
                    context.SaveChanges();
                }

                if (!context.Movies.Any())
                {
                    var imagesPath = Path.Combine(app.Environment.WebRootPath, "images");
                    var genres = context.Genres.ToList();
                    var movies = new List<Movie>
                    {
                        new Movie
                        {
                            Title = "The Shawshank Redemption",
                            Year = 1994,
                            Rate = 9.3,
                            StoryLine = "Over the course of several years, two convicts form a friendship, seeking consolation and eventual redemption through basic compassion.",
                            Image = File.ReadAllBytes(Path.Combine(imagesPath, "shawshank.jpg")),
                            Genre = genres.First(g => g.Name == "Drama")
                        },
                        new Movie
                        {
                            Title = "The Dark Knight",
                            Year = 2008,
                            Rate = 9.0,
                            StoryLine = "When the menace known as the Joker wreaks havoc on Gotham, Batman must accept one of the greatest tests to fight injustice.",
                            Image = File.ReadAllBytes(Path.Combine(imagesPath, "dark_knight.jpg")),
                            Genre = genres.First(g => g.Name == "Action")
                        },
                        new Movie
                        {
                            Title = "Inception",
                            Year = 2010,
                            Rate = 8.8,
                            StoryLine = "A thief who steals corporate secrets through dream-sharing technology is given the task of planting an idea into a CEO's mind.",
                            Image = File.ReadAllBytes(Path.Combine(imagesPath, "inception.jpg")),
                            Genre = genres.First(g => g.Name == "Sci-Fi")
                        },
                        new Movie
                        {
                            Title = "The Godfather",
                            Year = 1972,
                            Rate = 9.2,
                            StoryLine = "The aging patriarch of an organized crime dynasty transfers control of his empire to his reluctant youngest son.",
                            Image = File.ReadAllBytes(Path.Combine(imagesPath, "godfather.jpg")),
                            Genre = genres.First(g => g.Name == "Crime")
                        },
                        new Movie
                        {
                            Title = "Pulp Fiction",
                            Year = 1994,
                            Rate = 8.9,
                            StoryLine = "The lives of two mob hitmen, a boxer, a gangster and his wife intertwine in four tales of violence and redemption.",
                            Image = File.ReadAllBytes(Path.Combine(imagesPath, "pulp_fiction.jpg")),
                            Genre = genres.First(g => g.Name == "Crime")
                        },
                        new Movie
                        {
                            Title = "Interstellar",
                            Year = 2014,
                            Rate = 8.7,
                            StoryLine = "A team of explorers travel through a wormhole in space in an attempt to ensure humanity's survival.",
                            Image = File.ReadAllBytes(Path.Combine(imagesPath, "interstellar.jpg")),
                            Genre = genres.First(g => g.Name == "Sci-Fi")
                        },
                        new Movie
                        {
                            Title = "The Matrix",
                            Year = 1999,
                            Rate = 8.7,
                            StoryLine = "A computer hacker learns from mysterious rebels about the true nature of his reality and his role in the war against its controllers.",
                            Image = File.ReadAllBytes(Path.Combine(imagesPath, "the_matrix.jpg")),
                            Genre = genres.First(g => g.Name == "Action")
                        },
                        new Movie
                        {
                            Title = "Forrest Gump",
                            Year = 1994,
                            Rate = 8.8,
                            StoryLine = "The presidencies of Kennedy and Johnson, the Vietnam War, the Watergate scandal and other historical events unfold from the perspective of an Alabama man.",
                            Image = File.ReadAllBytes(Path.Combine(imagesPath, "forrest_gump.jpg")),
                            Genre = genres.First(g => g.Name == "Drama")
                        },
                        new Movie
                        {
                            Title = "Spirited Away",
                            Year = 2001,
                            Rate = 8.6,
                            StoryLine = "During her family's move to the suburbs, a sullen 10-year-old girl wanders into a world ruled by gods, witches, and spirits.",
                            Image = File.ReadAllBytes(Path.Combine(imagesPath, "spirited_away.jpg")),
                            Genre = genres.First(g => g.Name == "Comedy")
                        },
                        new Movie
                        {
                            Title = "Fight Club",
                            Year = 1999,
                            Rate = 8.8,
                            StoryLine = "An insomniac office worker and a devil-may-care soap maker form an underground fight club that evolves into much more.",
                            Image = File.ReadAllBytes(Path.Combine(imagesPath, "fight_club.jpg")),
                            Genre = genres.First(g => g.Name == "Thriller")
                        }
                    };
                    context.Movies.AddRange(movies);
                    context.SaveChanges();
                }
            }

            app.Run();
        }
    }
}
