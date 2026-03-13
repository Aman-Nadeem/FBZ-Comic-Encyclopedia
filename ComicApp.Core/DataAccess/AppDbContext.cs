using Microsoft.EntityFrameworkCore;
using ComicApp.Core.Models;

namespace ComicApp.Core.DataAccess
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<SavedSearch> SavedSearches { get; set; }
        public DbSet<FlaggedComic> FlaggedComics { get; set; }
    }
}