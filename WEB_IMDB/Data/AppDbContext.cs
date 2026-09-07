using Microsoft.EntityFrameworkCore;
using WEB_IMDB;

namespace WEB_IMDB.Data;

public class AppDbContext : DbContext
{
    public DbSet<Movie> Movies{get;set;}
    public DbSet<Genre> Genres{get;set;}
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
}
